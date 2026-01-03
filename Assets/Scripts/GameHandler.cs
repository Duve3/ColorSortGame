using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ReSharper disable PossibleLossOfFraction

public class GameHandler : MonoBehaviour
{
    public GameObject gameMaker;
    public GameObject completionObjects;
    public Button[] gameButtons;
    public Button addTubeButton;

    public TMP_Text levelText;
    public TMP_Text moveCountText;

    public bool extraTube;

    private int _moveCount;
    private int _levelCount = 1;

    private MakerHandler _gameMakerHandler;

    private DataSave _dataSave;

    private GameObject _selectedBall;

    private readonly List<Color> _colors = new()
    {
        new Color(65f / 255f, 135f / 255f, 245f / 255f), // blue
        new Color(235f / 255f, 25f / 255f, 215f / 255f), // pink
        new Color(235f / 255f, 25f / 255f, 25f / 255f), // red
        new Color(235f / 255f, 105f / 255f, 25f / 255f), // orange
        new Color(55f / 255f, 165f / 255f, 15f / 255f), // green
        new Color(220f / 255f, 215f / 255f, 30f / 255f), // yellow
        new Color(145f / 255f, 25f / 255f, 190f / 255f), // purple
        new Color(20f / 255f, 195f / 255f, 180f / 255f), // aqua
        new Color(100f / 255f, 50f / 255f, 15f / 255f), // brown
        new Color(255f / 255f, 255f / 255f, 255f / 255f), // white
        new Color(80f / 255f, 0f / 255f, 50f / 255f), // dark pink?
        new Color(50f / 255f, 0f / 255f, 80f / 255f), // dark purple/blue?
    };

    private readonly Stack<List<GameObject>> _moveList = new();

    private bool _inGUI;

    // Start is called before the first frame update
    private void Start()
    {
        _gameMakerHandler = gameMaker.GetComponent<MakerHandler>();

        _dataSave = GetComponent<DataSave>();

        _levelCount = PlayerPrefs.GetInt("Level");
        // ^ returns 0 by default!!!
        if (_levelCount == 0)
        {
            _levelCount = 1;
        }

        Debug.Log("LevelCount: " + _levelCount + " ; LevelText: " + levelText);
        levelText.text = "Level: " + _levelCount.ToString();
        
        // BEFORE WE RUN NEXT LEVEL, LETS CHECK FOR MORE SAVE DATA!!
        List<List<Color>> data = _dataSave.ReadGameData();

        if (data == _dataSave.ListEmpty)
        {
            Debug.Log("NO SAVE DATA FOUND, GENERATING NEW LEVEL");
            // we continue with our normal code
            _levelCount--;
            NextLevel();
        }
        else
        {
            Debug.Log("SAVE DATA FOUND, LOADING LEVEL");
            LevelFromSaveData(data);
        }

    }
    
    // todo: so far we have implemented an auto save upon application pause!
    //      now we gotta add: autosave upon closing scene (aka pressing the back button)
    //                      and actually loading our cool save file upon game launch/refocus?

    private void OnApplicationPause(bool status)
    {
        if (!status)
        {
            return;
        }
        // this means we are pausing the game, so we save our data
        Debug.Log("Application paused, saving data...");

        GameSaveData();
    }

    public void GameSaveData()
    {
        List<List<Color>> final = new();
        foreach (GameObject tube in _gameMakerHandler.tubes)
        {
            final.Add(tube.GetComponent<TubeHandler>().Balls.ToArray()
                .Select(obj => obj.GetComponent<SpriteRenderer>().color).ToList());
        }

        _dataSave.SaveGameData(final);
    }

