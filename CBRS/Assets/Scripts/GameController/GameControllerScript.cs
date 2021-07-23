using Assets.Scripts.AI;
using Assets.Scripts.CMAS;
using Assets.Scripts.Model;
using Assets.Scripts.Util;
using Assets.Scripts.VISAB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using VISABConnector;
using VISABConnector.Unity;

/**
 * Dieses Skript stellt den zentralen Bezugspunkt des Programmes dar, an dem alle relevanten Daten gespeichert sind.
 */

public class GameControllerScript : MonoBehaviour
{
    /// <summary>
    /// Represents the current state of the game Might rename to GameState
    /// </summary>
    public static GameInformation GameInformation { get; } = new GameInformation();

    #region fields

    /**
     * C.W.: Reference to an empty gameObject identifys the prefered camping position
     * and respective aiming positions.
     * Used in Camping behavior of CBR agent.
     */
    public GameObject mCampingPosition;
    /**
     * C.W.: Used to provide the camping position and aiming vector as static list object.
     */
    public static List<Transform> mCampingPositionTransforms;
    /**
     * C.W.: Reference to an empty gameObject identifys the central position in the szene.
     * Used by the trivial bot behavior.
     */
    public GameObject mCenterPosition;
    /**
     * C.W.: Reference to an empty gameObject identifys the fallback position in the szene.
     * Used by the trivial bot behavior.
     */
    public GameObject mFallbackPosition;
    /**
     * C.W.: supporting vector to provide the central position of the map as static attribute.
     * Used by the trivial bot behavior.
     */
    public static Vector3 mCenterVector;
    /**
     * C.W.: supporting vector to provide the fallback position of the map as static attribute.
     * Used by the trivial bot behavior.
     */
    public static Vector3 mFallbackVector;
    /**
     * C.W.: Reference to the CBR player in the game.
     */
    private Player mCBRPlayer;
    /*
     * C.W.: SupportVariable which is used to implement the round based setup.
     * Indicates the current RoundTimer.
     */
    private static float mRoundTimer;
    /**
     * C.W.: Indicates the material of the CBR agent.
     * Used to allow a visual distinction between both players
     */
    public Material mCBRMaterial;
    /**
     * C.W.: Integer which indicates the round duration time in seconds before a round ends.
     * A round will only be ended automatically if no player frags the other one.
     * The player with more health points gets a pseudoFrag.
     */
    public static float mRoundDuration = 90f;
    /**
     * AgentController, der das Multiagentensystem startet und verwaltet.
     */
    public static AgentController mAgentController;
    /**
     * Liste, in der alle verfügbaren Namen für die Spieler gespeichert werden.
     */
    private List<string> mPlayerNames;
    /**
     * Referenz auf das GameMenueScript, welches sich um den pausierten Zustand des Spiels kümmert.
     */
    public GameMenueScript mGameMenueScript;
    /**
     * Referenz auf alle verfügbaren PlayerSpawnpoints.
     */
    public GameObject mSpawnPointObject;
    /**
     * Referenz auf alle verfügbaren WeaponSpawnpoints.
     */
    public GameObject mWeaponSpawnPoint;
    /**
     * Referenz auf alle verfügbaren HealthSpawnpoints.
     */
    public GameObject mHealthSpawnPoint;
    /**
     * Referenz auf alle verfügbaren AmmunitionSpawnpoints.
     */
    public GameObject mPickUpsSpawnPoints;
    /**
     * Boolean, ob gerade die Waffe gewechselt wird.
     */
    private bool mIsSwitching = false;
    /**
     * Variable, die den Timestamp beim Spielstart darstellt.
     */

    public static DateTime mGameStart { get; private set; }
    /**
     * Wird zur Identifizierung des CBR-Players verwendet.
     */
    private static int mSpawnCounter = 0;

    /**
    * Wird zur Identifizierung des CBR-Players verwendet.
    */
    public static int roundCounter = 1;
    /**
     * Dieser Wert gibt an, wie viele Spieler am Spiel teilnehmen sollen. Standardwert: 2.
     * Theoretisch auch 1000 oder mehr Spieler möglich, allerdings ist der Prototyp dafür nicht ausgelegt und es gibt nicht genug Namen für 1000 Spieler.
     */
    public int NumberOfPlayers = 2;
    /**
     * Enum der den Status des Spiels darstellt (Laufend, Pausiert).
     */

    public enum GameState
    {
        PAUSED, RUNNING
    }

