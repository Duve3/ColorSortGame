using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private readonly Queue<Tuple<Vector3, float>> _animationQueue = new();

    private Tuple<Vector3, float> _currentAnimation = new(Vector3.zero, -1f);
    private float _totalTime;
    private Vector3 _mps;

    private bool _emptyQueue;
    
    private void Update()
    {
        if (_currentAnimation.Item1 == Vector3.zero)
        {
            if (_animationQueue.Count > 0)
            {
                _currentAnimation = _animationQueue.Dequeue();
                
                // we are finding our the difference between our two points (aka the TOTAL movement we would have to do to reach there)
                _mps = _currentAnimation.Item1 - transform.position;
                _mps /= _currentAnimation.Item2;
            }
            else
            {
                return;
            }
        }

        if (_emptyQueue)
        {
            // end current animation IMMEDIATELY
            transform.position = _currentAnimation.Item1;
            
            // clear out our queue (excluding the just added item
            for (int i = 0; i < _animationQueue.Count - 1; i++)
            {
                var obj = _animationQueue.Dequeue();

                transform.position = obj.Item1;
            }
            
            // reset our data and return so that a new iter begins
            _totalTime = 0f;
            _currentAnimation = new Tuple<Vector3, float>(Vector3.zero, -1f);
            _emptyQueue = false;
            return;
        }

        float dt = Time.deltaTime;

        _totalTime += dt;
        
        transform.position += _mps * dt;

        if (_totalTime < _currentAnimation.Item2) { return; }
        
        // animation must be finished if _totalTime is > or = to our total time (Item2)
        transform.position = _currentAnimation.Item1;
        _totalTime = 0f;
        _currentAnimation = new Tuple<Vector3, float>(Vector3.zero, -1f);
    }

    public void AddAnimationToQueue(Vector3 endPos, float time, bool? force = null)
    {
        _animationQueue.Enqueue(new Tuple<Vector3, float>(endPos, time));

        if (force == true && _animationQueue.Count > 1)
        {
            // otherwise we don't really need to empty the queue
            _emptyQueue = true;
        }
    }
}
