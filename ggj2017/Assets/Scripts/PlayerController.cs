﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Wait,
        Playing,
        Detained,
        End
    }

    private PlayerState mState;
    private CharacterController mCharController;
    private MegaphoneController mMegaphoneController;
    public GameObject mAOESphere;
    public GameObject G_Firebreathing;
    private ParticleSystem mFirebreather;
    private Vector3 mLookDir;
    private const float kTurnSpeed = 0.05f;
    public AudioSource[] WalkingSounds;
    private float mLastCharacterMoveTime = 0.0f;
    public AudioSource[] BoostSounds;
    public AudioSource[] TalkSounds;
    private float mLastCharacterTalkTime = 0.0f;

    private float mSprint = 1;
    public const float kMaxSprint = 1;
    public float SprintVal { get { return mSprint / kMaxSprint; } }

    public const float kSprintRate = 0.01f;
    public const float kSprintRecovery = 0.003f;
    public const float kBoostRechargeRate = 0.00025f;

    private float mBoostCharge = 0f;
    public bool BoostReady { get { return mBoostCharge == 1f; } }

    public Color BoostColor { get { return BoostReady ? Color.green : Color.white; } }

    public string BoostString { get { return string.Format("{0:P0}", System.Math.Round(mBoostCharge, 2)); } }
    // Use this for initialization

    private GameObject G_InfluencedPerson;
    private List<GameObject> mInfluencedPeople;
    private List<GameObject> mAOEInfluencedPeople;
    private Stack<GameObject> mCleanupStack;

    private void Start()

    {
        mState = PlayerState.Wait;
        mInfluencedPeople = new List<GameObject>();
        mAOEInfluencedPeople = new List<GameObject>();
        mCleanupStack = new Stack<GameObject>();
        mLookDir = new Vector3();
        mCharController = GetComponent<CharacterController>();
        Debug.Assert(mCharController != null);
        mMegaphoneController = GetComponentInChildren<MegaphoneController>();
        mMegaphoneController.gameObject.SetActive(false);
        Debug.Assert(mMegaphoneController != null);
        Debug.Assert(mAOESphere != null);
        Debug.Assert(G_Firebreathing != null);
        mFirebreather = G_Firebreathing.GetComponent<ParticleSystem>();
        Debug.Assert(mFirebreather != null);
    }

    public void BeginGame()
    {
        mState = PlayerState.Playing;
    }

    internal void GameEnd()
    {
        mState = PlayerState.End;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (mState)
        {
            case PlayerState.Playing:
                PlayingTick();
                break;
            case PlayerState.Detained:
                break;
            case PlayerState.Wait:
                // Waiting for the game to start
                break;
        }

        #region DEBUG

        foreach (GameObject go in mInfluencedPeople)
        {
            if (go == null)
            {
                mCleanupStack.Push(go);
                continue;
            }
            PersonController pc = go.GetComponent<PersonController>();
            if (pc == null || pc.isDetained)
            {
                mCleanupStack.Push(go);
                continue;
            }
            Debug.DrawLine(transform.position, go.transform.position, Color.blue);
        }

        // Clean up the list of influenced people
        while (mCleanupStack.Count > 0)
        {
            mInfluencedPeople.Remove(mCleanupStack.Pop());
        }

        foreach (GameObject go in mAOEInfluencedPeople)
        {
            if (go == null)
            {
                mCleanupStack.Push(go);
                continue;
            }
            PersonController pc = go.GetComponent<PersonController>();
            if (pc == null || pc.isDetained)
            {
                mCleanupStack.Push(go);
                continue;
            }
            Debug.DrawLine(transform.position, go.transform.position, Color.blue);
        }

        // Clean up the list of AOE
        while (mCleanupStack.Count > 0)
        {
            mInfluencedPeople.Remove(mCleanupStack.Pop());
        }

        #endregion DEBUG
    }

    private void PlayingTick()
    {
        #region Movement

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

        if (mMovementDir.magnitude >= 0.1f)
        {
            if (Time.time > mLastCharacterMoveTime + 0.35f)
            {
                mLastCharacterMoveTime = Time.time;
                // Play walkikng sound at random
                WalkingSounds[new System.Random().Next(WalkingSounds.Length)].Play();
            }
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
        float moveSpeed = (Input.GetKey(KeyCode.J)) ? 2.0f : 10.0f;
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

        #endregion Movement

        #region "Combat"

        if (BoostReady)
        {
            BoostReadyTick();
        }
        if (Input.GetKeyDown(KeyCode.K) && BoostReady)
        {
            DoBoost();
        }

        if (Input.GetKey(KeyCode.J))
        {
            mFirebreather.Play();

            if (Time.time > mLastCharacterTalkTime + 0.25f)
            {
                mLastCharacterTalkTime = Time.time;
                TalkSounds[new System.Random().Next(TalkSounds.Length)].Play();
            }

            // Try to convert everyone in your immediate effect
            foreach (GameObject conversionTarget in mInfluencedPeople)
            {
                PersonController pc = conversionTarget.GetComponent<PersonController>();
                if (pc != null)
                {
                    pc.BeginConvert(gameObject);
                }
            }
        }
        else
        {
            mFirebreather.Stop();
        }

        #endregion "Combat"

        #region Regen

        mBoostCharge = Mathf.Min(1f, mBoostCharge + kBoostRechargeRate);

        #endregion Regen
    }

    private void BoostReadyTick()
    {
        if (!mAOESphere.activeSelf)
        {
            mAOESphere.SetActive(true);
        }
        mAOESphere.transform.Rotate(mAOESphere.transform.up, 5f);
        mAOESphere.GetComponent<MeshRenderer>().materials[0].color = Color.Lerp(Color.red, Color.blue, Mathf.Sin(Time.time));
    }

    internal void Detain(GameObject source)
    {
        mState = PlayerState.Detained;
        foreach (Collider collider in transform.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        GetComponent<CharacterController>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Person"))
        {
            mInfluencedPeople.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mInfluencedPeople.Remove(other.gameObject);
    }

    private void DoBoost()
    {
        BoostSounds[new System.Random().Next(BoostSounds.Length)].Play();
        mMegaphoneController.gameObject.SetActive(true);
        Debug.Log("Do boost");
        mMegaphoneController.Begin();
        mMegaphoneController.G_AOESphere = mAOESphere;
        mBoostCharge = 0f;
        foreach (GameObject influencedPerson in mAOEInfluencedPeople)
        {
            if (influencedPerson == null)
            {
                continue;
            }
            PersonController pc = influencedPerson.GetComponent<PersonController>();
            if (pc == null)
            {
                continue;
            }
            pc.BoostConvert();
        }
    }

    internal void RemoveTrackedAOE(GameObject gameObject)
    {
        mAOEInfluencedPeople.Remove(gameObject);
    }

    internal void AddTrackedAOE(GameObject gameObject)
    {
        if (gameObject.CompareTag("Person"))
        {
            mAOEInfluencedPeople.Add(gameObject);
        }
    }
}
