using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public enum AudioSourceID
    {
        LOOP_DRUMS,
        LOOP_INSTRUMENTS,
        LOOP_HYPE,
        LOOP_MENUINTRO,
        LOOP_MENU
    }

    public AudioSource Loop_Drums;
    public AudioSource Loop_Instrumental;
    public AudioSource Loop_HYPE;

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(Loop_Drums != null);
        Debug.Assert(Loop_Instrumental != null);
        Debug.Assert(Loop_HYPE != null);
    }

    public void MixAudio(AudioSourceID src, float level)
    {
        switch (src)
        {
            case AudioSourceID.LOOP_DRUMS:
                Loop_Drums.volume = level;
                break;
            case AudioSourceID.LOOP_INSTRUMENTS:
                Loop_Instrumental.volume = level;
                break;
            case AudioSourceID.LOOP_HYPE:
                Loop_HYPE.volume = level;
                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
