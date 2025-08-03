using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TubeHandler : MonoBehaviour
{
    public readonly Stack<GameObject> Balls = new();
    public int size = 5;
    public bool solved;

    public Image checkmarkImage;
    public Canvas canvas;

    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public bool AddBall(GameObject ball, bool? force = null)
    {
        if (solved) { return false; } // we do not care

        if (force == true)
        {
            // basically you are forcing us to push ts on
            Balls.Push(ball);
            MoveBall(ball);
            return true;
        }

        // is the ball actually a ball?
        if (!ball.CompareTag("Ball")) { return false; }

        // we are full!!! (this should anyways be detected by being "disabled"
        if (Balls.Count == size) { return false; }
        
        if (Balls.Count < 1)
        {
            Balls.Push(ball);
            MoveBall(ball);
            return true;
        }

        Color ballColor = ball.GetComponent<SpriteRenderer>().color;
        Color topBallColor = Balls.Peek().GetComponent<SpriteRenderer>().color;

        if (topBallColor == ballColor)
        {
            Balls.Push(ball);
            MoveBall(ball);
            return true;
        }

        return false;
    }

    private void MoveBall(GameObject ball)
    {
        // move ball to the lowest open spot on the tube
        float sum = 0;
        foreach (GameObject b in Balls)
        {
            sum += b.transform.lossyScale.y + 0.05f;
        }

        float height = _renderer.bounds.size.y;

        float y = sum + (transform.position.y - (height / 2)) - (ball.transform.localScale.y / 2) + 0.1f;
        Vector3 newPos = new Vector3(transform.position.x, y, 0);

        ball.GetComponent<AnimationHandler>().AddAnimationToQueue(newPos, 0.25f, true);

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

        foreach (GameObject b in Balls)
        {
            SpriteRenderer cRender = b.GetComponent<SpriteRenderer>();
            cRender.color = new Color(cRender.color.r, cRender.color.g, cRender.color.b, 0.25f);
        }


        // creating the "checkmark" that goes on top of the completed tubes
        float tubeHeight = 0;

        Transform[] cObjs = GetComponentsInChildren<Transform>();
        foreach (Transform child in cObjs)
        {
            tubeHeight += child.localScale.y;
        }

        if (Camera.main is null)
        {
            throw new NullReferenceException("Camera.main is null");
        }
        Vector3 screenCords = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + (tubeHeight / 2f), transform.position.z));

        GameObject dup = Instantiate(checkmarkImage.gameObject, canvas.gameObject.transform);

        dup.transform.position = screenCords;
        
        dup.transform.SetAsFirstSibling();

        dup.SetActive(true);
        
        dup.GetComponent<CheckmarkHandler>().BeginDestroy();
    }

    public void Uncomplete()
    {
        /* 
         * used when redrawing the tube to now be incomplete
         */

        SpriteRenderer[] childObjects = GetComponentsInChildren<SpriteRenderer>();

        // just setting all our colors to normal alpha values!
        foreach (SpriteRenderer cRender in childObjects)
        {
            cRender.color = new Color(cRender.color.r, cRender.color.g, cRender.color.b, 1f);
        }

        foreach (GameObject b in Balls)
        {
            SpriteRenderer cRender = b.GetComponent<SpriteRenderer>();
            cRender.color = new Color(cRender.color.r, cRender.color.g, cRender.color.b, 1f);
        }

        solved = false;
    }

    public bool CheckCompletion()
    {
        if (Balls.Count < size)
        {
            return false;
        }

        Color start = Balls.Peek().GetComponent<SpriteRenderer>().color;
        foreach (GameObject ball in Balls)
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
        if (Balls.Count < 1 || solved)
        {
            GameObject obj = new GameObject();
            Destroy(obj, 0.5f);
            return obj;
        }

        return Balls.Pop();
    }
}
