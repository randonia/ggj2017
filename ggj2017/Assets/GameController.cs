using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject G_Player;
    private PlayerController mPlayer;
    public GameObject G_UI_Root;
    public GameObject UI_SprintSlider;

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(G_Player != null);
        Debug.Assert(G_UI_Root != null);
        Debug.Assert(UI_SprintSlider != null);
        G_UI_Root.SetActive(true);
        mPlayer = G_Player.GetComponent<PlayerController>();
        Debug.Assert(mPlayer != null);
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 sprintSliderScale = UI_SprintSlider.transform.localScale;
        sprintSliderScale.x = Mathf.Max(0f, mPlayer.SprintVal);
        UI_SprintSlider.transform.localScale = sprintSliderScale;
    }
}
