using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.AI;
using Assets.Scripts.Model;
using Assets.Scripts.CBR.Model;
using System;
using Assets.Scripts.CBR.Plan;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Util
{
    /**
 * Diese Klasse stellt einige Methoden zur Verfügung, die häufiger in der Unity-Welt benötigt werden.
 */
    public class CommonUnityFunctions
    {
        /**
         * Der Sichtradius des Bots (dieser Wert entspricht auch ca. dem Sichtfeld des Menschen) in °.
         */
        private static float mFieldOfViewInDegrees = 214f;
        /**
         * Diese Variable gibt an, ob die Rotation des Spielers innerhalb der Bewegung abgeschlossen ist.
         */
        public static bool mRotationFinished = false;
        /**
         * Die Distanz zwischen einem Spieler und seinem Ziel. Wäre die Distanz 0, so würde der Spieler direkt mit seinem Ziel kollidieren, für das Einsammeln von Pick-Ups notwendig, für die normale Bewegung ist 0 aber ungeeignet.
         */
        public const float NORMAL_STOPPING_DISTANCE = 8f;

        /**
         * Diese Methode ermittelt aus der Liste von Spielern den CBR-Spieler und den Non-CBR-Spieler und gibt diese als Tuple zurück, wobei der CBR-Spieler IMMER das ERSTE Element des Tuples ist.
         */
        public static Tuple<Player, Player> GetBotPlayersCorrectly()
        {
            Tuple<Player, Player> playerTuple;

            Player playerWithCBR = null;
            Player playerWithoutCBR = null;

            foreach (Player player in GameControllerScript.mPlayers)
            {
                if (player.mGameObject.GetComponent<BotBehaviourScript>())
                {
                    playerWithoutCBR = player;
                }
                else if (player.mGameObject.GetComponent<BotCBRBehaviourScript>())
                {
                    playerWithCBR = player;
                }
            }

            playerTuple = Tuple.Create(playerWithCBR, playerWithoutCBR);
            return playerTuple;
        }
        /**
         * Diese Methode gibt anhand eines gegebenen Spielers die Position des gegnerischen Spielers zurück.
         */
        public static Transform GetEnemyPosition(Player lookingPlayer)
        {

            Player other = null;

            foreach (Player player in GameControllerScript.mPlayers)
            {
                if (!player.mName.Equals(lookingPlayer.mName))
                {
                    other = player;
                    break;
                }
            }

            Transform position = null;
            RaycastHit hit;
            Transform lpTrans = lookingPlayer.mGameObject.transform;
            Transform otherTrans = other.mGameObject.transform;
            var rayDirection = (otherTrans.position - lpTrans.position) + new Vector3(0, 1, 0);

            if (Vector3.Angle(rayDirection, lpTrans.forward) <= (mFieldOfViewInDegrees * 0.5f))
            {
                Debug.DrawRay(lpTrans.position, rayDirection, Color.green);
                if (Physics.Raycast(lpTrans.position, rayDirection, out hit))
                {
                    if (hit.transform.gameObject.name.Equals(other.mName))
                    {
                        position = hit.transform;
                    }
                }
            }
            return position;
        }

        /**
         * Strafe von Viefhaus: Ermöglicht das Umrunden eines Gegners in offensiven Aktionen
         */
        public static void Strafe(Player player)
        {
            NavMeshAgent nma = player.mGameObject.GetComponent<NavMeshAgent>();
            Transform tLookingPlayer = player.mGameObject.transform;

            Vector3 leftPosition = -tLookingPlayer.right;
            Vector3 rightPosition = tLookingPlayer.right;

            bool canMoveLeft = !Physics.Raycast(tLookingPlayer.position, tLookingPlayer.TransformDirection(leftPosition), 0.1f);
            bool canMoveRight = !Physics.Raycast(tLookingPlayer.position, tLookingPlayer.TransformDirection(rightPosition), 0.1f);

            //Immer links umrunden
            if (canMoveLeft)
            {
                nma.Move(leftPosition * 0.5f);
            }
            //Wenn nicht möglich nach rechts, bzw. stehen bleiben
            else if (canMoveRight)
            {
                nma.Move(rightPosition * 0.5f);
            }
        }

        /**
         * Diese Methode führt den vorgeschlagenen Plan für den CBR-Spieler aus.
         */
        public static void ExecutePlan(Player player, Player enemy, Status status)
        {
            BotCBRBehaviourScript.mFirstTime = false;
            player.mPlan.progress = (int)Plan.Progress.IN_PROGRESS;

            CBR.Plan.Action action;

            int upperEnd = player.mPlan.GetActionCount();

            for (int i = 0; i < upperEnd; i++)
            {

                //Debug.Log("CommonUnityFunctions#ExecutePlan#for (int i = " + i + "; " + i + " < " + upperEnd + "; " + i + "++)");

                //C.W.: uses the upper end plan from plan stack
                action = player.mPlan.GetActionByIndex(i);
                
                //C.W.: pre checks if the suggested plan is not seeking or camping
                // if so immediately disable the respective scripts
                if (action.GetType() != typeof(Seek))
                {
                    player.mGameObject.GetComponent<SeekScript>().enabled = false;
                }
                if (action.GetType() != typeof(Camp))
                {
                    player.mGameObject.GetComponent<CampScript>().enabled = false;
                }


                if(action.GetType() == typeof(Camp))
                {
                    player.mGameObject.GetComponent<CampScript>().enabled = true;
                }
                if (action.GetType() == typeof(Seek))
                {
                    // C.W.: let the agent go seek.
                    // the seek action is used as a fallback. If not other plan does match the situation 
                    // the agent go seek.
                    player.mGameObject.GetComponent<SeekScript>().enabled = true;

                    // TODO: rotates the player in the direction of enemiesLastKnownPosition if set BUT
                    // at the moment the value (enemiesLastKnownPosition) wont be set correctly.
                    if (player.mStatus.enemiesLastKnownPosition != new Vector3(10000f, 10000f, 10000f))
                    {
                        RotateTowards(player, player.mStatus.enemiesLastKnownPosition);
                    }

                } else if (action.GetType() == typeof(MoveToEnemy))
                {
                    // C.W.: theoretically its cheating because if the CBR player gets the suggested
                    // plan "MoveToEnemy" he implicitly uses enemys position BUT
                    // CBR Engine is designed that THEORETICALLY the CBR agent can not get the plan MoveToEnemy
                    // without isEnemyVisible = true. This should prevent him from cheating. But to be honest its not
                    // strictly enough.

                    MoveTo(player, enemy.mGameObject.transform.position, NORMAL_STOPPING_DISTANCE);
                    LookAt(player, enemy);
                }
                else if (action.GetType() == typeof(CollectItem))
                {
                    CollectItem collect = (CollectItem)action;
                    string dest = collect.destination;

                    switch (dest)
                    {
                        case "ammu":
                            if (status.ammuPosition.x != 10000f)
                            {
                                MoveTo(player, status.ammuPosition, 0);
                            }
                            break;
                        case "health":
                            if (status.healthPosition.x != 10000f)
                            {
                                MoveTo(player, status.healthPosition, 0);
                            }
                            break;

                        case "weapon":
                            if (status.weaponPosition.x != 10000f)
                            {
                                MoveTo(player, status.weaponPosition, 0);
                            }
                            break;
                    }
                }
                else if (action.GetType() == typeof(Reload))
                {
                    player.Reload();
                }
                else if (action.GetType() == typeof(Shoot))
                {
                    //// C.W.: checks if enemy is in range of agents current weapon, if so shoot
                    if (status.distanceToEnemy <= (int)player.mPlayerShooting.mRange)
                    {
                        LookAt(player, enemy);
                        if(status.distanceToEnemy == (int)Status.Distance.middle)
                        {
                            if (EnemyInShootingLine(player, enemy))
                            {
                                LookAt(player, enemy);
                                player.Shoot();
                            }
                        }else if (status.distanceToEnemy <= (int)Status.Distance.near)
                        {
                            LookAt(player, enemy);
                            player.Shoot();
                            Strafe(player);
                        }
                    }
                }
                else if (action.GetType() == typeof(SwitchWeapon))
                {
                    player.SwitchWeapon();
                }
                else if (action.GetType() == typeof(UseCover))
                {
                    MoveTo(player, GetNearestCover(player).transform.position, NORMAL_STOPPING_DISTANCE);
                    if (DestinationReached(player))
                    {
                        Debug.Log(player.mName + " is now covered!");
                        player.mStatus.isCovered = true;
                        action.finished = true;
                    }
                }

                if (action.finished)
                {
                    Debug.Log(action.name + " is finished and gets removed");
                    player.mPlan.RemoveAction(action);
                    i = 0;
                    upperEnd = player.mPlan.GetActionCount();
                }
                //Debug.Log("THERE ARE " + player.mPlan.GetActionCount() + " actions in my plan");
            }

        }
        /**
         * Diese Methode rotiert einen gegebenen Spieler zu einer gegebenen Position.
         */
        public static void RotateTowards(Player lookingPlayer, Vector3 position)
        {
            if (lookingPlayer.mGameObject.GetComponent<NavMeshAgent>().angularSpeed != 999f)
            {
                lookingPlayer.mGameObject.GetComponent<NavMeshAgent>().angularSpeed = 999f;
            }
            Vector3 eulerAngles = Vector3.Lerp(lookingPlayer.mGameObject.transform.eulerAngles, position, 25f * Time.deltaTime);
            lookingPlayer.mGameObject.transform.eulerAngles = eulerAngles;
        }
        /**
         * Diese Methode gibt zurück, ob ein gegebenen Spieler seinen Gegenspieler sehen kann.
         */
        public static bool CanSeeEnemy(Player lookingPlayer)
        {
            return GetEnemyPosition(lookingPlayer) != null;
        }
        /**
         * Diese Methode gibt die Distanz zwischen zwei gegebenen Vektoren zurück.
         */
        public static float GetDistanceBetweenGameObjects(Vector3 object1, Vector3 object2)
        {
            return Vector3.Distance(object1, object2);
        }
        /**
         * Diese Methode gibt zurück, ob der Spieler eine weitere Waffe benötigt.
         */
        public static bool NeedAnotherWeapon(Player player)
        {
            return player.GetWeaponCount() < 2;
        }
        /**
         * Diese Methode gibt zurück, ob der Spieler Munition benötigt.
         */
        public static bool NeedAmmunition(Player player)
        {
            return player.mEquippedWeapon.mCurrentOverallAmmu < (player.mEquippedWeapon.mMaxAmmu * 0.5);
        }
        /**
         * Diese Methode gibt zurück, ob der Spieler Lebenspunkte benötigt.
         */
        public static bool NeedHealth(Player player)
        {
            return player.mPlayerHealth < (Player.mMaxLife * 0.5);
        }
        /**
         * Diese Methode gibt zurück, ob ein gegebener Spieler die einsammelbare Waffe sehen kann.
         */
        public static bool CanSeeWeapon(Player player)
        {
            return GetWeaponPosition(player) != null;
        }
        /**
         * Diese Methode gibt für einen gebenen Spieler die Position der einsammelbaren Waffe zurück. Ist die Sicht auf die Waffe möglich UND die Waffe ist im Sichtfeld, so wird die Position zurückgegeben, ansonsten wird NULL zurückgegeben.
         */
        public static Transform GetWeaponPosition(Player player)
        {
            Transform transform = null;
            GameObject m4a1 = GameObject.FindGameObjectWithTag("Machine Gun Collectable");

            if (m4a1)
            {
                RaycastHit hit;
                var rayDirection = (m4a1.transform.position - player.mGameObject.transform.position) + new Vector3(0, 0.4f, 0);

                if (Vector3.Angle(rayDirection, player.mGameObject.transform.forward) <= (mFieldOfViewInDegrees * 0.5f))
                {
                    Debug.DrawRay(player.mGameObject.transform.position, rayDirection, Color.red);
                    if (Physics.Raycast(player.mGameObject.transform.position, rayDirection, out hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.gameObject.tag.Equals("Machine Gun Collectable"))
                        {
                            transform = hit.transform;
                        }
                    }
                }
            }
            return transform;
        }
        /**
         * Diese Methode gibt zurück, ob ein gegebener Spieler die Munitionskiste sehen kann.
         */
        public static bool CanSeeAmmu(Player player)
        {
            return GetAmmuPosition(player) != null;
        }
        /**
         * Diese Methode gibt für einen gebenen Spieler die Position der Munitionskiste zurück. Ist die Sicht auf die Waffe möglich UND die Waffe ist im Sichtfeld, so wird die Position zurückgegeben, ansonsten wird NULL zurückgegeben.
         */
        public static Transform GetAmmuPosition(Player player)
        {

            Transform transform = null;

            GameObject ammu = GameObject.FindGameObjectWithTag("Ammunition");

            if (ammu)
            {
                RaycastHit hit;
                var rayDirection = (ammu.transform.position - player.mGameObject.transform.position) + new Vector3(0, 0.4f, 0);

                if (Vector3.Angle(rayDirection, player.mGameObject.transform.forward) <= (mFieldOfViewInDegrees * 0.5f))
                {
                    Debug.DrawRay(player.mGameObject.transform.position, rayDirection, Color.yellow);
                    if (Physics.Raycast(player.mGameObject.transform.position, rayDirection, out hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.gameObject.tag.Equals("Ammunition"))
                        {
                            transform = hit.transform;
                        }
                    }
                }
            }
            return transform;
        }
        /**
         * Diese Methode gibt zurück, ob ein gegebener Spieler den Herzcontainer sehen kann.
         */
        public static bool CanSeeHealthContainer(Player player)
        {
            return GetHealthContainerPosition(player) != null;
        }
        /**
         * Diese Methode gibt für einen gebenen Spieler die Position des Herzcontainers zurück. Ist die Sicht auf die Waffe möglich UND die Waffe ist im Sichtfeld, so wird die Position zurückgegeben, ansonsten wird NULL zurückgegeben.
         */
        public static Transform GetHealthContainerPosition(Player player)
        {
            Transform transform = null;
            GameObject health = GameObject.FindGameObjectWithTag("Health");

            if (health)
            {
                RaycastHit hit;
                var rayDirection = (health.transform.position - player.mGameObject.transform.position) + new Vector3(0, 0.2f, 0);

                if (Vector3.Angle(rayDirection, player.mGameObject.transform.forward) <= (mFieldOfViewInDegrees * 0.5f))
                {
                    Debug.DrawRay(player.mGameObject.transform.position, rayDirection, Color.blue);
                    if (Physics.Raycast(player.mGameObject.transform.position, rayDirection, out hit, Mathf.Infinity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.gameObject.tag.Equals("Health"))
                        {
                            transform = hit.transform;
                        }
                    }
                }
            }
            return transform;
        }

        /**
         * Diese Methode gibt die nächstgelegende Deckung zurück. Kann null returnen, falls keine Deckung in Sichtweite ist.
         */
        public static GameObject GetNearestCover(Player player)
        {
            GameObject nearestCover = null;

            var cols = Physics.OverlapSphere(player.mGameObject.transform.position, 10f);

            var dist = Mathf.Infinity;

            foreach (var col in cols)
            {
                var d = Vector3.Distance(player.mGameObject.transform.position, col.transform.position);
                var angle = Vector3.Angle(player.mGameObject.transform.position, col.transform.position);
                if ((d < dist) && (col.gameObject.tag == "Cover") && (angle <= mFieldOfViewInDegrees * 0.5f))
                {
                    dist = d;
                    nearestCover = col.gameObject;
                }
            }
            return nearestCover;
        }

        public static bool CheckIfTrulyCover(GameObject cover, Player lookingPlayer, Player enemy)
        {

            /*Vector3 coverPosition = cover.transform.position;
            Vector3 lookingPlayerPosition = lookingPlayer._gameObject.transform.position;
            Vector3 enemyPosition = enemy._gameObject.transform.position;

            float transformX = (lookingPlayerPosition.x + coverPosition.x) / 2.0f;
            float transformY = (lookingPlayerPosition.y + coverPosition.y) / 2.0f;
            float transformZ = (lookingPlayerPosition.z + coverPosition.z) / 2.0f;

            Vector3 transformPosition = new Vector3(transformX, transformY, transformZ);

            var rayDirection = transformPosition - enemyPosition;

            RaycastHit hit;

            if (Physics.Raycast(transformPosition, rayDirection, out hit))
            {
                if (!hit.transform.gameObject.name.Equals(enemy._name))
                {
                    Debug.Log("Hit: " + hit.transform.gameObject.name);
                    Debug.DrawRay(transformPosition, rayDirection, Color.white);
                    return true;
                }
            }

            Debug.DrawRay(lookingPlayerPosition, rayDirection, Color.cyan);

            return false;*/
            return true;
        }

        /**
         * Diese Methode gibt ein Player-Objekt anhand des übergebenen Namens zurück.
         */
        public static Player GetPlayerByName(string name)
        {
            Player foundPlayer = null;
            foreach (Player player in GameControllerScript.mPlayers)
            {
                if (player.mName.Equals(name))
                {
                    foundPlayer = player;
                    break;
                }
            }
            return foundPlayer;
        }
        /**
         * Diese Methode ermöglicht die Bewegung eines Spielers zu einem gegebenen Ziel und berücksichtigt dabei die Enddistanz zwischen Spieler und Ziel.
         */
        public static void MoveTo(Player movingPlayer, Vector3 destination, float stoppingDistance)
        {
            movingPlayer.mGameObject.GetComponent<NavMeshAgent>().destination = destination;
            //TODO: change back to speed of 6f
            movingPlayer.mGameObject.GetComponent<NavMeshAgent>().speed = 6f;
            movingPlayer.mGameObject.GetComponent<NavMeshAgent>().stoppingDistance = stoppingDistance;
            movingPlayer.mGameObject.GetComponent<NavMeshAgent>().autoRepath = true;
            movingPlayer.mGameObject.GetComponent<NavMeshAgent>().angularSpeed = 999f;
        }
        /**
         * Diese Methode analysiert die Umgebung respektive die Situation, den Status eines Spielers und gibt den so ermittelten Status zurück.
         */
        public static Status GetStatus(Player lookingPlayer, Player enemy, Status status)
        {
            status.isEnemyVisible = CanSeeEnemy(lookingPlayer);

            if (status.isEnemyVisible)
            {
                mRotationFinished = true;
                status.enemiesLastKnownPosition = enemy.mGameObject.transform.position;
                status.distanceToEnemy = (int)Status.GetEnemyDistanceFromFloatValue(GetDistanceBetweenGameObjects(lookingPlayer.mGameObject.transform.position, enemy.mGameObject.transform.position));
                status.isEnemyAlive = true;
            }
            else
            {
                status.distanceToEnemy = status.enemiesLastKnownPosition.x != 10000f ? (int)Status.GetEnemyDistanceFromFloatValue(GetDistanceBetweenGameObjects(lookingPlayer.mGameObject.transform.position,
                    status.enemiesLastKnownPosition)) : (int)Status.Distance.unknown;
            }

            status.lastPosition = (int)Status.GetLastPositionFromFloatValue(GetDistanceBetweenGameObjects(lookingPlayer.mGameObject.transform.position, status.enemiesLastKnownPosition));

            status.ownHealth = (int)Status.GetOwnHealthFromIntValue(lookingPlayer.mPlayerHealth);
            status.equippedWeapon = lookingPlayer.mEquippedWeapon.mName;

            status.currentAmmu = (int)Status.GetCurrentAmmunitionFromWeapon(lookingPlayer.mEquippedWeapon);
            status.currentOverallAmmu = (int)Status.GetCurrentOverallAmmunitionFromWeapon(lookingPlayer.mEquippedWeapon);


            status.isAmmunitionNeeded = NeedAmmunition(lookingPlayer);
            status.isWeaponNeeded = NeedAnotherWeapon(lookingPlayer);
            status.isHealthNeeded = NeedHealth(lookingPlayer);
            status.isCovered = lookingPlayer.mIsCovered;

            if (CanSeeAmmu(lookingPlayer))
            {
                status.ammunitionDistance = (int)Status.GetAmmunitionDistance(GetDistanceBetweenGameObjects(lookingPlayer.mGameObject.transform.position, GetAmmuPosition(lookingPlayer).position));
                status.ammuPosition = GetAmmuPosition(lookingPlayer).position;
            }
            else
            {
                status.ammunitionDistance = (int)Status.AmmunitionDistance.unknown;
            }
            if (CanSeeHealthContainer(lookingPlayer))
            {
                status.healthDistance = (int)Status.GetHealthDistance(GetDistanceBetweenGameObjects(lookingPlayer.mGameObject.transform.position, GetHealthContainerPosition(lookingPlayer).position));
                status.healthPosition = GetHealthContainerPosition(lookingPlayer).position;
            }
            else
            {
                status.healthDistance = (int)Status.HealthDistance.unknown;
            }

            if (CanSeeWeapon(lookingPlayer))
            {
                status.weaponDistance = (int)Status.GetWeaponDistance(GetDistanceBetweenGameObjects(lookingPlayer.mGameObject.transform.position, GetWeaponPosition(lookingPlayer).position));
                status.weaponPosition = GetWeaponPosition(lookingPlayer).position;
            }
            else
            {
                status.weaponDistance = (int)Status.WeaponDistance.unknown; ;
            }


            GameObject nearestCover = GetNearestCover(lookingPlayer);

            if (nearestCover != null)
            {
                if (CheckIfTrulyCover(nearestCover, lookingPlayer, enemy))
                {
                    status.isCoverNeeded = true;
                }
            }
            else
            {
                status.isCoverNeeded = false;
            }

            return status;

        }
        /**
         * Diese Methode gibt zurück, ob der Gegner des Spielers in der Schusslinie ist.
         */
        public static bool EnemyInShootingLine(Player lookingPlayer, Player enemy)
        {

            RaycastHit hit;
            var rayDirection = (enemy.mGameObject.transform.position - lookingPlayer.mGameObject.transform.position) + new Vector3(0, 1, 0);

            if (Vector3.Angle(rayDirection, lookingPlayer.mGameObject.transform.forward) <= (20 * 0.5f))
            {
                if (Physics.Raycast(lookingPlayer.mGameObject.transform.position, rayDirection, out hit))
                {
                    if (hit.transform.gameObject.name.Equals(enemy.mName))
                    {
                        return true;
                    }
                }
            }


            return false;
        }
        /**
         * Diese Methode gibt zurück, ob ein gegebener Spieler sein Ziel (Bewegung) erreicht hat.
         */
        public static bool DestinationReached(Player movingPlayer)
        {
            bool reached = false;
            NavMeshAgent agent = movingPlayer.mGameObject.GetComponent<NavMeshAgent>();
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        reached = true;
                    }
                }
            }

            return reached;
        }
        /**
         * Diese Methode ermöglicht das Zielen des Spielers. Diese Methode wird nur aufgerufen, wenn sich der Gegner bereits in der Schusslinie befindet. Ist dies der Fall, so wird direkt auf den Gegenspieler gezielt.
         */
        public static void LookAt(Player lookingPlayer, Player enemy)
        {
            lookingPlayer.mGameObject.transform.LookAt(enemy.mGameObject.transform.position + new Vector3(0, 1, 0));
        }
        /**
         * Diese Methode ermöglicht das Umsehen eines gegebenen Spielers, damit dieser seinen Gegner auch entdecken kann und nicht nur gerade in der Welt umherläuft.
         */
        public static void LookAround(Player lookingPlayer)
        {
            if (!mRotationFinished)
            {
                Vector3 rotation = lookingPlayer.mGameObject.transform.eulerAngles + 180f * Vector3.up;

                RotateTowards(lookingPlayer, rotation);
            }
        }
    }
}