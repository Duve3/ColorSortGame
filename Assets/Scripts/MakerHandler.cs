using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakerHandler : MonoBehaviour
{
    [SerializeField]
    public int RowLimit = 5;

    [SerializeField]
    private Image CheckmarkImage;
    [SerializeField]
    private Canvas Canvas;

    public List<GameObject> Tubes = new List<GameObject>();

    public GameObject TubePrefab;
    public GameObject BallPrefab;

    public GameObject TopRowPositioner;
    public GameObject BottomRowPositioner;
    public GameObject ExtraTubePositioner;

    public List<List<Color>> MostRecentFill = new();

    private float bottomRowY;
    private float topRowY;
    private float padding = 20;

    // Start func is here for debugging
    void Start()
    {
        bottomRowY = BottomRowPositioner.transform.position.y;
        topRowY = TopRowPositioner.transform.position.y;
    }

    private void CopyTo(List<Color> List1, List<Color> List2)
    {
        foreach (Color c in List1)
        {
            List2.Add(c);
        }
    }

    public int GetNumberOfEmptyTubes(int TubeCount)
    {
        if (TubeCount <= 4)
        {
            return 1;
        }

        return 2;
    }

    // unity has its own "reset" thing but i dont trust it here
    public void ResetTubes()
    {
        // destroy each tube
        foreach (GameObject tube in Tubes)
        {
            foreach (GameObject ball in tube.GetComponent<TubeHandler>().balls)
            {
                DestroyImmediate(ball);
            }
           
            DestroyImmediate(tube);
        }

        // reset our list!
        Tubes = new List<GameObject>();
    }

    public void CreateGame(int numTubes) {
        int divisor = numTubes;

        if (numTubes > RowLimit)
        {
            divisor = RowLimit;
        }

        float spacingBottom = (Screen.width - padding) / divisor;

        float spacingTop;
        if (numTubes % RowLimit != 0)
        {
            spacingTop = (Screen.width - padding) / ((numTubes % RowLimit) + 1);
        } else
        {
            spacingTop = (Screen.width - padding) / (RowLimit + 1);
        }

        bool top = false;

        Debug.Log("creating ; " + spacingBottom + " ; " + spacingTop + " ; numTubes: " + numTubes);
        for (int i = 0; i < numTubes; i++)
        {
            GameObject obj = Instantiate(TubePrefab);

            // allows us to pass in this "checkmark" image & canvas
            obj.GetComponent<TubeHandler>().CheckmarkImage = CheckmarkImage;
            obj.GetComponent<TubeHandler>().Canvas = Canvas;

            Tubes.Add(obj);

            int truei = i;
            float y = bottomRowY;

            if (Mathf.Floor(i / RowLimit) > 0)
            {
                top = true;
                truei = i % RowLimit;
                y = topRowY;
            }

            y += (obj.transform.localScale.y);

            float x;
            if (!top)
            {
                x = (padding / 2) + (spacingBottom / 2) + (spacingBottom * truei);
            } else
            {
                x = (padding / 2) + (spacingTop / 2) + (spacingTop * truei);
            }

            Debug.Log("x: " + x + " ; truei: " + truei + " ; y: " + y);
            obj.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, 0, 0));

            // fix z value (ensures that its 0) and put in y value, (y is now WORLD Pos not screen pos)
            obj.transform.position = new Vector3(obj.transform.position.x, y, 0);
        }
    }

    public void GenerateFill(List<Color> OldColors)
    {
        /*
         * Generate the "fill" or how to setup the tubes depending on the 
         * amount of balls 
         */
        // has to be done because we do destructive stuff with Colors
        List<Color> Colors = new() { };
        CopyTo(OldColors, Colors);

        MostRecentFill = new List<List<Color>>();

        int numColors = Colors.Count;

        int emptyTube = GetNumberOfEmptyTubes(Tubes.Count);

        if (numColors >= Tubes.Count - emptyTube)
        {
            // removes all colors beyond the range of amount of tubes
            Debug.Log("Truncating Colors!");
            Colors.RemoveRange(Tubes.Count - emptyTube, Colors.Count - (Tubes.Count - emptyTube));
            Debug.Log("New colors: " + Colors.Count);
            Debug.Log("\t" + string.Join(", ", Colors));
        }
        int tubeLimit = Tubes[0].GetComponent<TubeHandler>().size;

        Dictionary<Color, int> dictionary = new();
        foreach (Color c in Colors)
        {
            dictionary.Add(c, 0);
        }


        for (int j = 0; j < Tubes.Count - emptyTube; j++)
        {
            GameObject tube = Tubes[j];
            TubeHandler t_h = tube.GetComponent<TubeHandler>();
            for (int i = 0; i < tubeLimit; i++)
            {
                GameObject newBall = Instantiate(BallPrefab);
                Color chosenColor = Colors[Random.Range(0, Colors.Count)];

                if (Colors.Count == Tubes.Count - emptyTube)
                {
                    while (dictionary[chosenColor] >= tubeLimit)
                    {
                        chosenColor = Colors[Random.Range(0, Colors.Count)];
                    }
                    dictionary[chosenColor] += 1;
                } else
                {
                    Debug.LogWarning("WARNING: Maybe impossible sort! (# of colors!)");
                }

                newBall.GetComponent<SpriteRenderer>().color = chosenColor;
                t_h.AddBall(newBall, true);
            }

            Debug.Log("Tube before completion check: " + 
                string.Join(", ", t_h.balls.Select(obj => obj.GetComponent<SpriteRenderer>().color)));

            if (t_h.CheckCompletion())
            {
                dictionary[t_h.balls.Peek().GetComponent<SpriteRenderer>().color] -= tubeLimit;

                // clear out our list from balls
                foreach (GameObject b in t_h.balls)
                {
                    t_h.PopBall();
                }

                // just rerun the above code now, hopefully it works correctly this time!!
                for (int i = 0; i < tubeLimit; i++)
                {
                    GameObject newBall = Instantiate(BallPrefab);
                    Color chosenColor = Colors[Random.Range(0, Colors.Count)];

                    if (Colors.Count == Tubes.Count - emptyTube)
                    {
                        while (dictionary[chosenColor] >= tubeLimit)
                        {
                            chosenColor = Colors[Random.Range(0, Colors.Count)];
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

            Debug.Log("Tube AFTER completion check: " +
                string.Join(", ", t_h.balls.Select(obj => obj.GetComponent<SpriteRenderer>().color)));


            List<Color> tFill = new();
            foreach (GameObject ball in t_h.balls)
            {
                tFill.Add(ball.GetComponent<SpriteRenderer>().color);
            }

            tFill.Reverse();
            MostRecentFill.Add(tFill);
        }

        string mrf = "";
        foreach (List<Color> lc in MostRecentFill)
        {
            mrf += "(" + string.Join(", ", lc) + "), ";
        }
        Debug.Log("MOST RECENT FILL WRITE: " + mrf);
    }

    public void RecreateMostRecentFill()
    {
        int emptyTubes = GetNumberOfEmptyTubes(Tubes.Count);


        string mrf = "";
        foreach (List<Color> lc in MostRecentFill)
        {
            mrf += "(" + string.Join(", ", lc) + "), ";
        }
        Debug.Log("MOST RECENT FILL READ: " + mrf);


        for (var j = 0; j < Tubes.Count - emptyTubes; j++)
        {
            GameObject tube = Tubes[j];
            TubeHandler t_h = tube.GetComponent<TubeHandler>();

            List<Color> tFill = MostRecentFill[j];

            foreach (Color c in tFill)
            {
                GameObject ball = Instantiate(BallPrefab);

                ball.GetComponent<SpriteRenderer>().color = c;

                t_h.AddBall(ball, true);
            }
        }
    }

    public void ONLYGH_AddTube_AD()
    {
        GameObject obj = Instantiate(TubePrefab);

        // allows us to pass in this "checkmark" image & canvas
        obj.GetComponent<TubeHandler>().CheckmarkImage = CheckmarkImage;
        obj.GetComponent<TubeHandler>().Canvas = Canvas;

        obj.GetComponent<SpriteRenderer>().color = new Color(0f / 255f, 160f / 255f, 0f / 255f);

        Tubes.Add(obj);

        float y = ExtraTubePositioner.transform.position.y + obj.transform.localScale.y;
        float x = ExtraTubePositioner.transform.position.x;

        // fix z value (ensures that its 0) and put in y value, (y is now WORLD Pos not screen pos)
        obj.transform.position = new Vector3(x, y, 0);
    }
}
