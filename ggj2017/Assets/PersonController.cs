using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour
{
    private enum PersonState
    {
        Idle,
        Converted,
        Detained
    }

    private PersonState mState;
    public bool isDetained { get { return mState == PersonState.Detained; } }

    public string ConversionString { get { return string.Format("{0:P0}", Conversion); } }
    public float Conversion;

    public Material M_NormalPerson;
    public Material M_ConvertedPerson;

    // Use this for initialization
    private void Start()
    {
        Conversion = 0;
        Debug.Assert(M_ConvertedPerson != null);
        Debug.Assert(M_NormalPerson != null);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void BeginConvert(GameObject source)
    {
        if (source.CompareTag("Player"))
        {
            Debug.Log("Converted by Player");
            Conversion = Mathf.Min(Conversion + 0.25f, 1f);
        }
        if (Conversion == 1f && mState == PersonState.Idle)
        {
            // Begin Conversion if not converted
            Convert(source);
        }
    }

    private void Convert(GameObject source)
    {
        mState = PersonState.Converted;
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "model")
            {
                child.GetComponent<MeshRenderer>().material = M_ConvertedPerson;
            }
        }
    }

    public void Detain(GameObject source)
    {
        // Kill off all the colliders for this
        foreach (Collider collider in GetComponents<Collider>())
        {
            collider.enabled = false;
        }
        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        mState = PersonState.Detained;
    }

    private void OnDrawGizmos()
    {
        TextGizmo.Instance.DrawText(transform.position, ConversionString);
    }
}
