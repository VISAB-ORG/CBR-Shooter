using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CampScript : MonoBehaviour {

    private Transform campPosition;
    private List<Transform> aimingPositions;
    private NavMeshAgent player;
    private Transform playerTransform;
    private float timer;
    private int aimingPos = 0;

    private void Awake()
    {
        aimingPositions = new List<Transform>(); 
        player = GetComponent<NavMeshAgent>();
        playerTransform = player.GetComponentInParent<Transform>();
    }

    //C.W.: Assigns the camping position from the privided List of GameControllerScript.
    private void assignCampingPosition()
    {
        foreach(Transform t in GameControllerScript.mCampingPositionTransforms)
        {
            if(t.name == "CampingPosition")
            {
                campPosition = t;
            }
        }
    }

    //C.W.: Assigns the aiming positions from the privided List of GameControllerScript.
    private void assignAimingPositions()
    {
        foreach (Transform t in GameControllerScript.mCampingPositionTransforms)
        {
            if (t.name.Contains("AimingPosition"))
            {
                aimingPositions.Add(t);
            }
        }
    }

    public void OnEnable()
    {
        //C.W.: Gets the specified Aiming and Camping positions from GameControllerScript.
        assignAimingPositions();
        assignCampingPosition();

        //C.W.: Resets the stopping distance that the player will stop directly on the specified position.
        player.stoppingDistance = 0.5f;
    }

    // Update is called once per frame
    void Update () {

        timer += Time.deltaTime;

        //C.W.: Lets the player goes straight to his camping position.
        player.destination = campPosition.position;

        //C.W.: If player arrived at his destination start aiming.
        if (playerTransform.position == player.destination) 
        {
            //C.W.: If the player is on the set destination (CampingPosition) aim to the two specified targets.
            // Every 3 seconds the current aiming pos will be changed. 
            if(timer >= 3)
            {
                if (aimingPos == 0)
                    aimingPos++;
                else
                    aimingPos--;

                timer = 0;
            }

            //C.W.: Let the player aim to the current aimingPos
            aiming();
        }
    }

    //C.W.: Lets the player aim.
    private void aiming()
    {
        //C.W.: Indicates the rotation speed the player used to aim between both target positions.
        float rotationSpeed = 10f;

        //C.W.: Lets the player aims to the specified aiming poisition (over Time, not instant).
        Vector3 aimDirection = (aimingPositions[aimingPos].position - playerTransform.position).normalized;
        Quaternion aimQuaternion = Quaternion.LookRotation(aimDirection);
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, aimQuaternion, Time.deltaTime * rotationSpeed);
    }
}
