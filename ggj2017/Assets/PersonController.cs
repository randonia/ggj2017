﻿using System;
using UnityEngine;
using UnityEngine.AI;

public class PersonController : MonoBehaviour
{
    private enum PersonState
    {
        Idle,
        Converted,
        Detained
    }

    private PersonState mState;
    public bool isDetained { get { return mState == PersonState.Detained; } }

    public string ConversionString { get { return string.Format("{0:P0}", Conversion); } }
    public float Conversion;

    public Material M_NormalPerson;
    public Material M_ConvertedPerson;
    private Vector3 mTargetDestination;
    private CharacterController mCharacterController;

    // Use this for initialization
    private void Start()
    {
        mState = PersonState.Idle;
        Conversion = 0;
        Debug.Assert(M_ConvertedPerson != null);
        Debug.Assert(M_NormalPerson != null);
        mCharacterController = GetComponent<CharacterController>();
        Debug.Assert(mCharacterController != null);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (mState)
        {
            case PersonState.Idle:
                IdleTick();
                break;
            case PersonState.Converted:
                ConvertedTick();
                break;
        }
    }

    private void ConvertedTick()
    {
        // Run toward the goal!
        Debug.DrawLine(transform.position + Vector3.up, mTargetDestination, Color.green);
        Vector3 remainder = mTargetDestination - transform.position;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent.path.corners.Length > 0)
        {
            transform.rotation = Quaternion.LookRotation(remainder);
        }
        if (remainder.sqrMagnitude < 5f)
        {
            Score();
        }
    }

    private void Score()
    {
        GameObject gco = GameObject.Find("GameController");
        GameController gc = gco.GetComponent<GameController>();
        gc.ScorePerson();
        Destroy(gameObject);
    }

    private void IdleTick()
    {
        if (ConversionCheck())
        {
            return;
        }
        if (mTargetDestination == Vector3.zero)
        {
            mTargetDestination = GetRandomPosition();
        }
        // Move towards the destination
        Debug.DrawLine(transform.position, mTargetDestination, Color.green);
        Vector3 dir = mTargetDestination - transform.position;
        if (dir.sqrMagnitude < 5.0f)
        {
            mTargetDestination = GetRandomPosition();
        }
        dir.Normalize();
        GetComponent<NavMeshAgent>().SetDestination(mTargetDestination);
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private bool ConversionCheck()
    {
        if (Conversion >= 1f && mState == PersonState.Idle)
        {
            // Begin Conversion if not converted
            Convert();
            return true;
        }
        return false;
    }

    private Vector3 GetRandomPosition()
    {
        // Pick a random target position based on BottomLeft and TopRight
        Vector3 bottomLeft = GameObject.Find("BottomLeft").transform.position;
        Vector3 topRight = GameObject.Find("TopRight").transform.position;
        return new Vector3(UnityEngine.Random.Range(bottomLeft.x, topRight.x), 0f, UnityEngine.Random.Range(bottomLeft.z, topRight.z));
    }

    public void BeginConvert(GameObject source)
    {
        if (source.CompareTag("Player"))
        {
            Conversion = Mathf.Min(Conversion + 0.05f, 1f);
        }
        ConversionCheck();
    }

    private void Convert()
    {
        mState = PersonState.Converted;
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "model")
            {
                child.GetComponent<MeshRenderer>().material = M_ConvertedPerson;
                break;
            }
        }
        mTargetDestination = GameObject.Find("ScoreGoal").transform.position;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(mTargetDestination);
        agent.speed = 25f;
    }

    public void Detain(GameObject source)
    {
        // Kill off all the colliders for this
        foreach (Collider collider in GetComponents<Collider>())
        {
            collider.enabled = false;
        }
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        mState = PersonState.Detained;
    }

    private void OnDrawGizmos()
    {
    }

    internal void BoostConvert()
    {
        Conversion = 1;
    }
}
