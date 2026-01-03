using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakerHandler : MonoBehaviour
{
    [SerializeField]
    public int rowLimit = 7;

    [SerializeField]
    private Image checkmarkImage;
    [SerializeField]
    private Canvas canvas;

    public List<GameObject> tubes = new();

    public GameObject tubePrefab;
    public GameObject ballPrefab;

    public GameObject topRowPositioner;
    public GameObject bottomRowPositioner;
    public GameObject extraTubePositioner;

    private List<List<Color>> _mostRecentFill = new();
    private float _bottomRowY;
    private float _topRowY;
    private const float Padding = 20;

    // Start func is here for debugging
    private void Start()
    {
        _bottomRowY = bottomRowPositioner.transform.position.y;
        _topRowY = topRowPositioner.transform.position.y;
    }

    private static void CopyTo(List<Color> list1, List<Color> list2)
    {
        foreach (Color c in list1)
        {
            list2.Add(c);
        }
    }

    private static int GetNumberOfEmptyTubes()
    {
        return 2;
    }

    // unity has its own "reset" thing but i dont trust it here
    public void ResetTubes()
    {
        // destroy each tube
        foreach (GameObject tube in tubes)
        {
            foreach (GameObject ball in tube.GetComponent<TubeHandler>().Balls)
            {
                DestroyImmediate(ball);
            }
           
            DestroyImmediate(tube);
        }

        // reset our list!
        tubes = new List<GameObject>();
    }

    public void CreateGame(int numTubes) {
        int divisor = numTubes;

        if (numTubes > rowLimit)
        {
            divisor = rowLimit;
        }

        float spacingBottom = (Screen.width - Padding) / divisor;

        float spacingTop;
        if (numTubes % rowLimit != 0)
        {
            spacingTop = (Screen.width - Padding) / ((numTubes % rowLimit) + 1);
        } else
        {
            spacingTop = (Screen.width - Padding) / (rowLimit + 1);
        }

        bool top = false;

        Debug.Log("creating ; " + spacingBottom + " ; " + spacingTop + " ; numTubes: " + numTubes);
        for (int i = 0; i < numTubes; i++)
        {
            GameObject obj = Instantiate(tubePrefab);

            // allows us to pass in this "checkmark" image & canvas
            obj.GetComponent<TubeHandler>().checkmarkImage = checkmarkImage;
            obj.GetComponent<TubeHandler>().canvas = canvas;

            tubes.Add(obj);

            int truei = i;
            float y = _bottomRowY;

            if (Mathf.Floor(i / rowLimit) > 0)
            {
                top = true;
                truei = i % rowLimit;
                y = _topRowY;
            }

            y += (obj.transform.localScale.y);

            float x;
            if (!top)
            {
                x = (Padding / 2) + (spacingBottom / 2) + (spacingBottom * truei);
            } else
            {
                x = (Padding / 2) + (spacingTop / 2) + (spacingTop * truei);
            }

            Debug.Log("x: " + x + " ; truei: " + truei + " ; y: " + y);
            obj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, 0, 0));

            // fix z value (ensures that its 0) and put in y value, (y is now WORLD Pos not screen pos)
            obj.transform.position = new Vector3(obj.transform.position.x, y, 0);
        }
    }

    public void GenerateFill(List<Color> oldColors)
    {
        /*
         * Generate the "fill" or how to setup the tubes depending on the 
         * amount of balls 
         */
        // has to be done because we do destructive stuff with Colors
        List<Color> colors = new() { };
        CopyTo(oldColors, colors);

        _mostRecentFill = new List<List<Color>>();

        int numColors = colors.Count;

        int emptyTube = GetNumberOfEmptyTubes();

        if (numColors >= tubes.Count - emptyTube)
        {
            // removes all colors beyond the range of amount of tubes
            Debug.Log("Truncating Colors!");
            colors.RemoveRange(tubes.Count - emptyTube, colors.Count - (tubes.Count - emptyTube));
            Debug.Log("New colors: " + colors.Count);
            Debug.Log("\t" + string.Join(", ", colors));
        }
        int tubeLimit = tubes[0].GetComponent<TubeHandler>().size;

        Dictionary<Color, int> dictionary = new();
        foreach (Color c in colors)
        {
            dictionary.Add(c, 0);
        }


        for (int j = 0; j < tubes.Count - emptyTube; j++)
        {
            GameObject tube = tubes[j];
            TubeHandler t_h = tube.GetComponent<TubeHandler>();
            for (int i = 0; i < tubeLimit; i++)
            {
                GameObject newBall = Instantiate(ballPrefab);
                Color chosenColor = colors[Random.Range(0, colors.Count)];

                if (colors.Count == tubes.Count - emptyTube)
                {
                    while (dictionary[chosenColor] >= tubeLimit)
                    {
                        chosenColor = colors[Random.Range(0, colors.Count)];
                    }
                    dictionary[chosenColor] += 1;
                } else
                {
                    Debug.LogWarning("WARNING: Maybe impossible sort! (# of colors!)");
                }

                newBall.GetComponent<SpriteRenderer>().color = chosenColor;
                t_h.AddBall(newBall, true);
            }

            //Debug.Log("Tube before completion check: " + 
            //    string.Join(", ", t_h.balls.Select(obj => obj.GetComponent<SpriteRenderer>().color)));

            // while the tube is completed, keep on running this loop
            while (t_h.CheckCompletion())
            {
                // this right here fixes the bug of certain colors not existing in a dictionary
                // below just "strips" the alpha value from the color we find
                // (fixes #14)
                Color ballColor = t_h.Balls.Peek().GetComponent<SpriteRenderer>().color;
                Color properColor = new Color(ballColor.r, ballColor.g, ballColor.b, 1f);
                
                dictionary[properColor] -= tubeLimit;

                // clear out our list from balls
                foreach (GameObject b in t_h.Balls)
                {
                    t_h.PopBall();
                }

                // just rerun the above code now, hopefully it works correctly this time!!
                for (int i = 0; i < tubeLimit; i++)
                {
                    GameObject newBall = Instantiate(ballPrefab);
                    Color chosenColor = colors[Random.Range(0, colors.Count)];

                    if (colors.Count == tubes.Count - emptyTube)
                    {
                        while (dictionary[chosenColor] >= tubeLimit)
                        {
                            chosenColor = colors[Random.Range(0, colors.Count)];
                        }
                        dictionary[chosenColor] += 1;
                    }
                    else
                    {
                        Debug.LogWarning("WARNING: Maybe impossible sort! (# of colors!)");
                    }

                    newBall.GetComponent<SpriteRenderer>().color = chosenColor;
                    t_h.AddBall(newBall, true);
                }
            }

            //Debug.Log("Tube AFTER completion check: " +
            //    string.Join(", ", t_h.balls.Select(obj => obj.GetComponent<SpriteRenderer>().color)));


            List<Color> tFill = new();
            foreach (GameObject ball in t_h.Balls)
            {
                tFill.Add(ball.GetComponent<SpriteRenderer>().color);
            }

            tFill.Reverse();
            _mostRecentFill.Add(tFill);
        }

        string mrf = "";
        foreach (List<Color> lc in _mostRecentFill)
        {
            mrf += "(" + string.Join(", ", lc) + "), ";
        }
        Debug.Log("MOST RECENT FILL WRITE: " + mrf);
    }

    public void RecreateMostRecentFill()
    {
        int emptyTubes = GetNumberOfEmptyTubes();


        string mrf = "";
        foreach (List<Color> lc in _mostRecentFill)
        {
            mrf += "(" + string.Join(", ", lc) + "), ";
        }
        Debug.Log("MOST RECENT FILL READ: " + mrf);


        for (var j = 0; j < tubes.Count - emptyTubes; j++)
        {
            GameObject tube = tubes[j];
            TubeHandler t_h = tube.GetComponent<TubeHandler>();

            List<Color> tFill = _mostRecentFill[j];

            foreach (Color c in tFill)
            {
                GameObject ball = Instantiate(ballPrefab);

                ball.GetComponent<SpriteRenderer>().color = c;

                t_h.AddBall(ball, true);
            }
        }
    }

    public void ONLYGH_AddTube_AD()
    {
        GameObject obj = Instantiate(tubePrefab);

        // allows us to pass in this "checkmark" image & canvas
        obj.GetComponent<TubeHandler>().checkmarkImage = checkmarkImage;
        obj.GetComponent<TubeHandler>().canvas = canvas;

        obj.GetComponent<SpriteRenderer>().color = new Color(0f / 255f, 160f / 255f, 0f / 255f);

        tubes.Add(obj);

        float spacingTop = (Screen.width - Padding) / (rowLimit + 1);

        float y = extraTubePositioner.transform.position.y + obj.transform.localScale.y;
        float x = (Padding / 2) + (spacingTop / 2) + (spacingTop * rowLimit);

        float xWorldCoords = Camera.main.ScreenToWorldPoint(new Vector3(x, 0, 0)).x;

        // fix z value (ensures that its 0) and put in y value, (y is now WORLD Pos not screen pos)
        obj.transform.position = new Vector3(xWorldCoords, y, 0);
    }

    public void GenerateFromFillData(List<List<Color>> fillData)
    {
        for (int i = 0; i < fillData.Count; i++)
        {
            GameObject tube = tubes[i];
            TubeHandler t_h = tube.GetComponent<TubeHandler>();
            for (int j = 0; j < fillData[i].Count; j++)
            {
                GameObject ball = Instantiate(ballPrefab);
                
                ball.GetComponent<SpriteRenderer>().color = fillData[i][j];
                t_h.AddBall(ball, true);
            }
        }
    }
}
