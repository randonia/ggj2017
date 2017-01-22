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
    public float Conversion { get { return mConversionFactor; } }
    public string ConversionString { get { return string.Format("{0:P0}", Conversion); } }
    private float mConversionFactor;

    public Material M_NormalPerson;
    public Material M_ConvertedPerson;

    // Use this for initialization
    private void Start()
    {
        mConversionFactor = 0;
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
            mConversionFactor = Mathf.Min(mConversionFactor + 0.25f, 1f);
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
    }

    private void OnDrawGizmos()
    {
        TextGizmo.Instance.DrawText(transform.position, ConversionString);
    }
}
