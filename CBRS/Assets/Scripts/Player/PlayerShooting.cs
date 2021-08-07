using UnityEngine;
using Assets.Scripts.Model;
using Assets.Scripts.Util;
using System.Collections;
using Assets.Scripts.CBR.Model;
using Assets.Scripts.CBR.Plan;
using System.Collections.Generic;
using UnityEngine.AI;

/**
 * Dieses Skript übernimmt den Vorgang des Schießens eines Spielers.
 */
public class PlayerShooting : MonoBehaviour
{
    public float mRange = 100f;

    private float mTimer;
    private Ray mShootRay = new Ray();
    private RaycastHit mShootHit;
    private int mShootableMask;
    private ParticleSystem mGunParticles;
    private LineRenderer mGunLine;
    private AudioSource mGunAudio;
    private Light mGunLight;
    private float mEffectsDisplayTime = 0.2f;
    private int mReloadSeconds = 2;
    private bool mIsReloading = false;

    private GameObject mSpectatorCameraGameObject;

    public Player mShootingPlayer { get; set; }

    private PlayerHealth mPlayerHealthScript;

    public static Player mHumanPlayer;

    private void Awake()
    {
        mShootableMask = LayerMask.GetMask("Shootable");
    }

    private void Update()
    {

        if (mShootingPlayer != null)
        {
            InitComponents();
        }
        mTimer += Time.deltaTime;

        if (mHumanPlayer == null)
        {
            foreach (Player player in GameControllerScript.mPlayers)
            {
                if (player.mIsHumanControlled)
                {
                    mHumanPlayer = player;
                    break;
                }
            }
        }


        if (mHumanPlayer != null && mPlayerHealthScript == null)
        {
            mPlayerHealthScript = mHumanPlayer.mGameObject.GetComponent<PlayerHealth>();
        }

        if (mHumanPlayer != null && mPlayerHealthScript != null)
        {
            mPlayerHealthScript.UpdateAmmunition(mHumanPlayer);
        }

        if (mShootingPlayer != null && mTimer >= mShootingPlayer.mEquippedWeapon.mFireRate * mEffectsDisplayTime)
        {
            DisableEffects();
        }

        if (mShootingPlayer != null && mShootingPlayer.mIsHumanControlled && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(mShootingPlayer.mName + " reloads!");
            mIsReloading = true;
            StartCoroutine(ReloadRoutine(mShootingPlayer));

        }
    }

    private void InitComponents()
    {
        if (mGunLight == null)
        {
            mGunLight = StaticMenueFunctions.FindComponentInChildren<Light>(mShootingPlayer.mGameObject);
        }
        if (mGunParticles == null)
        {
            mGunParticles = StaticMenueFunctions.FindComponentInChildren<ParticleSystem>(mShootingPlayer.mGameObject);
        }
        if (mGunLine == null)
        {
            mGunLine = StaticMenueFunctions.FindComponentInChildren<LineRenderer>(mShootingPlayer.mGameObject);
        }
        if (mGunAudio == null)
        {
            mGunAudio = StaticMenueFunctions.FindComponentInChildren<AudioSource>(mShootingPlayer.mGameObject);
        }

    }

    public void DisableEffects()
    {
        mGunLine.enabled = false;
        mGunLight.enabled = false;
    }

    public void Shoot()
    {

        if (mTimer >= mShootingPlayer.mEquippedWeapon.mFireRate && Time.timeScale != 0 && !mIsReloading)
        {
            if (DoShoot())
            {
                if (mHumanPlayer != null && mPlayerHealthScript != null)
                {
                    mPlayerHealthScript.UpdateAmmunition(mHumanPlayer);
                }
            }
            else
            {
                if (!mShootingPlayer.mIsHumanControlled)
                {
                    StartCoroutine(ReloadRoutine(mShootingPlayer));
                }
            }
        }

    }

