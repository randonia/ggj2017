using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController mCharController;

    private Vector3 mMovementDir;
    private Vector3 mLookDir;

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
        transform.rotation = Quaternion.LookRotation(mLookDir);
        mCharController.SimpleMove(mMovementDir * 5.0f);
    }
}