    /**
     * Dieser Wert gibt an, wie viele Sekunden vergehen, bis ein toter Spieler reanimiert wird (Standard: 3).
     */

    public static float mRespawnTime { get; private set; }
    /**
     * Status des Spiels.
     */
    public GameState mState;
    /**
     * GameObject des Spielers (Prefab).
     */
    private GameObject mPlayerGameObject;
    /**
     * GameObject der Spielerkamera (Prefab).
     */
    private GameObject mCameraObject;
    /**
     * Liste, die alle Spawnpoints der Munitionskiste beinhaltet.
     */

    public static List<Transform> mWeaponsCrateSpawnPoints { get; private set; }
    /**
     * Liste, die alle Spawnpoints des Herzcontainers beinhaltet.
     */

    public static List<Transform> mHealthSpawnPoints { get; private set; }
    /**
     * Liste, die alle Spawnpoints der Spieler beinhaltet.
     */

    public static List<Transform> mSpawnPoints { get; private set; }
    /**
     * Liste, die alle Spawnpoints der zweiten Waffe beinhaltet.
     */

    public static List<Transform> mM4a1SpawnPoints { get; private set; }
    /**
     * Liste, die alle Spieler beinhaltet.
     */

    public static List<Player> mPlayers { get; private set; }

    /**
     * Vector3 mit Koordinaten für den Spawnpunkt Herzcontainer.
     */

    public static Vector3 healthPositionRaw { get; set; }

    /**
    * Vector3 mit Koordinaten für den Spawnpunkt Ammucontainer.
    */

    public static Vector3 ammuPositionRaw { get; set; }

    /**
    * Vector3 mit Koordinaten für den Spawnpunkt Waffe.
    */

    public static Vector3 weaponPositionRaw { get; set; }
    /**
     * Variable, welche die Information hält, wie viele Spieler am Spiel teilnehmen.
     */
    private int mPlayerCounter = 0;
    /**
     * Die Zuschauerkamera als Variable.
     */
    private Camera mSpectatorCamera;
    /**
     * GameObject der Zuschauerkamera (Prefab)
     */
    private GameObject mSpectatorCameraGameObject;
    /**
     * GameObject der Munitionskiste (Prefab)
     */
    private GameObject mWeaponCrateGameObject;
    /**
     * Der vom Menschen gesteuerte Spieler.
     */
    private Player mHumanControlled;
    /**
     * Die Reichweite der Waffen.
     */

    public float mRange { get; set; }
    /**
     * Instanziierung des Prefabs der Munitionskiste.
     */
    private GameObject mWeaponsCrateObject;
    /**
     * Prefab Objekt der zweiten Waffe.
     */
    private GameObject mM4a1GameObject;
    /**
     * Prefab Objekt der zweiten Waffe.
     */
    private GameObject mHeartGameObject;
    /**
     * Gibt an, ob die M4A1 eingesammelt ist (wichtig für Respawn).
     */
    public static bool mM4a1Collected = true;
    /**
     * Gibt an, ob die Munitionskiste eingesammelt ist (wichtig für Respawn).
     */
    public static bool mWeaponCrateCollected = true;
    /**
     * Gibt an, ob der Herzcontainer eingesammelt ist (wichtig für Respawn).
     */
    public static bool mHeartCollected = true;
    /**
     * Der menschliche Spieler verfügt über ein HUD.
     */

    public static GameObject hudCanvas { get; private set; }
    /**
     * Zeit in Sekunden, bis eine neue Waffe spawnt.
     */
    private float mWeaponSpawnTimer = 30f;
    /**
     * Zeit in Sekunden, bis eine neue Munitionskiste spawnt.
     */
    private float mWeaponCrateTimer = 20f;
    /**
     * Zeit in Sekunden, bis ein neuer Herzcontainer spawnt.
     */
    private float mHeartTimer = 20f;
    /**
     * Dauer in Sekunden, die für den Nachladevorgang benötigt werden.
     */
    private float mReloadTimer = 1f;
    /**
     * Unity Methode, die beim Aufruf des Skripts *einmalig* ausgeführt wird.
     */

    #endregion fields

