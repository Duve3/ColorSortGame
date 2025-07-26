using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHandler : MonoBehaviour
{
    public GameObject GameMaker;
    public GameObject CompletionObjects;
    public Button[] GameButtons;

    public TMP_Text LevelText;
    public TMP_Text MoveCountText;

    private int MoveCount;

    private int LevelCount = 1;

    private MakerHandler GameMakerHandler;

    private GameObject selectedBall = null;

    private readonly List<Color> Colors = new() { 
        new Color(65f  / 255f, 135f / 255f, 245f / 255f), // blue
        new Color(235f / 255f, 25f  / 255f, 215f / 255f), // pink
        new Color(235f / 255f, 25f  / 255f, 25f  / 255f), // red
        new Color(235f / 255f, 105f / 255f, 25f  / 255f), // orange
        new Color(55f  / 255f, 165f / 255f, 15f  / 255f), // green
        new Color(220f / 255f, 215f / 255f, 30f  / 255f), // yellow
        new Color(145f / 255f, 25f  / 255f, 190f / 255f), // purple
        new Color(20f  / 255f, 195f / 255f, 180f / 255f), // aqua
        new Color(100f / 255f, 50f  / 255f, 15f  / 255f), // brown
        new Color(255f / 255f, 255f / 255f, 255f / 255f), // white
    };

    private Stack<List<GameObject>> MoveList = new() { };

    // Start is called before the first frame update
    void Start()
    {
        GameMakerHandler = GameMaker.GetComponent<MakerHandler>();

        LevelCount = PlayerPrefs.GetInt("Level");
        // ^ returns 0 by default!!!
        if (LevelCount == 0)
        {
            LevelCount = 1;
        }
        Debug.Log("LevelCount: " + LevelCount + " ; LevelText: " + LevelText);
        LevelText.text = "Level: " + LevelCount.ToString();

        // this is intentionally done to allow the next function to run properly
        LevelCount--;
        NextLevel();
    }

    private void IncrementLevel()
    {
        LevelCount++;
        PlayerPrefs.SetInt("Level", LevelCount);
        LevelText.text = "Level: " + LevelCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 WorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            WorldPos.z = 0f;

            Collider2D hit = Physics2D.OverlapPoint(WorldPos);

            if (hit != null && hit.gameObject.CompareTag("Tube"))
            {
                Debug.Log("Capsule clicked!");
                if (selectedBall == null) 
                { 
                    SelectBall(hit.gameObject); 
                }
                else
                {
                    PutBall(hit.gameObject);
                }
            } 
            else
            {
                Debug.Log("no capsule");
            }
        }
    }

    public int GetNumberOfTubes()
    {
        int val = (int)Mathf.Floor(LevelCount / 3) + 5;

        // we cant display more than 12!
        if (val > 12) { return 12; }
        return val;
    }

    void SelectBall(GameObject tube)
    {
        /* 
         * This function will take a ball from a tube
         * and then put in the "open space"
         */

        GameObject ball = tube.GetComponent<TubeHandler>().PopBall();

        if (!ball.CompareTag("Ball"))
        {
            return;  // this means that our tube is empty (or completed!)
        }

        float tube_height = tube.GetComponent<SpriteRenderer>().bounds.size.y;

        float top = tube.transform.position.y + (tube_height / 2);

        Debug.Log("Top: " + top + " ; y: " + tube.transform.position.y + " ; height: " + tube_height);

        // WARN?: remove itween usage maybe? i dont really like how its done, but we will use it for now
        iTween.MoveTo(ball, new Vector3(ball.transform.position.x, top + 0.25f, 0), 0.75f);

        selectedBall = ball;
        ball.GetComponent<BallData>().previousTube = tube;
    }

    void PutBall(GameObject tube)
    {
        if (tube == selectedBall.GetComponent<BallData>().previousTube)
        {
            // basically just force the ball on since it was our previous tube
            tube.GetComponent<TubeHandler>().AddBall(selectedBall, true);
            selectedBall = null;
            return;
        }

        bool result = tube.GetComponent<TubeHandler>().AddBall(selectedBall);

        if (result)
        {
            MoveCount++;
            // push this move (selectedBall, (from) selectedBall.previous, (to) tube)
            MoveList.Push(new List<GameObject>() { selectedBall, selectedBall.GetComponent<BallData>().previousTube, tube });
            selectedBall = null;
            bool done = IsGameOver();

            if (done)
            {
                Debug.Log("Game completed, drawing objects and creating completions");
                foreach (GameObject t in GameMakerHandler.Tubes) {
                    // ensures all tubes (including empties) are drawn to be solved
                    t.GetComponent<TubeHandler>().DrawCompletion();
                }

                Debug.Log("moves: " + MoveCount);
                MoveCountText.text = "Moves: " + MoveCount;
                CompletionObjects.SetActive(true);

                foreach (Button child in GameButtons)
                {
                    child.interactable = false;
                }
            }

            return;
        }

        // TODO: minor bug, if you spam left click and it causes shakes
        //      the ball will begin to move farther and farther left due
        //      to the spammed coroutines (ball.transform.position keeps moving)
        selectedBall.GetComponent<BallData>().ShakeBall();
        // ^ above bug is minor because it doesn't break any gameplay
    }

    private bool IsGameOver()
    {
        int solved = 0;

        int empty = 0;

        foreach (GameObject t in GameMakerHandler.Tubes)
        {
            TubeHandler t_h = t.GetComponent<TubeHandler>();

            if (t_h.solved)
            {
                solved += 1;
                continue;
            }

            if (t_h.balls.Count == 0)
            {
                empty += 1;
                continue;
            }
        }

        if (solved + empty == GetNumberOfTubes())
        {
            return true;
        }

        return false;
    }

    public void NextLevel()
    {
        ClearLevel();

        foreach (Button child in GameButtons)
        {
            child.interactable = true;
        }

        IncrementLevel();
        GameMakerHandler.CreateGame(GetNumberOfTubes());
        GameMakerHandler.GenerateFill(Colors);
        MoveCount = 0;
    }

    public void UndoMove()
    {
        if (MoveList.Count < 1) { return; }
        List<GameObject> move = MoveList.Pop();

        // return selected ball to original spot (and wipes it from our data)
        if (selectedBall)
        {
            GameObject prev = selectedBall.GetComponent<BallData>().previousTube;
            prev.GetComponent<TubeHandler>().AddBall(selectedBall, true);
            selectedBall = null;
        }

        // uncomplete any completed tubes
        if (move[2].GetComponent<TubeHandler>().solved)
        {
            move[2].GetComponent<TubeHandler>().Uncomplete();
        }

        // remove the ball then move it to the other tube
        move[2].GetComponent<TubeHandler>().PopBall();
        move[1].GetComponent<TubeHandler>().AddBall(move[0], true);
    }

    private void ClearLevel()
    {
        GameMakerHandler.ResetTubes();
        CompletionObjects.SetActive(false);
        MoveList.Clear();
    }

    // ONLY FOR USAGE ON THE "RESTART?" BUTTON
    public void RegenFill()
    {
        /* 
         * ONLY USE ON THE "RESTART?" BUTTON!!!!!
         */

        // removes the current selected ball
        if (selectedBall) {
            selectedBall.GetComponent<BallData>().previousTube.GetComponent<TubeHandler>().AddBall(selectedBall, true);
            selectedBall = null;
        }

        // basically just "nextLevel" but doesnt increment level counter
        // ^ and uses the previous fill
        GameMakerHandler.ResetTubes();
        GameMakerHandler.CreateGame(GetNumberOfTubes());
        GameMakerHandler.RecreateMostRecentFill();
        MoveCount = 0;
    }
}
