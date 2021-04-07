using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Assets.Scripts.CMAS;
using Assets.Scripts.AI;
using Assets.Scripts.CBR.Plan;
using Assets.Scripts.CBR.Model;

namespace Assets.Scripts.Model
{
    /**
 * Delegate-Methode für den menschlichen Spieler, um zu ermitteln, welcher Spieler bei Tastendruck schießen soll.
 */
    public delegate void PlayerClick(object sender, EventArgs e);
    /**
     * Diese Klasse stellt eine Datenstruktur für einen Spieler zur Verfügung.
     */
    public class Player
    {
        
        public Vector3 botPlayerPosition;
        /**
         * Gibt an, ob der Spieler auf das CBR-System zugreift.
         */
        public bool mCBR { get; set; }
        /**
         * Gibt an, ob der Spieler vom Menschen gesteuert wird.
         */
        public bool mIsHumanControlled { get; set; }
        /**
         * Gibt an, ob der Spieler am Leben ist.
         */
        public bool mIsAlive { get; set; }
        /**
         * Gibt an, ob der Spieler in Deckung ist.
         */
        public bool mIsCovered { get; set; }
        /**
         * Der Name des Spielers.
         */
        public string mName { get; private set; }
        /**
         * Die aktuellen Lebenspunkte des Spielers.
         */
        public int mPlayerHealth { get; set; }
        /**
         * Identifier wird zur Zuweisung bei der Erstellung von Spielerobjekten benötigt (CBR-Spieler, Menschlicher Spieler, Non-CBR-Spieler, ...).
         */
        public int mIdentifier { get; set; }
        /**
         * Maximalen Lebenspunkte eines Spielers.
         */
        public static readonly int mMaxLife = 100;
        /**
         * Das GameObjekt des Spielers (grafisches 3D-Modell).
         */
        public GameObject mGameObject { get; set; }
        /**
         * Jeder Spieler verfügt über einen Agenten.
         */
        public PlayerAgent mPlayerAgent { get; private set; }
        /**
         * Die Statistiken des Spielers.
         */
        public Statistics mStatistics { get; set; }
        /**
         * Der aktuelle Status des Spielers.
         */
        public Status mStatus { get; set; }
        /**
         * Die aktuell ausgerüstete Waffe des Spielers.
         */
        public Weapon mEquippedWeapon { get; set; }
        /**
         * Referenz auf das Skript, was zum Schießen benötigt wird.
         */
        public PlayerShooting mPlayerShooting { get; set; }
        /**
         * Referenz auf das Skript, was die rudimentäre KI für den computergesteuerten Spieler implementiert.
         */
        public BotBehaviourScript mBotBehaviour { get; set; }
        /**
         * Referenz auf das Skript, was den Zugriff auf das CBR-System für einen Spieler ermöglicht.
         */
        public BotCBRBehaviourScript mCbrBehaviour { get; set; }
        /**
         * Kameraobjekt des Spielers, jeder Spieler verfügt über eine eigene Kamera.
         */
        public Camera mCamera { get; set; }
        /**
         * Der Plan des CBR-Spielers, der ausgeführt werden soll.
         */
        public Plan mPlan { get; set; }
        /**
         * Diese Liste enthält alle Waffen, die aktuell im Besitz des Spieler sind.
         */
        private List<Weapon> mWeapons;
        /**
         * Der NavMeshAgent wird für die Bewegung des Spielers benötigt.
         */
        public NavMeshAgent mNav { get; set; }
        /**
         * Delegate-Methode für den menschlichen Spieler, um zu ermitteln, welcher Spieler bei Tastendruck schießen soll.
         */
        public event PlayerClick OnPlayerClick;
        /**
         * Der einzige Konstruktor der Klasse, der den Namen des Spielers und ein 3D-Modell des Spielers benötigt. In diesem Konstruktor werden alle relevanten Einstellungen getätigt.
         */
        public Player(string name, GameObject gameObject)
        {
            Debug.Log("New Player created");
            mStatus = new Status();
            mStatistics = new Statistics();
            mName = name;
            mGameObject = gameObject;
            mWeapons = new List<Weapon>();
            mCamera = ((GameObject)Resources.Load("Prefabs/PlayerCamera")).GetComponent<Camera>();
            mGameObject.name = name;
            mPlayerAgent = new PlayerAgent(mName);
            mIsCovered = false;

            Init();
        }
        /**
         * Diese Methode fügt das Maschinengewehr dem Besitz des Spielers hinzu.
         */
        public void ActivateMachineGun()
        {
            mWeapons[1].mInPossess = true;
        }
        /**
         * Diese Methode fügt eine übergebene Waffe der Waffenliste des Spielers hinzu.
         */
        public void AddWeapon(Weapon weapon)
        {
            mWeapons.Add(weapon);
        }
        /**
         * Gibt die Anzahl der Waffen zurück.... NEEEDED???"?"OASJDKASJDK
         */
        public int GetWeaponCount()
        {
            return mWeapons.Count;
        }