    private void Awake()
    {
        // C.W.: Initialize the round timer with duration on every start of a new round
        mRoundTimer = mRoundDuration;

        Constants.CreateFolderIfDoesNotExistYet(Constants.SAVES_PATH);
        InitAgentsAndCBR();
        MainMenueScript.OnlyBots = true;

        hudCanvas = GameObject.FindGameObjectWithTag("HUD");

        mGameStart = DateTime.Now;

        Debug.Log("Game start at: " + StaticMenueFunctions.GetTimeStampString(mGameStart));

        mRespawnTime = 3f;

        mPlayerNames = new List<string>
        {
            "John Doe",
            "Jane Doe",
            "Chuck Norris"
        };

        mPlayerGameObject = Resources.Load("Prefabs/Player") as GameObject;
        mSpectatorCameraGameObject = Resources.Load("Prefabs/SpectatorCamera") as GameObject;
        mWeaponsCrateObject = Resources.Load("Prefabs/WeaponsCrate/WeaponsCrate") as GameObject;
        mM4a1GameObject = Resources.Load("Prefabs/M4A1_Collectable") as GameObject;

        mM4a1SpawnPoints = new List<Transform>();

        for (int i = 0; i < mWeaponSpawnPoint.transform.childCount; i++)
        {
            mM4a1SpawnPoints.Add(mWeaponSpawnPoint.transform.GetChild(i));
        }

        mSpectatorCamera = mSpectatorCameraGameObject.GetComponent<Camera>();

        mSpawnPoints = new List<Transform>();

        for (int i = 0; i < mSpawnPointObject.transform.childCount; ++i)
        {
            mSpawnPoints.Add(mSpawnPointObject.transform.GetChild(i));
        }

        mHealthSpawnPoints = new List<Transform>();

        for (int i = 0; i < mHealthSpawnPoint.transform.childCount; i++)
        {
            mHealthSpawnPoints.Add(mHealthSpawnPoint.transform.GetChild(i));
        }

        mWeaponsCrateSpawnPoints = new List<Transform>();

        for (int i = 0; i < mPickUpsSpawnPoints.transform.childCount; ++i)
        {
            mWeaponsCrateSpawnPoints.Add(mPickUpsSpawnPoints.transform.GetChild(i));
        }



        // C.W.: Spawning and Creating the players
        mPlayers = new List<Player>();
        Cursor.lockState = CursorLockMode.Locked;
        mState = GameState.RUNNING;
        mGameMenueScript.SetGameMenueToDefaultSettings();

        for (int i = 0; i < NumberOfPlayers; ++i)
        {
            // C.W.: changed from loading the prefab iteself to a new intance of it.
            //The instantiaten is now already done in previous steps and obsolete at this point.
            mPlayers.Add(new Player(mPlayerNames[i], mPlayerGameObject));
            //mPlayers.Add(new Player(mPlayerNames[i], Instantiate<GameObject>(mPlayerGameObject)));
        }

        if (mSpectatorCamera != null && MainMenueScript.OnlyBots)
        {
            EnableSpectatorCamera();
        }

        // C.W.: first initiate of the players / agents
        InitiatePlayers();

        // C.W.: assign the CBR player membervariable for further use
        AssignCBRPlayer();

        mRange = 100f;

        //C.W.: provides a static public accessible reference to the center and fallback vectors of the map
        mCenterVector = new Vector3(mCenterPosition.transform.position.x, mCenterPosition.transform.position.y, mCenterPosition.transform.position.z);
        mFallbackVector = new Vector3(mFallbackPosition.transform.position.x, mFallbackPosition.transform.position.y, mFallbackPosition.transform.position.z);

        //C.W.: Fill CampingPosition list with set items from unity scene.
        mCampingPositionTransforms = new List<Transform>();
        for (int i = 0; i < mCampingPosition.transform.childCount; ++i)
        {
            mCampingPositionTransforms.Add(mCampingPosition.transform.GetChild(i));
        }

        
    }

