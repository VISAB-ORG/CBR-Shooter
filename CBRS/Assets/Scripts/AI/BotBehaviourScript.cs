using System;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Scripts.Model;
using Assets.Scripts.CBR.Model;
using UnityEngine.AI;

namespace Assets.Scripts.AI
{

    /**
     * 
     * Diese Klasse stellt die triviale KI für einen computergesteuerten Spieler zur Verfügung.
     * 
     */
    public class BotBehaviourScript : MonoBehaviour
    {
        /*
         * Für eine KI, die mehrere Gegner berücksichtigen kann, muss hier eine Liste mit allen Gegnern verwendet werden.
         * /
        /**
         * Der Spieler ohne CBR-System.
         */
        private Player mPlayerWithoutCBR;
        /**
         * Der Gegner des Spielers.
         */
        private Player mEnemy;
        /**
         * Variable, welche die vergangene Zeit speichert. Es sollen nur alle x Sekunden Aktionen durchgeführt werden.
         */
        private float mTimer = 0f;
        /**
        * Plan des Scriptbot
        */
        public static String ScriptBotPlan { get; set; }
        /**
         * Timer, wie lange der Bot schon in der Mitte der Karte steht.
         */
        private float mCenterStandingTimer = 0f;
        /**
         * Timer, wie lange der Bot allgemein schon steht
         */
        private float mStandingTimer = 0f;
        /**
         * Der Bot darf maximal 3 Sekunden rumstehen.
         */
        private const float mMaximumgStandingTime = 3f;
        /**
         * Da der Bot nicht am exakten Nullpunkt (0, 0, 0) stehen kann, muss da eine gewisse Karenz gewährt werden. Also (+-5, 0, +-5) zälht auch als "Mitte".
         */
        private const float mMidDistance = 5f;

        /**
         * Ziel erreicht?
         */
        bool mDestinationReached = false;

