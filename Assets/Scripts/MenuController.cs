using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public RectTransform startPanelRoot;
    public RectTransform leaderBoardRoot;
    public RectTransform inGamePanelRoot;
    private TMP_Text remainText;
    private bool isStart;
    private bool isEnd;
    
    // Start is called before the first frame update
    void Start()
    {
        remainText = inGamePanelRoot.transform.Find("RemainText").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isStart)
        {
            if(GameManager.Instance.isStart)
            {
                StartGame();
            }
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                Application.Quit();
                Debug.Log("quit");
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                BackToStart();
            }
        }

        if(!GameManager.Instance.isEnd)
        {
            remainText.text = (GameManager.Instance.bubbleNumber-GameManager.Instance.playerScore.Count).ToString();
        }
        else
        {
            if(!isEnd) ShowLeaderBoard();
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                BackToStart();
            }
        }



    }

    public void StartGame()
    {
        startPanelRoot.DOAnchorPosX(-Screen.width, 2f);
        inGamePanelRoot.DOAnchorPosY(0, 1f);
        isStart = true;
    }

    public void ShowLeaderBoard()
    {
        inGamePanelRoot.DOAnchorPosY(-Screen.height, 2f);
        leaderBoardRoot.DOAnchorPosX(0, 2f);
        isEnd = true;
    }

    public void BackToStart()
    {
        inGamePanelRoot.DOAnchorPosY(-Screen.height, 2f);
        startPanelRoot.DOAnchorPosX(0, 2f);
        GameManager.Instance.isStart = false;
    }
}
