using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDispatcher : MonoBehaviour
{
    public enum PersonSpawnRates
    {
        LOW = 6,
        MEDIUM = 3,
        HIGH = 2,
        RIOT = 1
    }

    private iTweenPath mSpawnPoints;
    private float mLastSpawnTime;
    public PersonSpawnRates SpawnRate = PersonSpawnRates.LOW;
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
        int numExistingPersons = GameObject.FindGameObjectsWithTag("Person").Length;
        if (numExistingPersons < 10)
        {
            SpawnRate = PersonSpawnRates.RIOT;
        }
        else if (numExistingPersons < 20)
        {
            SpawnRate = PersonSpawnRates.HIGH;
        }
        else if (numExistingPersons < 40)
        {
            SpawnRate = PersonSpawnRates.MEDIUM;
        }
        else
        {
            SpawnRate = PersonSpawnRates.LOW;
        }
        if (mLastSpawnTime + (int)SpawnRate < Time.time)
        {
            SpawnPerson();
            mLastSpawnTime = Time.time;
        }
    }

    private void SpawnPerson()
    {
        Vector3 spawnPoint = mSpawnPoints.nodes[UnityEngine.Random.Range(0, mSpawnPoints.nodes.Count)];
        GameObject newPerson = Instantiate(P_Person);
        newPerson.transform.position = spawnPoint;
    }
}