    // Das Nachladen soll nicht sofort geschehen, sondern x Sekunden dauern, derzeit 2 Sekunden
    private IEnumerator ReloadRoutine(Player player)
    {
        yield return new WaitForSeconds(mReloadSeconds);
        Debug.Log(player.mName + " finished reloading!");
        mIsReloading = false;
        player.Reload();
    }

    public bool DoShoot()
    {

        if (mShootingPlayer.mEquippedWeapon.mCurrentMagazineAmmu <= 0)
        {
            return false;
        }

        mShootingPlayer.mEquippedWeapon.mCurrentMagazineAmmu -= 1;
        mTimer = 0f;

        mGunAudio.Play();

        mGunLight.enabled = true;

        mGunParticles.Stop();
        mGunParticles.Play();

        LineRenderer line = StaticMenueFunctions.FindComponentInChildren<LineRenderer>(mShootingPlayer.mGameObject);
        mGunLine.enabled = true;
        mGunLine.SetPosition(0, line.transform.position);



        mShootRay.origin = transform.position;
        mShootRay.direction = transform.forward;

        if (Physics.Raycast(mShootRay, out mShootHit, mRange, mShootableMask))
        {
            HitByRaycast(mShootHit.collider.gameObject);
            mGunLine.SetPosition(1, mShootHit.point);
        }
        else
        {
            mGunLine.SetPosition(1, mShootRay.origin + mShootRay.direction * mRange);
        }

        return true;
    }

    /**
     * C.W.: This method implements the TakeDamangeFunction considering a respective inaccuracy 
     * depending on the movement of shooting player and distance between shooting and hit player.
     */
    private void TakeDamageAndConsiderAccuracy(Player hitPlayer)
    {
        Vector3 posShooting = mShootingPlayer.mGameObject.transform.position;
        Vector3 posHit = hitPlayer.mGameObject.transform.position;
        int roll;
        bool shooterIsWalking = mShootingPlayer.mGameObject.GetComponent<Animator>().GetBool("IsWalking");
        bool hitPlayerIsWalking = hitPlayer.mGameObject.GetComponent<Animator>().GetBool("IsWalking");

        if (!hitPlayerIsWalking)
        {
            //C.W.: Random roll between 1 and 100 in case the potentially hit player is not moving.
            roll = Random.Range(1, 100);
        }
        else
        {
            //C.W.: Random roll between 1 and 90 in case the potentially hit player is moving.
            // Considers a minus 10 points at rolling to simulate lower probability to hit a moving player.
            roll = Random.Range(1, 90);
        }

        //C.W.: Checks if the shooting player is moving while shooting.
        if (shooterIsWalking)
        {

            //C.W.: Checks the distance between shooting player and his target.
            if (Vector3.Distance(posShooting, posHit) >= 50)
            {
                //C.W.: If distance between shooting and hit player is bigger than x there is a 
                // 20 percent chance of missing the target.
                if (roll > 20)
                {
                    hitPlayer.TakeDamage(mShootingPlayer.mEquippedWeapon.mDamage);
                }
                else
                {
                    //C.W.: Shooting Player missed his target
                    Debug.Log(mShootingPlayer.mName + " missed the target!");
                }
            }
            else if (Vector3.Distance(posShooting, posHit) >= 35)
            {
                if (roll > 10)
                {
                    hitPlayer.TakeDamage(mShootingPlayer.mEquippedWeapon.mDamage);
                }
                else
                {
                    //C.W.: Shooting Player missed his target
                    Debug.Log(mShootingPlayer.mName + " missed the target!");
                }
            }
            else if (Vector3.Distance(posShooting, posHit) >= 20)
            {
                if (roll > 7)
                {
                    hitPlayer.TakeDamage(mShootingPlayer.mEquippedWeapon.mDamage);
                }
                else
                {
                    //C.W.: Shooting Player missed his target
                    Debug.Log(mShootingPlayer.mName + " missed the target!");
                }
            }
            else if (Vector3.Distance(posShooting, posHit) < 10)
            {
                if (roll > 2)
                {
                    hitPlayer.TakeDamage(mShootingPlayer.mEquippedWeapon.mDamage);
                }
                else
                {
                    //C.W.: Shooting Player missed his target
                    Debug.Log(mShootingPlayer.mName + " missed the target!");
                }
            }
        }
        else
        {
            //C.W.: If the shooting player isnt moving, check if the target player is moving.
            if (hitPlayerIsWalking)
            {
                // C.W.: If the target player is moving there is 5 percent chance to miss him because of his movement.
                if (roll <= 5)
                {
                    //C.W.: Shooting Player missed his target because target was moving.
                    Debug.Log(mShootingPlayer.mName + " missed the target!");
                }
                else
                {
                    hitPlayer.TakeDamage(mShootingPlayer.mEquippedWeapon.mDamage);
                }

                // C.W.: If the shooting player is not moving and the target player is not moving there is a 100 percent chance for a hit.
            }
            else
            {
                hitPlayer.TakeDamage(mShootingPlayer.mEquippedWeapon.mDamage);
            }

        }
    }

