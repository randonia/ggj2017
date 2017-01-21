using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOETriggerController : MonoBehaviour
{
    public GameObject G_Player;
    private PlayerController mOwnerController;

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(G_Player != null);
        mOwnerController = G_Player.GetComponent<PlayerController>();
        Debug.Assert(mOwnerController != null);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HI2U " + other.gameObject.name);
        mOwnerController.AddTrackedAOE(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("BAI2U " + other.gameObject.name);
        mOwnerController.RemoveTrackedAOE(other.gameObject);
    }
}
