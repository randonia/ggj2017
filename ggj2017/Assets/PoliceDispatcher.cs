using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceDispatcher : MonoBehaviour
{
    public enum SpawnRates
    {
        LOW = 5,
        MEDIUM = 3,
        HIGH = 2,
        HYPE = 1
    }

    public GameObject P_BasicPolice;
    public GameObject P_PoliceVan;

    public SpawnRates SpawnRate;
    public bool running = false;

    private iTweenPath[] mPaths;
    private float mLastSpawn;

    /// <summary>
    /// To add more spawn point/directions:
    /// 1. Add another ITweenPath to the PoliceDispatcher
    /// 2. make sure it has only 2 nodes
    /// 3. Name it something to switch on and a spawnrate (in ms)
    ///     - "basic-13"
    /// </summary>
    // Use this for initialization
    private void Start()
    {
        mPaths = GetComponents<iTweenPath>();
        mLastSpawn = -5000f; // Start 5 seconds after the game begins
        Debug.Assert(mPaths.Length > 0);
        Debug.Assert(P_BasicPolice != null);
        Debug.Assert(P_PoliceVan != null);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!running)
        {
            return;
        }

        if (mLastSpawn + (int)SpawnRate < Time.time)
        {
            int numToMake = (SpawnRate == SpawnRates.HYPE) ? 4 : 2;
            for (int i = 0; i < numToMake; ++i)
            {
                iTweenPath path = RandomPath();
                GameObject newThing = null;
                switch (path.pathName)
                {
                    case "basic":
                        newThing = Instantiate(P_BasicPolice);
                        break;
                }
                if (newThing == null)
                {
                    Debug.Log("Error");
                    return;
                }
                newThing.transform.position = path.nodes[0];
                Vector3 dir = (path.nodes[1] - path.nodes[0]).normalized;
                PoliceBasicController pbc = newThing.GetComponent<PoliceBasicController>();
                pbc.MoveDirection = dir;
                pbc.TargetDestination = path.nodes[1];
            }
            mLastSpawn = Time.time;
        }
    }

    private iTweenPath RandomPath()
    {
        return mPaths[Random.Range(0, mPaths.Length)];
    }
}
