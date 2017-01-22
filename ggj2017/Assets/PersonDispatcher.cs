using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDispatcher : MonoBehaviour
{
    private iTweenPath mSpawnPoints;
    private float mLastSpawnTime;
    public float SpawnRate = 5f;
    public bool running = false;
    public GameObject P_Person;

    // Use this for initialization
    private void Start()
    {
        mSpawnPoints = GetComponent<iTweenPath>();
        Debug.Assert(mSpawnPoints != null);
        Debug.Assert(P_Person != null);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!running)
        {
            return;
        }
        if (mLastSpawnTime + SpawnRate < Time.time)
        {
            SpawnPerson();
            mLastSpawnTime = Time.time;
        }
    }

    private void SpawnPerson()
    {
        Vector3 spawnPoint = mSpawnPoints.nodes[UnityEngine.Random.Range(0, mSpawnPoints.nodes.Count)];
        GameObject newPerson = Instantiate(P_Person);
        P_Person.transform.position = spawnPoint;
    }
}
