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
        Detaining,
        EndGame
    }

    private PoliceState mState;
    private Vector3 mMoveDirection = Vector3.zero;
    private CharacterController mController;

    private GameObject mDetainee;
    public float kPoliceSpeed = 15.0f;
    private GameObject[] mDetainPoints;
    private Transform mCapturePoint;
    private GameObject mClosestDetainmentPoint;
    public Vector3 MoveDirection { get { return mMoveDirection; } set { this.mMoveDirection = value; } }

    public Vector3 TargetDestination { get; internal set; }

    // Use this for initialization
    private void Start()
    {
        mState = PoliceState.Idle;
        mController = GetComponent<CharacterController>();
        Debug.Assert(mController != null);
        //Debug.Log("DEBUG MOVING TO PLAYER");
        //StartMoving(GameObject.Find("Player").transform.position - transform.position);
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
            case PoliceState.Idle:
                IdleTick();
                break;
            case PoliceState.Moving:
                MoveTick();
                break;
            case PoliceState.Detaining:
                DetainTick();
                break;
        }
    }

    private void IdleTick()
    {
        if (mMoveDirection != Vector3.zero)
        {
            StartMoving(mMoveDirection);
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
        if (mDetainee.CompareTag("Player"))
        {
            // Game end
            GameObject gco = GameObject.Find("GameController");
            GameController gc = gco.GetComponent<GameController>();
            gc.EndGame();
            mState = PoliceState.EndGame;
            mDetainee.GetComponent<PlayerController>().GameEnd();
        }
        else
        {
            Destroy(mDetainee);
            Destroy(gameObject);
        }
        // Message spawner??

        // Delete self
    }

    private void MoveTick()
    {
        mController.SimpleMove(mMoveDirection * kPoliceSpeed);
        transform.rotation = Quaternion.LookRotation(mMoveDirection);
        // See if we're near the end
        if ((TargetDestination - transform.position).sqrMagnitude < 4f)
        {
            // Deleted
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mState != PoliceState.Detaining)
        {
            bool isPlayer = other.CompareTag("Player") && other.GetType() == typeof(CharacterController);
            bool isPerson = other.CompareTag("Person");
            if (isPerson || isPlayer)
            {
                if (isPerson)
                {
                    PersonController pc = other.GetComponent<PersonController>();
                    if (pc == null)
                    {
                        return;
                    }
                }
                Debug.Log("Police interaction. Detaining: " + other.gameObject.name);
                Detain(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (mState == PoliceState.Detaining)
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.SimpleMove(cc.transform.position - transform.position);
            }
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
                if (pc != null)
                {
                    pc.Detain(gameObject);
                }
                else
                {
                    Debug.Log("Error Could not get personcontroller for " + other.gameObject.name);
                    //foreach (Collider policeCollider in transform.GetComponentsInChildren<Collider>())
                    //{
                    //    foreach (Collider personCollider in other.transform.GetComponentsInChildren<Collider>())
                    //    {
                    //        Physics.IgnoreCollision(policeCollider, personCollider);
                    //    }
                    //}
                    //return;
                }
                break;
        }
        //foreach (Collider collider in transform.GetComponentsInChildren<Collider>())
        //{
        //    if (collider.GetType() == typeof(CharacterController))
        //    {
        //        continue;
        //    }
        //    collider.enabled = false;
        //}
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

    private void OnDrawGizmos()
    {
        TextGizmo.Instance.DrawText(transform.position, string.Format("{0}", mState.ToString()));
    }

    public void StartMoving(Vector3 direction)
    {
        mMoveDirection = direction.normalized;
        mState = PoliceState.Moving;
    }
}
