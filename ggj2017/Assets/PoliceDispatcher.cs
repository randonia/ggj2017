using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceDispatcher : MonoBehaviour
{
    private enum SpawnRates
    {
        LOW = 10,
        MEDIUM = 5,
        HIGH = 3
    }

    public GameObject P_BasicPolice;
    public GameObject P_PoliceVan;

    private iTweenPath[] mPaths;
    private float mLastSpawn;
    public string DebugText;

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
        // DEBUG
        DebugText = string.Format("{0}\n{1}\n{2}\n{3}", mLastSpawn, Time.time, (int)SpawnRates.LOW, (mLastSpawn + (int)SpawnRates.LOW < Time.time));
        if (mLastSpawn + (int)SpawnRates.LOW < Time.time)
        {
            iTweenPath path = RandomPath();
            GameObject newThing = null;
            DebugText = DebugText + "\n" + path.pathName;
            switch (path.pathName)
            {
                case "basic":
                    Debug.Log("Creating a new PoPo");
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
            mLastSpawn = Time.time;
        }
    }

    private iTweenPath RandomPath()
    {
        return mPaths[Random.Range(0, mPaths.Length)];
    }
}