        public Vector3 GetPlayerPosition()
        {
            botPlayerPosition = new Vector3(mGameObject.transform.position.x, mGameObject.transform.position.y, mGameObject.transform.position.z);
            return botPlayerPosition;
        }
        /**
         * Diese Methode aktiviert einen Spieler für den menschlichen Gebrauch.
         */
        public void ActivatePlayer()
        {
            mIsHumanControlled = true;
            InitGameObjectAndCamera();
        }
        /**
         * Diese Methode deaktiviert einen menschengesteuerten Spieler.
         */
        public void DeactivatePlayer()
        {
            mIsHumanControlled = false;
            InitGameObjectAndCamera();
        }
        /**
         * Diese Methode gibt die Waffenliste zurück.
         */
        public List<Weapon> GetWeapons()
        {
            return mWeapons;
        }
        /**
         * Diese Methode initialisiert einige wichtige Daten (ausgerüstete Waffe, etc.). Diese Methode wird beim Start und nach dem Tod eines Spielers aufgerufen.
         */
        public void Init()
        {
            mWeapons.Clear();
            mWeapons.Add(new Pistol(mGameObject));
            mEquippedWeapon = mWeapons[0];

            StaticMenueFunctions.FindComponentInChildWithTag<Transform>(mGameObject, "Machine Gun").gameObject.SetActive(false);
            StaticMenueFunctions.FindComponentInChildWithTag<Transform>(mGameObject, "Pistol").gameObject.SetActive(false);

            TriggerWeaponActivation();

            mPlayerHealth = mMaxLife;
            mIsAlive = true;
        }
        /**
         * Delegate-Methode.
         */
        public void OnClick()
        {
            OnPlayerClick(this, new EventArgs());
        }
        /**
         * Diese Methode sorgt für den Waffenwechsel des Spielers.
         */
        public void SwitchWeapon()
        {
            if (mWeapons.Count > 1)
            {
                if (mWeapons[0] == mEquippedWeapon)
                {
                    if (mWeapons[1].mInPossess && (mWeapons[1].mCurrentMagazineAmmu > 0 || mWeapons[1].mCurrentOverallAmmu > 0))
                    {
                        mEquippedWeapon = mWeapons[1];
                    }
                }
                else if (mWeapons[1] == mEquippedWeapon)
                {
                    if (mWeapons[0].mInPossess && (mWeapons[0].mCurrentMagazineAmmu > 0 || mWeapons[0].mCurrentOverallAmmu > 0))
                    {
                        mEquippedWeapon = mWeapons[0];
                    }
                }

                TriggerWeaponActivation();


                if (mCBR)
                {
                    for (int i = 0; i < mPlan.GetActionCount(); i++)
                    {
                        if (mPlan.actions[i].GetType() == typeof(SwitchWeapon))
                        {
                            mPlan.actions[i].finished = true;
                            break;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return mName + " has following stats: " + mStatistics.ToString();
        }
        /**
         * Diese Methode verringert die Lebenspunkte des Spielers um die gegebenen Schadenspunkte.
         */
        public void TakeDamage(int damage)
        {
            mPlayerHealth -= damage;
        }
        /**
         * Diese Methode ermöglicht das Schießen eines Spielers.
         */
        public void Shoot()
        {
            mPlayerShooting.Shoot();
        }
        /**
         * Diese Methode ermöglicht das Nachladen eines Spielers.
         */
        public void Reload()
        {
            if (mCBR)
            {
                for (int i = 0; i < mPlan.GetActionCount(); i++)
                {
                    if (mPlan.actions[i].GetType() == typeof(Reload))
                    {
                        mPlan.actions[i].finished = true;
                        break;
                    }
                }
            }
            mEquippedWeapon.Reload();
            BotBehaviourScript.ScriptBotPlan = "Reload";
        }
        /**
         * Diese Methode (de-)aktiviert nach einem Waffenwechsel die entsprechenden Waffenmodelle.
         */
        private void TriggerWeaponActivation()
        {
            foreach (Weapon weapon in mWeapons)
            {
                if (weapon != mEquippedWeapon)
                {
                    weapon.Deactivate();
                }
                else
                {
                    weapon.Activate();
                }
            }
        }
        /**
         * Diese Methode initialisiert das GameObjekt und die Kamera des Spielers.
         */
        public void InitGameObjectAndCamera()
        {

            if (!mGameObject.activeSelf)
            {
                mGameObject.SetActive(true);
            }
            if (!mCamera.gameObject.activeSelf)
            {
                mCamera.gameObject.SetActive(true);
            }

            Camera playerCamera = StaticMenueFunctions.FindComponentInChildren<Camera>(mGameObject);

            if (!mIsHumanControlled)
            {
                mGameObject.GetComponent<FPSWalkerEnhanced>().enabled = false;
                mGameObject.GetComponent<PlayerPerspective>().enabled = false;
                mGameObject.GetComponent<PlayerHealth>().enabled = false;
                mGameObject.GetComponent<CharacterController>().enabled = false;



                GameControllerScript.hudCanvas.SetActive(false);


                if (mGameObject.GetComponent<BotBehaviourScript>() != null)
                {
                    mGameObject.GetComponent<BotBehaviourScript>().enabled = true;
                }
                if (mGameObject.GetComponent<BotCBRBehaviourScript>() != null)
                {
                    mCBR = true;
                    mGameObject.GetComponent<BotCBRBehaviourScript>().enabled = true;
                }
                if (mGameObject.GetComponent<NavMeshAgent>() != null)
                {
                    mGameObject.GetComponent<NavMeshAgent>().enabled = true;
                }


                if (playerCamera != null)
                {
                    playerCamera.GetComponent<AudioListener>().enabled = false;
                    playerCamera.enabled = false;
                }
            }
            else
            {
                mGameObject.GetComponent<FPSWalkerEnhanced>().enabled = true;
                mGameObject.GetComponent<PlayerPerspective>().enabled = true;
                mGameObject.GetComponent<PlayerHealth>().enabled = true;
                mGameObject.GetComponent<CharacterController>().enabled = true;
                GameControllerScript.hudCanvas.SetActive(true);
                if (mGameObject.GetComponent<BotBehaviourScript>() != null)
                {
                    mGameObject.GetComponent<BotBehaviourScript>().enabled = false;
                }
                if (mGameObject.GetComponent<BotCBRBehaviourScript>() != null)
                {
                    mGameObject.GetComponent<BotCBRBehaviourScript>().enabled = false;
                    mCBR = false;
                }
                if (mGameObject.GetComponent<NavMeshAgent>() != null)
                {
                    mGameObject.GetComponent<NavMeshAgent>().enabled = false;
                }

                if (playerCamera != null)
                {
                    playerCamera.GetComponent<AudioListener>().enabled = true;
                    playerCamera.enabled = true;
                }
            }
        }
    }

}
