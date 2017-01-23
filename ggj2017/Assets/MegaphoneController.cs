using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaphoneController : MonoBehaviour
{
    private const float kDuration = 1f;
    private ParticleSystem mParticleSystem;
    private float mStartTime;
    public bool mRunning = false;
    public GameObject G_AOESphere;

    // Use this for initialization
    private void Start()
    {
        mParticleSystem = GetComponent<ParticleSystem>();
        Debug.Assert(mParticleSystem != null);
    }

    public void Begin()
    {
        mRunning = true;
        mStartTime = Time.time;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        GetComponent<ParticleSystem>().Play();
    }

    // Update is called once per frame
    private void Update()
    {
        if (mRunning)
        {
            transform.Rotate(Vector3.right, 12);
            if (mStartTime + kDuration < Time.time)
            {
                GetComponent<ParticleSystem>().Stop();
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                G_AOESphere.SetActive(false);
                mRunning = false;
            }
        }
    }
}
