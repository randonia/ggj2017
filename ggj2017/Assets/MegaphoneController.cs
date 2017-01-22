using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaphoneController : MonoBehaviour
{
    private const float kDuration = 1f;
    private ParticleSystem mParticleSystem;
    private float mStartTime;
    private bool mRunning;
    public GameObject G_AOESphere;

    // Use this for initialization
    private void Start()
    {
        mParticleSystem = GetComponent<ParticleSystem>();
        Debug.Assert(mParticleSystem != null);
        mRunning = false;
    }

    public void Begin()
    {
        mRunning = true;
        mStartTime = Time.time;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        mParticleSystem.Play();
    }

    // Update is called once per frame
    private void Update()
    {
        if (mRunning)
        {
            transform.Rotate(Vector3.right, 12);
            if (mStartTime + kDuration < Time.time)
            {
                mParticleSystem.Stop();
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                G_AOESphere.SetActive(false);
                mRunning = false;
            }
        }
    }
}
