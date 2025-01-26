using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    public RectTransform startPanelRoot;
    public RectTransform leaderBoardRoot;
    private bool isEnd;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.isStart)
        {
            if(Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
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
            
        }
        else
        {
            if(!isEnd) ShowLeaderBoard();
        }



    }

    public void StartGame()
    {
        startPanelRoot.DOAnchorPosX(-Screen.width, 2f);
        GameManager.Instance.isStart = true;
    }

    public void ShowLeaderBoard()
    {
        leaderBoardRoot.DOAnchorPosX(0, 2f);
        isEnd = true;
    }

    public void BackToStart()
    {
        startPanelRoot.DOAnchorPosX(0, 2f);
        GameManager.Instance.isStart = false;
    }
}
