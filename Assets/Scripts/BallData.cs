using System.Collections;
using UnityEngine;

public class BallData : MonoBehaviour
{
    public GameObject previousTube;

    private bool _shaking;
    
    public AnimationHandler animationHandler;

    private void Start()
    {
        animationHandler =  GetComponent<AnimationHandler>();
    }

    public void ShakeBall()
    {
        StartCoroutine(_ShakeBall());
    }

    private IEnumerator _ShakeBall()
    {
        if (_shaking) { yield break; }

        _shaking = true;

        Vector3 ballLeft = transform.position + new Vector3(-0.15f, 0, 0);
        Vector3 ballRight = transform.position + new Vector3(0.15f, 0, 0);
        Vector3 ballOriginal = transform.position;

        animationHandler.AddAnimationToQueue(ballLeft, 0.05f);
        animationHandler.AddAnimationToQueue(ballRight, 0.05f);
        animationHandler.AddAnimationToQueue(ballOriginal, 0.025f);

        yield return new WaitForSeconds(0.125f);
        _shaking = false;
    }
}
