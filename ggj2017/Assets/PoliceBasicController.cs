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

    // Use this for initialization
    private void Start()
    {
        mState = PoliceState.Idle;
        mController = GetComponent<CharacterController>();
        Debug.Assert(mController != null);
        Debug.Log("DEBUG MOVING TO PLAYER");
        StartMoving(GameObject.Find("Player").transform.position - transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (mState)
        {
            case PoliceState.Moving:
                MoveTick();
                break;
        }
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
                PlayerController personController = mDetainee.GetComponent<PlayerController>();
                personController.Detain(gameObject);
                break;
            case "Person":

                break;
        }
        foreach (Collider collider in transform.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        mState = PoliceState.Detaining;
    }

    public void StartMoving(Vector3 direction)
    {
        mMoveDirection = direction.normalized;
        mState = PoliceState.Moving;
    }
}
