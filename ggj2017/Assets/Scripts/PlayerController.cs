﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController mCharController;

    private Vector3 mMovementDir;
    private Vector3 mLookDir;
    private const float kTurnSpeed = 0.05f;

    private float mSprint = 1;
    public const float kMaxSprint = 1;
    public float SprintVal { get { return mSprint / kMaxSprint; } }

    public const float kSprintRate = 0.01f;

    // Rate at which sprint is recovered
    public const float kSprintRecovery = 0.003f;

    // Use this for initialization

    private void Start()
    {
        mMovementDir = new Vector3();
        mLookDir = new Vector3();
        mCharController = GetComponent<CharacterController>();
        Debug.Assert(mCharController != null, "Player needs CharacterController!");
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
        Vector3 mMovementDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            mMovementDir.z += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            mMovementDir.z -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            mMovementDir.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            mMovementDir.x -= 1.0f;
        }

        //moveVec.x = Input.GetAxis("Horizontal");
        mLookDir = Vector3.Lerp(mLookDir, mMovementDir, Time.deltaTime * 2.5f);
        mLookDir.x = (mMovementDir.x - mLookDir.x) * kTurnSpeed + mLookDir.x;
        mLookDir.y = (mMovementDir.y - mLookDir.y) * kTurnSpeed + mLookDir.y;
        mLookDir.z = (mMovementDir.z - mLookDir.z) * kTurnSpeed + mLookDir.z;
        if (mMovementDir.x != 0 || mMovementDir.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(mLookDir);
        }
        float moveSpeed = 10.0f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (SprintVal > 0.0f)
            {
                moveSpeed *= 1.5f;
                mSprint = Mathf.Max(mSprint - kSprintRate, 0f);
            }
        }
        else
        {
            mSprint = Mathf.Min(1.0f, mSprint + kSprintRecovery);
        }
        mCharController.SimpleMove(mMovementDir * moveSpeed);
    }
}