using Assets.Scripts.CBR.Model;
using Assets.Scripts.CBR.Plan;
using Assets.Scripts.Model;
using Assets.Scripts.Util;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.AI
{
    /**
     *
     * Diese Klasse stellt die KI für einen computergesteuerten Spieler, der auf das CBR-System zugreift, zur Verfügung.
     *
     */

    public class BotCBRBehaviourScript : MonoBehaviour
    {
        /*
         * Für eine KI, die mehrere Gegner berücksichtigen kann, muss hier eine Liste mit allen Gegnern verwendet werden.
        */

        /// <summary>
        /// Der Spieler mit CBR-System.
        /// </summary>
        private Player mPlayerWithCBR;
        /// <summary>
        /// Variable, welche die vergangene Zeit speichert. Es sollen nur alle x Sekunden Aktionen durchgeführt werden.
        /// </summary>
        public static bool mFirstTime = true;
        /// <summary>
        /// Wird aktuelle eine Anfrage verarbeitet?
        /// </summary>
        public static bool mIsRequesting = false;
        /// <summary>
        /// Variable, die angibt, nach welcher Zeitspanne frühestens eine neue Anfrage gestellt werden kann.
        /// </summary>
        private float mCbrInterval = 0.06f;
        /// <summary>
        /// Variable, die benötigt wird, um die Anzahl an Anfragen zu zählen.
        /// </summary>
        private int mCounter = 0;
        /// <summary>
        /// Der Gegner des Spielers.
        /// </summary>
        private Player mEnemy;

        /// <summary>
        /// Variable, welche die vergangene Zeit speichert. Es sollen nur alle x Sekunden Aktionen durchgeführt werden.
        /// </summary>
        private float mTimer = 0f;

        /// <summary>
        /// Startzeit zum Senden von Statistiken an den Path Viewer
        /// </summary>
        private int updateTime = 1;

        /// <summary>
        /// Diese Methode ordnet die vorhandenen Spieler korrekt zu.
        /// </summary>
        private void AssignPlayers()
        {
            Tuple<Player, Player> playerTuple = CommonUnityFunctions.GetBotPlayersCorrectly();
            mPlayerWithCBR = playerTuple.Item1;
            mEnemy = playerTuple.Item2;
        }

        /// <summary>
        /// Unity method
        /// </summary>
        private void Update()
        {
            // Wenn die Start-Zeit erreicht wurde:
            if (Time.time >= updateTime)
            {
                // Frequenz hochzählen
                updateTime = Mathf.FloorToInt(Time.time) + 1;
            }

            //Debug.Log(test);

            //string json = JsonParser<StatisticsForPathViewer>

            //Communication cTest = new Communication(json);
            //String cTestString = cTest.Body;
            //JsonParser<Response>.SerializeObject(
            //    new ConnectionToPathViewer.ConnectionToPathViewer().Send(
            //        JsonParser<Request>.DeserializeObject(cTestString)));

            mTimer += Time.deltaTime;
            if (mEnemy == null || mPlayerWithCBR == null)
            {
                AssignPlayers();
            }

            //C.W.: Sets the isMoving boolean of the agents animators controller according to the
            // current agents velocity. If the agent doesn't move the volicity equals to (0,0,0).
            // The IsWalking parameter will be used for animation controll as well as for shootingAccuracy.
            if (mPlayerWithCBR.mGameObject.GetComponent<NavMeshAgent>().velocity != Vector3.zero)
            {
                mPlayerWithCBR.mGameObject.GetComponent<Animator>().SetBool("IsWalking", true);
            }
            else
            {
                mPlayerWithCBR.mGameObject.GetComponent<Animator>().SetBool("IsWalking", false);
            }

            if (mPlayerWithCBR != null && mTimer >= mCbrInterval && Time.timeScale != 0)
            {
                if (mPlayerWithCBR.mEquippedWeapon.Equals("Pistol") && mPlayerWithCBR.GetWeaponCount() == 2)
                {
                    mPlayerWithCBR.SwitchWeapon();
                }

                mTimer = 0f;
                if (mFirstTime && mCounter++ == 0)
                {
                    mPlayerWithCBR.mStatus = CommonUnityFunctions.GetStatus(mPlayerWithCBR, mEnemy, mPlayerWithCBR.mStatus);
                    mPlayerWithCBR.mPlayerAgent.SendStringMessage(Constants.COMMUNICATION_AGENT_NAME, JsonParser<Request>.SerializeObject(new Request(new Situation(mPlayerWithCBR.mName, mPlayerWithCBR.mStatus))));
                    mIsRequesting = true;
                }

                if (mPlayerWithCBR.mPlan != null)
                {
                    // C.W.: logs the entire current plan of CBR Agent
                    string planString = "";
                    foreach (CBR.Plan.Action action in mPlayerWithCBR.mPlan.actions)
                    {
                        planString += action.name;
                    }
                    //Debug.Log("Here is my current CBR-PLAN : " + planString);

                    Status stat = new Status();
                    //Debug.Log("BotCBRBehaviourScript#Update#Hole neuen Status");
                    stat = CommonUnityFunctions.GetStatus(mPlayerWithCBR, mEnemy, stat);
                    //Debug.Log("BotCBRBehaviourScript#Update#Status geholt!");

                    if ((mPlayerWithCBR.mPlan.progress == (int)Plan.Progress.DONE || mPlayerWithCBR.mPlan.progress == (int)Plan.Progress.NOT_STARTED))
                    {
                        //Debug.Log("if ((mPlayerWithCBR.mPlan.progress == (int)Plan.Progress.DONE || mPlayerWithCBR.mPlan.progress == (int)Plan.Progress.NOT_STARTED))");
                        if (!mFirstTime)
                        {
                            //Debug.Log("BotCBRBehaviourScript#Update#Sende Nachricht an Kommunikationsagenten!");
                            mPlayerWithCBR.mPlayerAgent.SendStringMessage(Constants.COMMUNICATION_AGENT_NAME, JsonParser<Request>.SerializeObject(new Request(new Situation(mPlayerWithCBR.mName, mPlayerWithCBR.mStatus))));
                            mIsRequesting = true;
                            mCounter++;
                            //Debug.Log("BotCBRBehaviourScript#Update#Nachricht ist gesendet!");
                        }

                        // ???
                        while (mIsRequesting) ;

                        //Debug.Log("BotCBRBehaviourScript#Update#Führe Plan aus!");
                        CommonUnityFunctions.ExecutePlan(mPlayerWithCBR, mEnemy, mPlayerWithCBR.mStatus);
                    }
                    else if (mPlayerWithCBR.mPlan.progress == (int)Plan.Progress.IN_PROGRESS)
                    {
                        //Debug.Log("else if (mPlayerWithCBR.mPlan.progress == (int)Plan.Progress.IN_PROGRESS)");
                        if (mPlayerWithCBR.mStatus.Equals(stat)) // Situation hat sich nicht geändert - führe Plan weiterhin aus
                        {
                            CommonUnityFunctions.ExecutePlan(mPlayerWithCBR, mEnemy, mPlayerWithCBR.mStatus);
                        }
                        else
                        {
                            // Situation hat sich geändert - hole neuen Plan
                            mPlayerWithCBR.mStatus = stat;
                            mPlayerWithCBR.mPlan.progress = (int)Plan.Progress.DONE;
                        }

                        //Debug.Log("else if (mPlayerWithCBR.mPlan.progress == (int)Plan.Progress.IN_PROGRESS)");
                        // new situation leads to mark the current plan as done which forces a new plan
                        //if (!mPlayerWithCBR.mStatus.Equals(stat))
                        //{
                        //    mPlayerWithCBR.mStatus = stat;
                        //    mPlayerWithCBR.mPlan.progress = (int)Plan.Progress.DONE;
                        //}
                    }
                }
            }
        }
    }
}