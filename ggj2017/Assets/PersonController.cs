using System;
using UnityEngine;
using UnityEngine.AI;

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
    private Vector3 mTargetDestination;
    private CharacterController mCharacterController;
    private float mMoveSpeed;

    // Use this for initialization
    private void Start()
    {
        mState = PersonState.Idle;
        Conversion = 0;
        Debug.Assert(M_ConvertedPerson != null);
        Debug.Assert(M_NormalPerson != null);
        mCharacterController = GetComponent<CharacterController>();
        Debug.Assert(mCharacterController != null);
        mMoveSpeed = UnityEngine.Random.Range(1f, 3f);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (mState)
        {
            case PersonState.Idle:
                IdleTick();
                break;
            case PersonState.Converted:
                ConvertedTick();
                break;
        }
    }

    private void ConvertedTick()
    {
        // Run toward the goal!
        Debug.DrawLine(transform.position + Vector3.up, mTargetDestination, Color.green);
        Vector3 remainder = mTargetDestination - transform.position;
        if (remainder.sqrMagnitude < 5f)
        {
            Score();
        }
    }

    private void Score()
    {
        GameObject gco = GameObject.Find("GameController");
        GameController gc = gco.GetComponent<GameController>();
        gc.ScorePerson();
        Destroy(gameObject);
    }

    private void IdleTick()
    {
        if (ConversionCheck())
        {
            return;
        }
        if (mTargetDestination == Vector3.zero)
        {
            mTargetDestination = GetRandomPosition();
        }
        // Move towards the destination
        Debug.DrawLine(transform.position, mTargetDestination, Color.green);
        Vector3 dir = mTargetDestination - transform.position;
        if (dir.sqrMagnitude < 4.0f)
        {
            mTargetDestination = GetRandomPosition();
        }
        dir.Normalize();
        mCharacterController.SimpleMove(dir * mMoveSpeed);
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private bool ConversionCheck()
    {
        if (Conversion >= 1f && mState == PersonState.Idle)
        {
            // Begin Conversion if not converted
            Convert();
            return true;
        }
        return false;
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(UnityEngine.Random.Range(-25f, 25f), 0f, UnityEngine.Random.Range(-25f, 25f));
    }

    public void BeginConvert(GameObject source)
    {
        if (source.CompareTag("Player"))
        {
            Debug.Log("Converted by Player");
            Conversion = Mathf.Min(Conversion + 0.25f, 1f);
        }
        ConversionCheck();
    }

    private void Convert()
    {
        mState = PersonState.Converted;
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "model")
            {
                child.GetComponent<MeshRenderer>().material = M_ConvertedPerson;
                break;
            }
        }
        mTargetDestination = GameObject.Find("ScoreGoal").transform.position;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(mTargetDestination);
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
        TextGizmo.Instance.DrawText(transform.position, string.Format("{0}\n{1}", mState.ToString(), ConversionString));
    }

    internal void BoostConvert()
    {
        Conversion = 1;
    }
}