    private void HitByRaycast(GameObject source)
    {
        foreach (Player hitPlayer in GameControllerScript.mPlayers)
        {
            if (source.name.Equals(hitPlayer.mName))
            {

                // C.W.: Handles the damge input of the hit player. Considers current movement of shooting player
                // as well as distance between both (target, source).
                TakeDamageAndConsiderAccuracy(hitPlayer);

                //C.W.: Disabled TakeDamage function. Used instead TakeDamageAndConsiderAccuracy(...)
                //hitPlayer.TakeDamage(mShootingPlayer.mEquippedWeapon.mDamage);

                if (!hitPlayer.mIsHumanControlled)
                {
                    if (!hitPlayer.mStatus.isEnemyVisible)
                    {
                        CommonUnityFunctions.mRotationFinished = false;
                        CommonUnityFunctions.LookAround(hitPlayer);
                    }
                    else
                    {
                        CommonUnityFunctions.mRotationFinished = false;
                        CommonUnityFunctions.LookAt(hitPlayer, mShootingPlayer);
                    }

                }


                Debug.Log(hitPlayer.mName + " has " + hitPlayer.mPlayerHealth + "/" + Player.mMaxLife + " life points left");

                hitPlayer.mIsAlive = hitPlayer.mPlayerHealth > 0;

                if (hitPlayer.mIsHumanControlled && GameControllerScript.hudCanvas.activeSelf)
                {
                    mPlayerHealthScript = hitPlayer.mGameObject.GetComponent<PlayerHealth>();

                    if (mPlayerHealthScript != null)
                    {
                        mPlayerHealthScript.damaged = true;
                        mPlayerHealthScript.TakeDamage(hitPlayer);
                    }

                }

                //  the hitPlayer died
                if (!hitPlayer.mIsAlive)
                {
                    hitPlayer.mStatus = new Status();
                    hitPlayer.mStatistics.AddDeath(new Death());
                    mShootingPlayer.mStatistics.AddFrag(new Frag());

                    mShootingPlayer.mStatus.enemiesLastKnownPosition = new Vector3(10000f, 10000f, 10000f);

                    Constants.SaveDeath(hitPlayer, GameControllerScript.mGameStart);
                    Constants.SaveFrag(mShootingPlayer, GameControllerScript.mGameStart);

                    if (mShootingPlayer.mCBR)
                    {
                        // C.W.: Updates the ScoreBoard
                        Debug.Log("CBR won");
                        ScoreBoardManager.increaseFrags();

                        mShootingPlayer.mPlan.progress = (int)Plan.Progress.DONE;
                        mShootingPlayer.mStatus.isEnemyVisible = false;
                        mShootingPlayer.mStatus.isEnemyAlive = false;
                        mShootingPlayer.mStatus.lastPosition = (int)Status.LastPosition.unknown;
                        mShootingPlayer.mStatus.distanceToEnemy = (int)Status.Distance.unknown;

                        for (int i = 0; i < mShootingPlayer.mPlan.GetActionCount(); i++)
                        {
                            mShootingPlayer.mPlan.GetActionByIndex(i).finished = true;
                        }

                    }
                    else
                    {
                        // C.W.: Updates the ScoreBoard
                        ScoreBoardManager.increaseDeaths();
                    }
                    //Debug.Log(hitPlayer.mName + " was killed by " + mShootingPlayer.mName + " after " + (hitPlayer.mStatistics.GetLatestDeath().mTimestamp - GameControllerScript.mGameStart));
                    hitPlayer.mGameObject.SetActive(false);

                    if (!MainMenueScript.OnlyBots && hitPlayer.mIsHumanControlled)
                    {
                        EnableSpectatorCamera();
                    }

                    GameControllerScript.StartNewRound();

                    Debug.Log("-------Round Ended by Frag -----------");
                }
                break;
            }
        }
    }

