using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakerHandler : MonoBehaviour
{
    [SerializeField]
    private int RowLimit = 5;

    [SerializeField]
    private Image CheckmarkImage;
    [SerializeField]
    private Canvas Canvas;

    public List<GameObject> Tubes = new List<GameObject>();

    public GameObject TubePrefab;
    public GameObject BallPrefab;

    public GameObject TopRowPositioner;
    public GameObject BottomRowPositioner;

    private float bottomRowY;
    private float topRowY;

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

        float spacing = Screen.width / divisor;

        Debug.Log("creating ; " + spacing);
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
                truei = i % RowLimit;
                y = topRowY;
            }

            float x = (spacing / 2) + (spacing * truei);
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

        int numColors = Colors.Count;

        int emptyTube = 1;

        if (Tubes.Count > 4)
        {
            emptyTube = 2;
        }

        if (numColors >= Tubes.Count - emptyTube)
        {
            // removes all colors beyond the range of amount of tubes
            Debug.Log("Truncating Colors!");
            Colors.RemoveRange(Tubes.Count - emptyTube, Colors.Count - (Tubes.Count - emptyTube));
            Debug.Log("New colors: " + Colors.Count);
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
                tube.GetComponent<TubeHandler>().AddBall(newBall, true);
            }
            
        }

    }
}
