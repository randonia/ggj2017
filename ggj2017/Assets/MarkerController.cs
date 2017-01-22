using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerController : MonoBehaviour
{
    public GameObject[] LinesTo;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        foreach (GameObject go in LinesTo)
        {
            Debug.DrawLine(transform.position, go.transform.position, Color.magenta);
        }
    }
}