    private void DeactivateShooting(Player player)
    {
        for (int i = 0; i < player.mPlan.GetActionCount(); i++)
        {
            Debug.Log("PlayerShooting#DeactivateShooting#for (int i = " + i + "; " + i + " < " + player.mPlan.GetActionCount() + "; " + i + "++)");
            player.mPlan.actions[i].finished = true;
        }
    }

    private void EnableSpectatorCamera()
    {
        mSpectatorCameraGameObject = Resources.Load("Prefabs/SpectatorCamera") as GameObject;
        mSpectatorCameraGameObject = Instantiate(mSpectatorCameraGameObject, new Vector3(0f, 15f, 0f), Quaternion.identity);
        mSpectatorCameraGameObject.name = "SpectatorCamera";

        mSpectatorCameraGameObject.GetComponent<SpectatorCameraScript>().enabled = true;
        mSpectatorCameraGameObject.GetComponent<PlayerPerspective>().enabled = true;
        mSpectatorCameraGameObject.SetActive(true);
        mSpectatorCameraGameObject.GetComponent<Camera>().enabled = true;
        GameControllerScript.hudCanvas.SetActive(false);
    }

    private void DisableSpectatorCamera()
    {
        if (mSpectatorCameraGameObject != null)
        {
            mSpectatorCameraGameObject.GetComponent<SpectatorCameraScript>().enabled = false;
            mSpectatorCameraGameObject.GetComponent<PlayerPerspective>().enabled = false;
            mSpectatorCameraGameObject.SetActive(false);
            mSpectatorCameraGameObject.GetComponent<Camera>().enabled = false;

            GameControllerScript.hudCanvas.SetActive(true);

            Destroy(mSpectatorCameraGameObject);
            mSpectatorCameraGameObject = null;

        }

    }

    // C.W.: Obsolete Method - it is round based now.
    private IEnumerator DelayedRespawn(Player player)
    {
        yield return new WaitForSeconds(GameControllerScript.mRespawnTime);

        bool free = false;

        while (!free)
        {
            Transform spawnPoint = GameControllerScript.mSpawnPoints[Random.Range(0, GameControllerScript.mSpawnPoints.Count)];
            Vector3 spawnVector = spawnPoint.position;

            var hitColliders = Physics.OverlapSphere(spawnVector, 2);

            if (hitColliders.Length > 0)
            {

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
                    player.mGameObject.transform.position = spawnPoint.position;
                    player.mGameObject.transform.rotation = Quaternion.identity;

                    if (mPlayerHealthScript != null)
                    {
                        mPlayerHealthScript.ResetValues();
                    }
                }

            }
        }

        if (!MainMenueScript.OnlyBots)
        {
            DisableSpectatorCamera();
        }

        player.Init();
        player.mGameObject.SetActive(true);

    }
}