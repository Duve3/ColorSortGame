using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TubeHandler : MonoBehaviour
{
    public Stack<GameObject> balls = new Stack<GameObject>();
    public int size = 2;
    public bool solved = false;

    public Image CheckmarkImage;
    public Canvas Canvas;

    public bool AddBall(GameObject ball, bool? force = null)
    {
        if (solved) { return false; } // we do not care

        if (force == true)
        {
            // basically you are forcing us to push ts on
            balls.Push(ball);
            MoveBall(ball);
            return true;
        }

        // is the ball actually a ball?
        if (!ball.CompareTag("Ball")) { return false; }

        // we are full!!! (this should anyways be detected by being "disabled"
        if (balls.Count == size) { return false; }
        
        if (balls.Count < 1)
        {
            balls.Push(ball);
            MoveBall(ball);
            return true;
        }

        Color ballColor = ball.GetComponent<SpriteRenderer>().color;
        Color topBallColor = balls.Peek().GetComponent<SpriteRenderer>().color;

        if (topBallColor == ballColor)
        {
            balls.Push(ball);
            MoveBall(ball);
            return true;
        }

        return false;
    }

    private void MoveBall(GameObject ball)
    {
        // move ball to the lowest open spot on the tube
        float sum = 0;
        foreach (GameObject b in balls)
        {
            sum += b.transform.lossyScale.y + 0.1f;
        }

        float y = sum + transform.position.y - (ball.transform.localScale.y / 2);
        Vector3 newPos = new Vector3(transform.position.x, y, 0);

        iTween.MoveTo(ball, newPos, 0.5f);

        bool completion = CheckCompletion();

        if (!completion)
        {
            return;
        }

        // tube is complete do not modify anymore
        solved = true;

        DrawCompletion();
    }

    public void DrawCompletion()
    {
        SpriteRenderer[] childObjects = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer cRender in childObjects)
        {
            cRender.color = new Color(cRender.color.r, cRender.color.g, cRender.color.b, 0.25f);
        }

        foreach (GameObject b in balls)
        {
            SpriteRenderer cRender = b.GetComponent<SpriteRenderer>();
            cRender.color = new Color(cRender.color.r, cRender.color.g, cRender.color.b, 0.25f);
        }


        // creating the "checkmark" that goes on top of the completed tubes
        float tube_height = 0;

        Transform[] cObjs = GetComponentsInChildren<Transform>();
        foreach (Transform child in cObjs)
        {
            tube_height += child.localScale.y;
        }

        Vector3 ScreenCords = Camera.main.WorldToScreenPoint(new(transform.position.x, transform.position.y + (tube_height / 2f), transform.position.z));

        GameObject dup = Instantiate(CheckmarkImage.gameObject);

        dup.transform.position = ScreenCords;

        dup.transform.SetParent(Canvas.gameObject.transform);

        dup.SetActive(true);

        
        dup.GetComponent<CheckmarkHandler>().BeginDestroy();
    }

    private bool CheckCompletion()
    {
        if (balls.Count < size)
        {
            return false;
        }

        Color start = balls.Peek().GetComponent<SpriteRenderer>().color;
        foreach (GameObject ball in balls)
        {
            if (ball.GetComponent<SpriteRenderer>().color != start)
            {
                return false;
            }
        }

        return true;
    }

    public GameObject PopBall()
    {
        if (balls.Count < 1 || solved)
        {
            GameObject obj = new GameObject();
            Destroy(obj, 0.5f);
            return obj;
        }

        return balls.Pop();
    }
}
