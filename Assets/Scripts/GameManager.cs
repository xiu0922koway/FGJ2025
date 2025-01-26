using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LeaderboardCreatorDemo;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool isStart;
    public bool isEnd;
    public int bubbleNumber = 5;
    public List<float> playerScore = new List<float>();
    public List<GameObject> bubbles = new List<GameObject>();
    public float bestScore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
     
        if(playerScore.Count >= bubbleNumber)
        {
            isEnd = true;
        }
    }

    public void AddScore(float score)
    {
        playerScore.Add(score);
        bestScore = playerScore.Max();
        LeaderBoard.Instance.PlayerScoreCheck();
    }

    public void Reset()
    {
        playerScore.Clear();
        foreach(var bubble in bubbles)
        {
            Destroy(bubble);
        }
        bubbles.Clear();
        isStart = false;
        isEnd = false;
        bestScore = 0;

        BubbleController1.Instance.willGenerate = true;
    }
}
