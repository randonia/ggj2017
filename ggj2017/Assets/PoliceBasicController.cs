using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBasicController : MonoBehaviour
{
    private enum PoliceState
    {
        Idle,
        Moving,
        Detaining
    }

    private PoliceState mState;
    private Vector3 mMoveDirection;
    private CharacterController mController;

    private GameObject mDetainee;
    public const float kPoliceSpeed = 5.0f;
    private GameObject[] mDetainPoints;
    private Transform mCapturePoint;
    private GameObject mClosestDetainmentPoint;

    // Use this for initialization
    private void Start()
    {
        mState = PoliceState.Idle;
        mController = GetComponent<CharacterController>();
        Debug.Assert(mController != null);
        Debug.Log("DEBUG MOVING TO PLAYER");
        StartMoving(GameObject.Find("Player").transform.position - transform.position);
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "CapturePoint")
            {
                mCapturePoint = child;
                break;
            }
        }
        Debug.Assert(mCapturePoint != null);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (mState)
        {
            case PoliceState.Moving:
                MoveTick();
                break;
            case PoliceState.Detaining:
                DetainTick();
                break;
        }
    }

    private void DetainTick()
    {
        foreach (GameObject detainPoint in mDetainPoints)
        {
            Debug.DrawLine(transform.position + Vector3.up, detainPoint.transform.position, (detainPoint == mClosestDetainmentPoint) ? Color.red : Color.gray);
        }
        mDetainee.transform.position = mCapturePoint.transform.position;
        // Move toward the closest detention point
        Vector3 moveVec = mClosestDetainmentPoint.transform.position - transform.position;
        if (moveVec.sqrMagnitude < 6f)
        {
            // Abort
            DoCleanup();
        }
        moveVec.Normalize();
        mController.SimpleMove(moveVec * kPoliceSpeed);
        transform.rotation = Quaternion.LookRotation(moveVec);
    }

    private void DoCleanup()
    {
        // Delete target
        Destroy(mDetainee);
        // Message spawner??

        // Delete self
        Destroy(gameObject);
    }

    private void MoveTick()
    {
        mController.SimpleMove(mMoveDirection * kPoliceSpeed);
        transform.rotation = Quaternion.LookRotation(mMoveDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Person"))
        {
            Debug.Log("Police interaction. Detaining: " + other.gameObject.name);
            Detain(other.gameObject);
        }
    }

    private void Detain(GameObject other)
    {
        mDetainee = other;
        switch (other.tag)
        {
            case "Player":
                PlayerController plrc = mDetainee.GetComponent<PlayerController>();
                plrc.Detain(gameObject);
                break;
            case "Person":
                PersonController pc = mDetainee.GetComponent<PersonController>();
                pc.Detain(gameObject);
                break;
        }
        foreach (Collider collider in transform.GetComponentsInChildren<Collider>())
        {
            if (collider.GetType() == typeof(CharacterController))
            {
                continue;
            }
            collider.enabled = false;
        }
        // Pick the closest detainment point
        mDetainPoints = GameObject.FindGameObjectsWithTag("DetainPoint");
        float minDist = float.MaxValue;
        mClosestDetainmentPoint = null;
        foreach (GameObject detainPoint in mDetainPoints)
        {
            float dist = (transform.position - detainPoint.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                mClosestDetainmentPoint = detainPoint;
            }
        }
        mState = PoliceState.Detaining;
    }

    public void StartMoving(Vector3 direction)
    {
        mMoveDirection = direction.normalized;
        mState = PoliceState.Moving;
    }
}