        /**
         * 
         * Unity Methode
         */
        private void Update()
        {
            mTimer += Time.deltaTime;


            if (mPlayerWithoutCBR == null || mEnemy == null)
            {
                AssignPlayers();
            }

            //C.W.: Sets the isMoving boolean of the agents animators controller according to the
            // current agents velocity. If the agent doesn't move the volicity equals to (0,0,0).
            // The IsWalking parameter will be used for animation controll as well as for shootingAccuracy.
            if (mPlayerWithoutCBR.mGameObject.GetComponent<NavMeshAgent>().velocity != Vector3.zero)
            {
                mPlayerWithoutCBR.mGameObject.GetComponent<Animator>().SetBool("IsWalking", true);
            }
            else
            {
                mPlayerWithoutCBR.mGameObject.GetComponent<Animator>().SetBool("IsWalking", false);
            }
            
            if (mTimer >= 0.3f)
            {
                mTimer = 0f;
                if (mEnemy != null && mPlayerWithoutCBR != null)
                {
                    // Analysiere die Umgebung und führe dementsprechend Aktionen durch.
                    mPlayerWithoutCBR.mStatus = CommonUnityFunctions.GetStatus(mPlayerWithoutCBR, mEnemy, mPlayerWithoutCBR.mStatus);

                    bool collectWeapon = mPlayerWithoutCBR.mStatus.isWeaponNeeded && mPlayerWithoutCBR.mStatus.weaponPosition.x != 10000f && mPlayerWithoutCBR.mStatus.weaponDistance <= (int)Status.WeaponDistance.far;
                    bool collectHealth = mPlayerWithoutCBR.mStatus.isHealthNeeded && mPlayerWithoutCBR.mStatus.healthPosition.x != 10000f && mPlayerWithoutCBR.mStatus.healthDistance <= (int)Status.HealthDistance.far;
                    bool collectAmmunition = mPlayerWithoutCBR.mStatus.isAmmunitionNeeded && mPlayerWithoutCBR.mStatus.ammuPosition.x != 10000f && mPlayerWithoutCBR.mStatus.ammunitionDistance <= (int)Status.AmmunitionDistance.far;

                    if (mPlayerWithoutCBR.mStatus.isEnemyVisible)
                    {
                        if ((mPlayerWithoutCBR.GetWeaponCount() == 2 && !mPlayerWithoutCBR.mEquippedWeapon.mName.Equals("Machine Gun")) || (mPlayerWithoutCBR.GetWeaponCount() == 2 && mPlayerWithoutCBR.mEquippedWeapon.IsWeaponEmpty()))
                        {
                            //Setzt den Plan auf "SwitchWeapon" wenn die Waffe gewechselt wird.
                            mPlayerWithoutCBR.SwitchWeapon();
                            ScriptBotPlan = "SwitchWeapon";

                        }

                        if (mPlayerWithoutCBR.mStatus.distanceToEnemy == (int)Status.Distance.middle && CommonUnityFunctions.EnemyInShootingLine(mPlayerWithoutCBR, mEnemy))
                        {
                            CommonUnityFunctions.LookAt(mPlayerWithoutCBR, mEnemy);

                            //Setzt den Plan auf "Shoot" wenn die Waffe abgefeuert  wird.
                            mPlayerWithoutCBR.Shoot();
                            ScriptBotPlan = "Shoot";
                        }
                        else if (mPlayerWithoutCBR.mStatus.distanceToEnemy <= (int)Status.Distance.near)
                        {
                            CommonUnityFunctions.LookAt(mPlayerWithoutCBR, mEnemy);
                            mPlayerWithoutCBR.Shoot();
                            //Setzt den Plan auf "Shoot" wenn die Waffe abgefeuert  wird.
                            ScriptBotPlan = "Shoot";
                        }
                        else
                        {
                            mDestinationReached = CommonUnityFunctions.DestinationReached(mPlayerWithoutCBR);

                            if (CommonUnityFunctions.EnemyInShootingLine(mPlayerWithoutCBR, mEnemy))
                            {
                                CommonUnityFunctions.mRotationFinished = false;
                                CommonUnityFunctions.RotateTowards(mPlayerWithoutCBR, mEnemy.mGameObject.transform.position);
                            }
                            else if (CommonUnityFunctions.DestinationReached(mPlayerWithoutCBR))
                            {
                                CommonUnityFunctions.mRotationFinished = true;
                            }

                            Transform enemyPosition = CommonUnityFunctions.GetEnemyPosition(mPlayerWithoutCBR);
                            CommonUnityFunctions.MoveTo(mPlayerWithoutCBR, enemyPosition == null ? mEnemy.mGameObject.transform.position : enemyPosition.position, CommonUnityFunctions.NORMAL_STOPPING_DISTANCE);
                            //Setzt den Plan auf "MoveToEnemy" wenn der ScriptBot sich zum Gegner bewegt.
                            ScriptBotPlan = "MoveToEnemy";
                        }
                    }
                    else if (mPlayerWithoutCBR.mStatus.enemiesLastKnownPosition.x != 10000f && !mDestinationReached)
                    {
                        CommonUnityFunctions.mRotationFinished = false;
                        CommonUnityFunctions.MoveTo(mPlayerWithoutCBR, mPlayerWithoutCBR.mStatus.enemiesLastKnownPosition, CommonUnityFunctions.NORMAL_STOPPING_DISTANCE);
                        mDestinationReached = CommonUnityFunctions.DestinationReached(mPlayerWithoutCBR);

                        CommonUnityFunctions.LookAround(mPlayerWithoutCBR);
                        //Setzt den Plan auf "Seek" wenn der ScriptBot den Gegner sucht.
                        ScriptBotPlan = "Seek";
                    }
                    else
                    {
                        if (collectAmmunition)
                        {
                            CommonUnityFunctions.mRotationFinished = false;
                            CommonUnityFunctions.MoveTo(mPlayerWithoutCBR, mPlayerWithoutCBR.mStatus.ammuPosition, 0);
                            //Setzt den Plan auf "CollectItem" wenn ein Gegenstand aufgesammelt werden soll.
                            ScriptBotPlan = "CollectItem";
                        }
                        else if (collectHealth)
                        {
                            CommonUnityFunctions.mRotationFinished = false;
                            CommonUnityFunctions.MoveTo(mPlayerWithoutCBR, mPlayerWithoutCBR.mStatus.healthPosition, 0);
                            //Setzt den Plan auf "CollectItem" wenn ein Gegenstand aufgesammelt werden soll.
                            ScriptBotPlan = "CollectItem";
                        }
                        else if (collectWeapon)
                        {
                            CommonUnityFunctions.mRotationFinished = false;
                            CommonUnityFunctions.MoveTo(mPlayerWithoutCBR, mPlayerWithoutCBR.mStatus.weaponPosition, 0);
                            //Setzt den Plan auf "CollectItem" wenn ein Gegenstand aufgesammelt werden soll.
                            ScriptBotPlan = "CollectItem";
                        }
                        else
                        {
                            // C.W.: Checks if player stands around the middle of the map. mCenterVector bases on a GameObject set in the unity scene.
                            bool isInCenter = (mPlayerWithoutCBR.mGameObject.transform.position.x <= GameControllerScript.mCenterVector.x + mMidDistance 
                                && mPlayerWithoutCBR.mGameObject.transform.position.x >= - ( GameControllerScript.mCenterVector.x - mMidDistance)) 
                                && (mPlayerWithoutCBR.mGameObject.transform.position.z <= (GameControllerScript.mCenterVector.z + mMidDistance) 
                                && mPlayerWithoutCBR.mGameObject.transform.position.z >= (GameControllerScript.mCenterVector.z - mMidDistance));

                            // C.W.: Checks if player stands around the fallback position of the map. mFallbackVector bases on a GameObject set in the unity scene.
                            bool isInOtherPosition = (mPlayerWithoutCBR.mGameObject.transform.position.x <= (GameControllerScript.mFallbackVector.x + mMidDistance)
                                && mPlayerWithoutCBR.mGameObject.transform.position.x >= GameControllerScript.mFallbackVector.x - mMidDistance) 
                                && (mPlayerWithoutCBR.mGameObject.transform.position.z <= (GameControllerScript.mFallbackVector.z + mMidDistance) 
                                && mPlayerWithoutCBR.mGameObject.transform.position.z >= (GameControllerScript.mFallbackVector.z - mMidDistance));

                            if (isInCenter)
                            {
                                mCenterStandingTimer += Time.deltaTime * 15;
                            }
                            if (isInOtherPosition)
                            {
                                mStandingTimer += Time.deltaTime * 15;
                            }


                            if (mCenterStandingTimer <= mMaximumgStandingTime || mStandingTimer > mMaximumgStandingTime)
                            {
                                // C.W.: Moves the trivial agent to the middle of the map
                                mStandingTimer = 0f;
                                CommonUnityFunctions.mRotationFinished = false;
                                CommonUnityFunctions.MoveTo(mPlayerWithoutCBR, GameControllerScript.mCenterVector, 0);
                                CommonUnityFunctions.LookAround(mPlayerWithoutCBR);
                                //Setzt den Plan auf "Seek" wenn der ScriptBot den Gegner sucht.
                                ScriptBotPlan = "Seek";
                            }
                            else if (mStandingTimer <= mMaximumgStandingTime || mCenterStandingTimer > mMaximumgStandingTime)
                            {
                                // C.W.: Moves the trivial agent to the fallback position of the map
                                CommonUnityFunctions.mRotationFinished = false;
                                CommonUnityFunctions.MoveTo(mPlayerWithoutCBR, GameControllerScript.mFallbackVector, 0);
                                CommonUnityFunctions.LookAround(mPlayerWithoutCBR);
                                //Setzt den Plan auf "Seek" wenn der ScriptBot den Gegner sucht.
                                ScriptBotPlan = "Seek";

                                if (CommonUnityFunctions.DestinationReached(mPlayerWithoutCBR))
                                {
                                    mCenterStandingTimer = 0f;
                                }
                            }
                        }
                    }
                }
            }
        }

        /**
         * Diese Methode ordnet die vorhandenen Spieler korrekt zu.
         */
        private void AssignPlayers()
        {
            Tuple<Player, Player> playerTuple = CommonUnityFunctions.GetBotPlayersCorrectly();
            mEnemy = playerTuple.Item1;
            mPlayerWithoutCBR = playerTuple.Item2;
        }



    }
}