    private void IncrementLevel()
    {
        _levelCount++;
        PlayerPrefs.SetInt("Level", _levelCount);
        levelText.text = "Level: " + _levelCount.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_inGUI)
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (Camera.main is null)
        {
            throw new NullReferenceException();
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.gameObject.CompareTag("Tube"))
        {
            Debug.Log("Capsule clicked!");
            if (_selectedBall == null)
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

    private int GetNumberOfTubes()
    {
        int val;

        // this weird ahh code basically just makes it scale slower past 7 tubes (1 row)
        if (_levelCount < 9)
        {
            val = (int)Mathf.Floor(_levelCount / 3) + 5;
        }
        else if (_levelCount < 20)
        {
            val = (int)Mathf.Floor(_levelCount / 5) + 6;
        }
        else if (_levelCount < 40)
        {
            val = (int)Mathf.Floor(_levelCount / 10) + 8;
        }
        else
        {
            val = (int)Mathf.Floor(_levelCount / 20) + 10;
        }


        // we cant display more than the row limit * 2 (two rows!)!
        if (val > _gameMakerHandler.rowLimit * 2)
        {
            return _gameMakerHandler.rowLimit * 2;
        }

        return val;
    }

    private void SelectBall(GameObject tube)
    {
        /*
         * This function will take a ball from a tube
         * and then put in the "open space"
         */

        GameObject ball = tube.GetComponent<TubeHandler>().PopBall();

        if (!ball.CompareTag("Ball"))
        {
            return; // this means that our tube is empty (or completed!)
        }

        float tubeHeight = tube.GetComponent<SpriteRenderer>().bounds.size.y;

        float top = tube.transform.position.y + (tubeHeight / 2);

        Debug.Log("Top: " + top + " ; y: " + tube.transform.position.y + " ; height: " + tubeHeight);

        // hopefully this works
        BallData bd = ball.GetComponent<BallData>();
        bd.animationHandler.AddAnimationToQueue(new Vector3(ball.transform.position.x, top + 0.25f, 0), 0.25f, true);

        _selectedBall = ball;
        bd.previousTube = tube;
    }

    private void PutBall(GameObject tube)
    {
        // ReSharper disable once InconsistentNaming
        TubeHandler t_h = tube.GetComponent<TubeHandler>();
        BallData bd = _selectedBall.GetComponent<BallData>();
        if (tube == bd.previousTube)
        {
            // basically just force the ball on since it was our previous tube
            t_h.AddBall(_selectedBall, true);
            _selectedBall = null;
            return;
        }

        bool result = t_h.AddBall(_selectedBall);

        if (result)
        {
            _moveCount++;
            // push this move (selectedBall, (from) selectedBall.previous, (to) tube)
            _moveList.Push(new List<GameObject>() { _selectedBall, bd.previousTube, tube });
            _selectedBall = null;
            bool done = IsGameOver();

            if (done)
            {
                Debug.Log("Game completed, drawing objects and creating completions");
                foreach (GameObject t in _gameMakerHandler.tubes)
                {
                    // ensures all tubes (including empties) are drawn to be solved
                    t.GetComponent<TubeHandler>().DrawCompletion();
                }

                Debug.Log("moves: " + _moveCount);
                moveCountText.text = "Moves: " + _moveCount;
                completionObjects.SetActive(true);

                foreach (Button child in gameButtons)
                {
                    child.interactable = false;
                }
            }

            return;
        }

        bd.ShakeBall();
        // ^ above bug HOPEFULLY is fixed!!
    }

    private bool IsGameOver()
    {
        int solved = 0;

        int empty = 0;

        foreach (GameObject t in _gameMakerHandler.tubes)
        {
            // ReSharper disable once InconsistentNaming
            TubeHandler t_h = t.GetComponent<TubeHandler>();

            if (t_h.solved)
            {
                solved += 1;
                continue;
            }

            if (t_h.Balls.Count == 0)
            {
                empty += 1;
                // im not sure why its consider redundant
                // ReSharper disable once RedundantJumpStatement
                continue;
            }
        }

        if (extraTube)
        {
            if (solved + empty == GetNumberOfTubes() + 1)
            {
                return true;
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
        // destroys the selected ball 
        if (_selectedBall)
        {
            DestroyImmediate(_selectedBall.gameObject);
            _selectedBall = null;
        }

        ClearLevel();

        foreach (Button child in gameButtons)
        {
            child.interactable = true;
        }

        IncrementLevel();
        _gameMakerHandler.CreateGame(GetNumberOfTubes());
        _gameMakerHandler.GenerateFill(_colors);
        _moveCount = 0;
        extraTube = false;

        // ensures an ad is ready
        addTubeButton.gameObject.SetActive(true);
    }

    private void LevelFromSaveData(List<List<Color>> saveData)
    {
        if (_selectedBall)
        {
            DestroyImmediate(_selectedBall.gameObject);
            _selectedBall = null;
        }

        ClearLevel();

        foreach (Button child in gameButtons)
        {
            child.interactable = true;
        }
        _gameMakerHandler.CreateGame(GetNumberOfTubes());
        _gameMakerHandler.GenerateFromFillData(saveData);
        _moveCount = 0;
        extraTube = false;

        // ensures an ad is ready
        addTubeButton.gameObject.SetActive(true);
    }

    public void UndoMove()
    {
        if (_moveList.Count < 1)
        {
            return;
        }

        List<GameObject> move = _moveList.Pop();

        // return selected ball to original spot (and wipes it from our data)
        if (_selectedBall)
        {
            GameObject prev = _selectedBall.GetComponent<BallData>().previousTube;
            prev.GetComponent<TubeHandler>().AddBall(_selectedBall, true);
            _selectedBall = null;
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
        _gameMakerHandler.ResetTubes();
        completionObjects.SetActive(false);
        _moveList.Clear();
    }

    // ONLY FOR USAGE ON THE "RESTART?" BUTTON
    public void RegenFill()
    {
        /*
         * ONLY USE ON THE "RESTART?" BUTTON!!!!!
         */

        // removes the current selected ball
        if (_selectedBall)
        {
            _selectedBall.GetComponent<BallData>().previousTube.GetComponent<TubeHandler>()
                .AddBall(_selectedBall, true);
            _selectedBall = null;
        }

        // basically just "nextLevel" but doesnt increment level counter
        // ^ and uses the previous fill
        _gameMakerHandler.ResetTubes();
        _gameMakerHandler.CreateGame(GetNumberOfTubes());
        _gameMakerHandler.RecreateMostRecentFill();
        _moveCount = 0;

        // fixes #22, ensures extra tube is given if ad completed
        if (extraTube)
        {
            AddTube_AD();
        }
    }

    public void AddTube_AD()
    {
        _gameMakerHandler.ONLYGH_AddTube_AD();
        extraTube = true;
        addTubeButton.gameObject.SetActive(false);
    }

    #region Dialogs

    public void OpenDialog(GameObject dialog)
    {
        if (_inGUI)
        {
            return;
        } 
        
        dialog.SetActive(true);
        _inGUI = true;
    }

    public void CloseDialog(GameObject dialog)
    {
        dialog.SetActive(false);
        _inGUI = false;
    }
    
    #endregion
}