    public void Start()
    {
        // Initially set the game information
        var players = CommonUnityFunctions.GetBotPlayersCorrectly();
        GameInformation.Players.Add(players.Item1);
        GameInformation.Players.Add(players.Item2);
        if (Time.timeScale != SpeedController.Speed)
            Time.timeScale = SpeedController.Speed;
        GameInformation.Speed = Time.timeScale;

        GameInformation.MapRectangle = VISABHelper.GetMapRectangle();

        UpdateGameInformation();

        //Start VISAB api transmission
        LoopBasedSession.MessageAddedEvent += Debug.Log;

        var meta = VISABHelper.GetMetaInformation();
        var success = LoopBasedSession.StartSessionAsync(meta, VISABHelper.HostAdress, VISABHelper.Port, VISABHelper.RequestTimeout).Result;
        if (success)
        {
            VisabLoopCTS = new CancellationTokenSource();
            var delay = Mathf.FloorToInt((1000 / VISABHelper.SendPerSecond) / GameInformation.Speed);
            LoopBasedSession.StartStatisticsLoopAsync(VISABHelper.GetCurrentStatistics, () => GameInformation?.GameState == GameState.RUNNING, delay, VisabLoopCTS.Token, queryFile: true);
        }

        var instantConfig = new InstantiationConfiguration
        {
            SpawnLocation = GameObject.Find("SnapSpawn").transform.position,
            PrefabPath = "Prefabs/WeaponsCrate/WeaponsCrate",
        };

        var settings2 = new SnapshotConfiguration
        {
            ImageHeight = 1024,
            ImageWidth = 1024,
            CameraOffset = 2f,
            CameraRotation = new Vector3(0, 0, 45),
            Orthographic = false,
            InstantiationSettings = instantConfig
        };

        var settings = new SnapshotConfiguration
        {
            GameObjectId = "Environment",
            ImageHeight = 1024,
            ImageWidth = 1024,
            CameraOffset = 2f,
            CameraRotation = new Vector3(0, 0, 90),
            Orthographic = true
        };

        var bytes = ImageCreator.TakeSnapshot(settings);
        var name = SnapshotName(settings.ImageWidth, settings.ImageHeight);

        File.WriteAllBytes(name, bytes);
        //StartCoroutine(ImageExtractionRoutine());

        var images = VISABHelper.MakeSnapshots();
        LoopBasedSession.SendImagesAsync(images).Wait();
    }

