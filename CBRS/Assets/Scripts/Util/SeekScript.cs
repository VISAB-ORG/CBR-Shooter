using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SeekScript : MonoBehaviour {

    public List<Transform> mHotspots;
    public int mSeekIndex = 0;
    private NavMeshAgent player;

    private void Awake()
    {
        player = GetComponent<NavMeshAgent>();
        player.autoBraking = true;
    }
    //C.W.: Each time the component gets enabled it will build a new 
    // sorted mHotspots list according to the current position of the player. 
    // Also it resets the mSeekIndex to force the agent to start the hotSpotList from 0 
    // 0 equals to the closest hotspot
    private void OnEnable()
    {
        getSortedHotSpotsForPlayer();
        mSeekIndex = 0;
        GotoNextPoint();
    }

    void Update () {
        // C.W.: regularly check if the player reached a specific closeness to his destination
        // if so move to the next hotspot
        if (!player.pathPending && player.remainingDistance < 10f)
            GotoNextPoint();
    }
  
    private void GotoNextPoint()
    {
        if (mHotspots.Count == 0)
            return;

        player.destination = mHotspots[mSeekIndex].position;

        // C.W.: increments the Seek index and use modulo to loop the hotspots
        mSeekIndex = (mSeekIndex + 1) % mHotspots.Count;
    }

    public void getSortedHotSpotsForPlayer()
    {
        mHotspots = new List<Transform>();

        // C.W.: adds all spawn points to the hot spot list
        foreach (Transform transform in GameControllerScript.mHealthSpawnPoints)
        {
            mHotspots.Add(transform);
        }

        foreach (Transform transform in GameControllerScript.mM4a1SpawnPoints)
        {
            mHotspots.Add(transform);
        }

        foreach (Transform transform in GameControllerScript.mWeaponsCrateSpawnPoints)
        {
            mHotspots.Add(transform);
        }

        // C.W.: sorts the hotspot list according to the distance to agents current position
        mHotspots = mHotspots.OrderBy(
           x => Vector3.Distance(player.GetComponentInParent<Transform>().position, x.transform.position)
          ).ToList();
    }
}
