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

    private float mStartTime;

    public float StartTime { get { return mStartTime; } }
    private float mRunningTime;
    public float RunningTime { get { return mRunningTime; } }
    private int mScore;

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
        StartGame();
    }

    private void StartGame()
    {
        State = GameState.Playing;
        mStartTime = Time.time;
        GameObject.Find("PoliceDispatcher").GetComponent<PoliceDispatcher>().running = true;
        GameObject.Find("PersonDispatcher").GetComponent<PersonDispatcher>().running = true;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (State)
        {
            case GameState.Playing:
                mRunningTime += Time.deltaTime;
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
