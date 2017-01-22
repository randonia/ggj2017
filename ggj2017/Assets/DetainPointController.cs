﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetainPointController : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            TextGizmo.Instance.DrawText(transform.position + Vector3.up, "Detain Point");
            Gizmos.DrawWireCube(collider.transform.position, collider.size);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered: " + other.gameObject);
    }
}