    public static string SnapshotName(int width, int height)
    {
        return string.Format("{0}/Snapshots/minimap_{1}x{2}_{3}.png", Application.dataPath, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    IEnumerator ImageExtractionRoutine()
    {
        yield return new WaitForSeconds(2);

        var instantConfig = new InstantiationConfiguration
        {
            SpawnLocation = GameObject.Find("SnapSpawn").transform.position,
            PrefabPath = "Prefabs/WeaponsCrate/WeaponsCrate",
        };

        var settings2 = new SnapshotConfiguration
        {
            ImageHeight = 1024,
            ImageWidth = 1024,
            CameraOffset = 2f,
            CameraRotation = new Vector3(90, 0, 0),
            Orthographic = false,
            InstantiationSettings = instantConfig
        };

        var settings = new SnapshotConfiguration
        {
            GameObjectId = "Environment",
            ImageHeight = 1024,
            ImageWidth = 1024,
            CameraOffset = 2f,
            CameraRotation = new Vector3(0, 0, 90),
            Orthographic = false
        };

        var bytes = ImageCreator.TakeSnapshot(settings);
        var name = SnapshotName(settings.ImageWidth, settings.ImageHeight);

        File.WriteAllBytes(name, bytes);
    }

    private void UpdateGameInformation()
    {
        GameInformation.RoundTime = mRoundDuration - mRoundTimer;
        GameInformation.AmmunitionPosition = ammuPositionRaw;
        GameInformation.GameState = mState;
        GameInformation.HealthPosition = healthPositionRaw;
        GameInformation.RoundCounter = roundCounter;
        GameInformation.WeaponPosition = weaponPositionRaw;
        GameInformation.TotalTime += Time.deltaTime;
    }

    /**
     * Methode, die zu Beginn des Skripts ausgeführt wird und das Java-Programm, sowie den Agentencontroller inklusive der benötigten Agenten startet.
     */

    private void InitAgentsAndCBR()
    {
        bool withWindow = true;
        Constants.StartServer(withWindow);
        mAgentController = new AgentController();
        mAgentController.StartAgentPortal();
    }

    #region VISAB variables

    /// <summary>
    /// Cancellation Token to cancel the loop of sending data to VISAB
    /// </summary>
    public static CancellationTokenSource VisabLoopCTS { get; private set; }

    /// <summary>
    /// The VISABStatistics object, holding the information that will be sent to VISAB
    /// </summary>
    public static VISABStatistics VisabStatistics { get; private set; }

    #endregion VISAB variables

    /*
     * C.W.: Method to end a round. Is used when the roundtime is up.
    * The method decides which player won the round by checking who has more healthpoints.
    * In case of equal health points of the players, the round ends with a draw.
    */

    private void EndRoundByTime()
    {
        // C.W.: indicates the player with most health points
        int winningPlayer = 0;

        // C.W.: indicates the max value of healthPoints of a player
        int maxHealthPoints = mPlayers[0].mPlayerHealth;

        // C.W.: checking for the max player health and gets the index of the respective player
        for (int i = 0; i < mPlayers.Count; i++)
        {
            if (mPlayers[i].mPlayerHealth > maxHealthPoints)
            {
                winningPlayer = i;
                maxHealthPoints = mPlayers[i].mPlayerHealth;
            }
        }

        // C.W.: this array contains the indizes of the players which have the same max amount of
        // health points
        List<int> drawnPlayers = new List<int>();
        int x = 0;

        // C.W.: double check if more than one player has the same max amount of health points which
        // would lead to a shared frag or a draw
        for (int i = 0; i < mPlayers.Count; i++)
        {
            if (mPlayers[i].mPlayerHealth == maxHealthPoints)
            {
                x++;
                drawnPlayers.Add(i);
            }
        }

        if (drawnPlayers.Count > 1)
        {
            // C.W.: We have a draw between the players contained in drawnPlayers
            // TODO: needs to be processed in further work right now the game is only optimized for
            // 2 players and shall do nothing in case of a draw.

            Debug.Log("Round ends with a draw. All have " + maxHealthPoints + " HP");
        }
        else
        {
            //Debug.Log(mPlayers[winningPlayer].mName + " won with " + mPlayers[winningPlayer].mPlayerHealth + " HP");

            //TODO: needs to indentify loosing player index and write death into csv
            //Constants.SaveDeath(loosingPlayer, GameControllerScript.mGameStart);
            //mPlayers[losingPlayer].mStatistics.AddDeath(new Death);

            // C.W.: writes the win as a frag in the csv file
            mPlayers[winningPlayer].mStatistics.AddFrag(new Frag());
            Constants.SaveFrag(mPlayers[winningPlayer], GameControllerScript.mGameStart);

            // C.W.: give the mPlayers[winningPlayer] a frag in CBR statistic
            if (mPlayers[winningPlayer].mCBR)
            {
                ScoreBoardManager.increaseFrags();
            }
            else
            {
                //C.W.: TODO: needs to be removed when will be optimized for n players.
                // line assumes that only one other player excepting the CBR player is there
                // increments death because CBR loses the round
                ScoreBoardManager.increaseDeaths();
            }
        }
        Debug.Log("-------Round Ended by Time -----------");
    }

    /*
     * C.W.:Reorganized Method to initiate the players and spawns them first time on the map
     */

    private void InitiatePlayers()
    {
        foreach (Player player in mPlayers)
        {
            SpawnPlayer(player);
            player.OnPlayerClick += Clicked;
            mAgentController.AddAgent(player.mPlayerAgent);
            Constants.InitSaveFolderAndPlayerSaveFile(player, mGameStart);
        }
    }

    /*
     * C.W.: Method which starts a new Round and resets respective player values.
     * TODO: Needs to be optimized for more than just these two static players named "John" and "Jane"
    */

    public static void StartNewRound()
    {
        // C.W.: initialize the round timer with set RoundDuration on every start of a new round
        mRoundTimer = mRoundDuration;
        // Counts the rounds
        roundCounter++;
        Debug.Log("Rundeanzahl ist:  " + roundCounter);

        // C.W.: reset respective player
        for (int i = 0; i < mPlayers.Count; i++)
        {
            Vector3 spawnPoint;

            // C.W.: changed to fixed SpawnPoints for both players
            // TODO: currently not optimized for more than 2 players
            if (mPlayers[i].mName == "John Doe")
            {
                spawnPoint = mSpawnPoints[2].position;
            }
            else
            {
                spawnPoint = mSpawnPoints[4].position;
            }

            // C.W.: Gets the nearest secure position on the nav mesh for the spawnPoint
            NavMeshHit secureSpawnPoint;
            Vector3 spawnPointResult;
            if (NavMesh.SamplePosition(spawnPoint, out secureSpawnPoint, 5f, NavMesh.AllAreas))
            {
                spawnPointResult = secureSpawnPoint.position;
            }
            else
            {
                spawnPointResult = spawnPoint;
            }

            mPlayers[i].mGameObject.GetComponent<NavMeshAgent>().Warp(spawnPointResult);
            mPlayers[i].mGameObject.transform.rotation = Quaternion.identity;
            mPlayers[i].Init();
            mPlayers[i].mGameObject.SetActive(true);
        }
    }

    /**
     * Methode, die einen übergebenen Spieler in der Spielwelt starten lässt.
     */

    private void SpawnPlayer(Player player)
    {
        Transform spawnPoint = null;

        bool free = false;

        float timer = 0f;
        while (!free)
        {
            timer += Time.deltaTime;

            // changed to fixed SpawnPoints
            // TODO: currently not optimized for more than 2 players
            if (player.mName == "John Doe")
            {
                spawnPoint = mSpawnPoints[2];
            }
            else
            {
                spawnPoint = mSpawnPoints[4];
            }

            // C.W.: random selection of a spawn point is disabled spawnPoint =
            // mSpawnPoints[UnityEngine.Random.Range(0, mSpawnPoints.Count)];
            Vector3 spawnVector = spawnPoint.position;

            var hitColliders = Physics.OverlapSphere(spawnVector, 2);

            int pCounter = 0;

            foreach (Collider collider in hitColliders)
            {
                if (collider.tag == "Player")
                {
                    pCounter++;
                }
            }

            if (pCounter <= 0)
            {
                free = true;
            }
        }

        if (MainMenueScript.OnlyBots || !MainMenueScript.OnlyBots && mPlayerCounter == 1)
        {
            player.mIsHumanControlled = false;
        }
        else
        {
            player.mIsHumanControlled = true;
            mPlayerCounter++;
        }

        // C.W.: Gets the nearest secure position on the nav mesh for the spawnPoint
        NavMeshHit secureSpawnPoint;
        Vector3 spawnPointResult;
        if (NavMesh.SamplePosition(spawnPoint.position, out secureSpawnPoint, 5f, NavMesh.AllAreas))
        {
            spawnPointResult = secureSpawnPoint.position;
        }
        else
        {
            spawnPointResult = spawnPoint.position;
        }

        mCameraObject = Instantiate(player.mCamera.gameObject, spawnPoint.position, Quaternion.identity);
        player.mGameObject = Instantiate(player.mGameObject, spawnPointResult, Quaternion.identity);
        player.mIdentifier = mSpawnCounter++;
        player.mBotBehaviour = player.mIdentifier >= 1 ? player.mGameObject.AddComponent<BotBehaviourScript>() : null;
        player.mCbrBehaviour = player.mIdentifier < 1 ? player.mGameObject.AddComponent<BotCBRBehaviourScript>() : null;
        player.InitGameObjectAndCamera();
        player.mGameObject.name = player.mName;

        mCameraObject.transform.parent = player.mGameObject.transform;
        mCameraObject.transform.position = mCameraObject.transform.position + new Vector3(0, 1, 0.5f);

        player.mPlayerShooting = player.mGameObject.AddComponent<PlayerShooting>();
        player.mPlayerShooting.mShootingPlayer = player;

        // C.W.: Highlights the CBR player in pumpkin material needed to do the modification and one
        // further player.mName check because the player.mGameObject is instantiated just a few
        // lines earlier. Its not possible to modify the player game object earlier because at first
        // it is the prefab itself instead of an instance of it.
        if (player.mName == "John Doe")
        {
            player.mGameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = mCBRMaterial;
        }

        if (player.mIsHumanControlled)
        {
            mHumanControlled = player;
        }
    }

    /**
     * Delegate-Methode, um zu ermitteln, welcher Spieler beim entsprechenden Mausklick schießen soll.
     */

    public void Clicked(object sender, EventArgs e)
    {
        Player player = sender as Player;

        if (player.Equals(mHumanControlled) && !mIsSwitching)
        {
            player.Shoot();
        }
    }

    /**
     * Unity Methode
     */

    private void FixedUpdate()
    {
        if (MainMenueScript.OnlyBots && mState == GameState.RUNNING)
        {
            Player tmp = null;
            if (Input.GetKeyDown(KeyCode.F1))
            {
                tmp = mPlayers[0];
                DisableSpectatorCamera();
                tmp.ActivatePlayer();
                mHumanControlled = tmp;
                PlayerShooting.mHumanPlayer = mHumanControlled;
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                tmp = mPlayers[1];
                DisableSpectatorCamera();
                tmp.ActivatePlayer();
                mHumanControlled = tmp;
                PlayerShooting.mHumanPlayer = mHumanControlled;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                foreach (Player player in mPlayers)
                {
                    if (player.mIsHumanControlled)
                    {
                        player.DeactivatePlayer();
                        mHumanControlled = null;
                        PlayerShooting.mHumanPlayer = null;
                        break;
                    }
                }

                EnableSpectatorCamera();
            }
        }
    }

    /**
     * Methode, um die Zuschauerkamera auszuschalten.
     */

    private void DisableSpectatorCamera()
    {
        MainMenueScript.OnlyBots = false;

        mSpectatorCameraGameObject.GetComponent<SpectatorCameraScript>().enabled = false;
        mSpectatorCameraGameObject.GetComponent<PlayerPerspective>().enabled = false;
        mSpectatorCameraGameObject.SetActive(false);
        mSpectatorCameraGameObject.GetComponent<Camera>().enabled = false;

        Destroy(mSpectatorCameraGameObject);
        mSpectatorCameraGameObject = null;

        hudCanvas.SetActive(true);
    }

    /**
     * Methode, um die Zuschauerkamera einzuschalten.
     */

    private void EnableSpectatorCamera()
    {
        MainMenueScript.OnlyBots = true;

        //C.W.: Changed the initial camera position
        //mSpectatorCameraGameObject = Instantiate(mSpectatorCameraGameObject, new Vector3(0f, 15f, 0f), Quaternion.identity);
        mSpectatorCameraGameObject = Instantiate(mSpectatorCameraGameObject, new Vector3(0f, 130f, -70f), Quaternion.identity);
        mSpectatorCameraGameObject.name = "SpectatorCamera";

        mSpectatorCameraGameObject.GetComponent<SpectatorCameraScript>().enabled = true;
        mSpectatorCameraGameObject.GetComponent<PlayerPerspective>().enabled = true;
        mSpectatorCameraGameObject.SetActive(true);
        mSpectatorCameraGameObject.GetComponent<Camera>().enabled = true;
        hudCanvas.SetActive(false);
    }

    /**
     * Unity Methode.
     */

    private void Update()
    {
        updateRoundTimer();
        updateCurrentCBRHealthPoints();

        // C.W.: initially loads the spectator cam
        if (mSpectatorCameraGameObject == null)
        {
            mSpectatorCameraGameObject = Resources.Load("Prefabs/SpectatorCamera") as GameObject;
        }
        
        restartPickUpTimer();
        checkInput();

        UpdateGameInformation();
    }

    /*
    * C.W.: Method updates the display of the current CBR HP.
    */

    private void updateCurrentCBRHealthPoints()
    {
        ScoreBoardManager.updateCurrentCBRHP(mCBRPlayer.mPlayerHealth);
    }


    /*
    * C.W.: Method starts the round timer and ends round if counter decrements to 0.
    */

    private void updateRoundTimer()
    {
        // C.W.: updates the timer value
        mRoundTimer -= Time.deltaTime;

        // C.W.: sets the updated timer in the RoundTimeManager which causes an UITimer update
        RoundTimeManager.mRoundTimeLeft = mRoundTimer;

        if (mRoundTimer <= 0)
        {
            EndRoundByTime();
            StartNewRound();
        }
    }

    /*
    * C.W.: Reorganized method to start the different timer for new pickUps
    */

    private void restartPickUpTimer()
    {
        if (mWeaponCrateCollected)
        {
            StartCoroutine(SpawnWeaponsCrate());
        }

        if (mHeartCollected)
        {
            StartCoroutine(SpawnHealthContainer());
        }

        if (mM4a1Collected)
        {
            StartCoroutine(SpawnM4A1());
        }
    }

    /*
     * C.W.: reorganized Method to outsource the input check from update method
     */

    private void checkInput()
    {
        if (Input.GetButton("Fire1"))
        {
            foreach (Player player in mPlayers)
            {
                player.OnClick();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (mHumanControlled != null)
            {
                Debug.Log(mHumanControlled.mName + " tries to switch weapon!");
                StartCoroutine(SwitchWeapon());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mState == GameState.PAUSED)
            {
                ContinueGame(false);
            }
            else
            {
                PauseGame();
            }

            mGameMenueScript.ToggleGameMenue();
            // If the MainMenue button is pressed
        }

        if (Input.GetButtonDown("Tab")) // GetButton(...)
        {
            Debug.Log("SHOW STATISTICS");
            foreach (Player player in mPlayers)
            {
                Debug.Log(player.ToString());
            }
        }
    }

    /**
     * Subroutine, um die Waffe zu wechseln.
     */

    public IEnumerator SwitchWeapon()
    {
        mIsSwitching = true;
        yield return new WaitForSeconds(mReloadTimer);
        mHumanControlled.SwitchWeapon();
        mIsSwitching = false;
    }

    /**
     * Methode, um das Spiel und den Zeitfluss fortzusetzen.
     */

    public void ContinueGame(bool fromScene)
    {
        Time.timeScale = 1;
        mState = GameState.RUNNING;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (fromScene)
        {
            mGameMenueScript.ToggleGameMenue();
        }
    }

    /**
     * Methode, um das Spiel und den Zeitfluss zu stoppen.
     */

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0;
        mState = GameState.PAUSED;
    }

    /**
     * Diese Unity-Methode wird beim Verlassen des Programms ausgeführt. Hier wird die Java-CBR Applikation beendet.
     */

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        // Cancel visab token
        VisabLoopCTS.Cancel();
        Constants.proc.Kill();
        Thread.Sleep(500);
#endif
    }

    /**
     * Methode, um einen Herzcontainer zu spawnen.
     */

    private IEnumerator SpawnHealthContainer()
    {
        mHeartCollected = false;
        Transform spawnPoint = null;

        bool free = false;

        while (!free)
        {
            spawnPoint = mHealthSpawnPoints[UnityEngine.Random.Range(0, mHealthSpawnPoints.Count)];
            Vector3 spawnVector = spawnPoint.position;

            var hitColliders = Physics.OverlapSphere(spawnVector, 2);

            int pCounter = 0;

            foreach (Collider collider in hitColliders)
            {
                if (collider.tag == "Health")
                {
                    pCounter++;
                }
            }

            if (pCounter <= 0)
            {
                free = true;
            }
        }

        yield return new WaitForSeconds(mHeartTimer);

        if (free)
        {
            Debug.Log("spawn health container");
            healthPositionRaw = spawnPoint.position;
            if (mHeartGameObject == null)
            {
                mHeartGameObject = Resources.Load("Prefabs/Health") as GameObject;
            }
            mHeartGameObject = Instantiate(mHeartGameObject, spawnPoint.position, Quaternion.identity);
            mHeartGameObject.AddComponent<HealthContainerScript>();
        }
    }

    /**
     * Methode, um eine Waffe zu spawnen.
     */

    private IEnumerator SpawnM4A1()
    {
        mM4a1Collected = false;
        Transform spawnPoint = null;

        bool free = false;

        while (!free)
        {
            spawnPoint = mM4a1SpawnPoints[UnityEngine.Random.Range(0, mM4a1SpawnPoints.Count)];
            Vector3 spawnVector = spawnPoint.position;

            var hitColliders = Physics.OverlapSphere(spawnVector, 2);

            int pCounter = 0;

            foreach (Collider collider in hitColliders)
            {
                if (collider.tag == "Machine Gun Collectable")
                {
                    pCounter++;
                }
            }

            if (pCounter <= 0)
            {
                free = true;
            }
        }

        yield return new WaitForSeconds(mWeaponSpawnTimer);

        if (free)
        {
            Debug.Log("spawn weapon");
            weaponPositionRaw = spawnPoint.position;
            if (mM4a1GameObject == null)
            {
                mM4a1GameObject = Resources.Load("Prefabs/M4A1_Collectable") as GameObject;
            }
            mM4a1GameObject = Instantiate(mM4a1GameObject, spawnPoint.position, Quaternion.identity);
            mM4a1GameObject.AddComponent<M4A1Script>();
        }
    }

    /**
     * Methode, um eine Munitionskiste zu spawnen.
     */

    private IEnumerator SpawnWeaponsCrate()
    {
        mWeaponCrateCollected = false;
        Transform spawnPoint = null;

        bool free = false;

        while (!free)
        {
            spawnPoint = mWeaponsCrateSpawnPoints[UnityEngine.Random.Range(0, mWeaponsCrateSpawnPoints.Count)];
            Vector3 spawnVector = spawnPoint.position;

            var hitColliders = Physics.OverlapSphere(spawnVector, 2);

            int pCounter = 0;

            foreach (Collider collider in hitColliders)
            {
                if (collider.tag == "Ammunition")
                {
                    pCounter++;
                }
            }

            if (pCounter <= 0)
            {
                free = true;
            }
        }

        yield return new WaitForSeconds(mWeaponCrateTimer);

        if (free)
        {
            Debug.Log("create crate");
            ammuPositionRaw = spawnPoint.position;
            mWeaponCrateGameObject = Instantiate(mWeaponsCrateObject, spawnPoint.position, Quaternion.identity);
            mWeaponCrateGameObject.AddComponent<AmmunitionScript>();
        }
    }

    /**
     * C.W.: Reorganized supporting method to assign the CBRPlayer member variable.
     */

    private void AssignCBRPlayer()
    {
        foreach (Player player in mPlayers)
        {
            if (player.mCBR)
            {
                mCBRPlayer = player;
            }
        }
    }
}