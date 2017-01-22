using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        StartMenu,
        Playing,
        EndGame
    }

    public GameState State;
    public GameObject G_Player;
    private PlayerController mPlayer;
    public GameObject G_UI_Root;
    public GameObject UI_SprintSlider;
    public GameObject G_AudioController;
    private AudioController mAudio;
    private float mStartTime;
    public GameObject G_PoliceDispatcher;
    public GameObject G_PersonDispatcher;
    private PoliceDispatcher mPoliceDispatcher;
    private PersonDispatcher mPersonDispatcher;

    public float StartTime { get { return mStartTime; } }
    private float mRunningTime;
    public float RunningTime { get { return mRunningTime; } }
    private int mScore;
    private float mTimeLastChange;
    public float PhaseChangeRate = 20f;
    public int Score { get { return mScore; } }

    public float GameTime { get { return mRunningTime - StartTime; } }
    public int GameSeconds { get { return ((int)GameTime) % 60; } }
    public int GameMinutes { get { return ((int)GameTime) / 60; } }
    public string GameTimeString { get { return string.Format("{0}:{1:D2}", GameMinutes, (int)GameTime); } }

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(G_Player != null);
        Debug.Assert(G_UI_Root != null);
        Debug.Assert(UI_SprintSlider != null);
        G_UI_Root.SetActive(true);
        mPlayer = G_Player.GetComponent<PlayerController>();
        Debug.Assert(mPlayer != null);
        Debug.Assert(G_AudioController != null);
        mAudio = G_AudioController.GetComponent<AudioController>();
        Debug.Assert(mAudio != null);

        Debug.Assert(G_PoliceDispatcher != null);
        mPoliceDispatcher = G_PoliceDispatcher.GetComponent<PoliceDispatcher>();
        Debug.Assert(mPoliceDispatcher != null);
        Debug.Assert(G_PersonDispatcher != null);
        mPersonDispatcher = G_PersonDispatcher.GetComponent<PersonDispatcher>();
        Debug.Assert(mPersonDispatcher != null);
        StartGame();
    }

    private void StartGame()
    {
        State = GameState.Playing;
        mStartTime = Time.time;
        GameObject.Find("PoliceDispatcher").GetComponent<PoliceDispatcher>().running = true;
        GameObject.Find("PersonDispatcher").GetComponent<PersonDispatcher>().running = true;
        mAudio.MixAudio(AudioController.AudioSourceID.LOOP_DRUMS, 0.75f);
        mAudio.MixAudio(AudioController.AudioSourceID.LOOP_INSTRUMENTS, 0.75f);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (State)
        {
            case GameState.Playing:
                mRunningTime += Time.deltaTime;
                if (mTimeLastChange + PhaseChangeRate < Time.time)
                {
                    // If it's been 20 seconds, change the rate of things
                    int newRand = UnityEngine.Random.Range(0, 4);
                    switch (newRand)
                    {
                        case 0:
                            // Switch to low
                            mPoliceDispatcher.SpawnRate = PoliceDispatcher.SpawnRates.LOW;
                            mPersonDispatcher.SpawnRate = PersonDispatcher.PersonSpawnRates.LOW;
                            break;
                        case 1:
                            // Switch to medium
                            mPoliceDispatcher.SpawnRate = PoliceDispatcher.SpawnRates.MEDIUM;
                            mPersonDispatcher.SpawnRate = PersonDispatcher.PersonSpawnRates.MEDIUM;
                            break;
                        case 2:
                            // switch to high
                            mPoliceDispatcher.SpawnRate = PoliceDispatcher.SpawnRates.HIGH;
                            mPersonDispatcher.SpawnRate = PersonDispatcher.PersonSpawnRates.HIGH;
                            break;
                        case 3:
                            // switch to RIOT
                            mPoliceDispatcher.SpawnRate = PoliceDispatcher.SpawnRates.HYPE;
                            mPersonDispatcher.SpawnRate = PersonDispatcher.PersonSpawnRates.RIOT;
                            break;
                    }

                    if (mPoliceDispatcher.SpawnRate == PoliceDispatcher.SpawnRates.HYPE)
                    {
                        mAudio.MixAudio(AudioController.AudioSourceID.LOOP_HYPE, 1f);
                        mAudio.MixAudio(AudioController.AudioSourceID.LOOP_DRUMS, 0f);
                    }
                    else
                    {
                        mAudio.MixAudio(AudioController.AudioSourceID.LOOP_HYPE, 0f);
                        mAudio.MixAudio(AudioController.AudioSourceID.LOOP_DRUMS, 1f);
                    }
                    mTimeLastChange = Time.time;
                }

                break;
        }

        #region UI Updates

        Vector3 sprintSliderScale = UI_SprintSlider.transform.localScale;
        sprintSliderScale.x = Mathf.Max(0.02f, mPlayer.SprintVal);
        UI_SprintSlider.transform.localScale = sprintSliderScale;

        #endregion UI Updates
    }

    internal void ScorePerson()
    {
        mScore++;
    }

    internal void EndGame()
    {
        State = GameState.EndGame;
        // Disable the player
        mPlayer.GameEnd();
        // Disable everyone
        GameObject.Find("PoliceDispatcher").GetComponent<PoliceDispatcher>().running = false;
        GameObject.Find("PersonDispatcher").GetComponent<PersonDispatcher>().running = false;
    }
}
