using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallData : MonoBehaviour
{
    public GameObject previousTube;

    private bool shaking = false;

    public void ShakeBall()
    {
        StartCoroutine(bShakeBall());
    }

    private IEnumerator bShakeBall()
    {
        if (shaking) { yield break; }

        shaking = true;

        Vector3 ballLeft = transform.position + new Vector3(0.1f, 0, 0);
        Vector3 ballRight = transform.position + new Vector3(-0.1f, 0, 0);
        Vector3 ballOriginal = transform.position;

        iTween.MoveTo(gameObject, ballLeft, 0.1f);
        yield return new WaitForSeconds(0.1f);

        iTween.MoveTo(gameObject, ballRight, 0.1f);
        yield return new WaitForSeconds(0.1f);

        iTween.MoveTo(gameObject, ballOriginal, 0.1f);
        yield return new WaitForSeconds(0.1f);

        shaking = false;
    }
}
