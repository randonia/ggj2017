using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject G_AudioListener;
    private GameObject G_Target;
    public Vector3 TargetOffsetVector;
    public Vector3 CameraGroundVector;

    public Vector3 TargetGroundPosition { get { return G_Target.transform.position + TargetOffsetVector; } }
    public Vector3 TargetPosition { get { return (G_Target.transform.position + TargetOffsetVector + CameraGroundVector); } }
    public bool[] debugs = { true, true, true };

    // Use this for initialization
    private void Start()
    {
        // Try to find the player
        if (G_Target == null)
        {
            G_Target = GameObject.Find("Player");
        }
        Debug.Assert(G_Target != null);
        Debug.Assert(G_AudioListener != null);
    }

    private void Update()
    {
        if (debugs[0]) Debug.DrawLine(transform.position, G_Target.transform.position, Color.yellow);
        if (debugs[2]) Debug.DrawLine(transform.position, TargetPosition, Color.red);
        Vector3 deltaPos = TargetPosition - transform.position;
        Debug.DrawRay(transform.position, deltaPos, Color.green);
        transform.Translate(deltaPos * Time.deltaTime * 3.0f);
        G_AudioListener.transform.position = G_Target.transform.position;
    }
}
