using System.Collections.Generic;
using System;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private RectTransform _rectTransform;

    private readonly Queue<Tuple<Vector2, float>> _animationQueue = new();

    private Tuple<Vector2, float> _currentAnimation = new(Vector2.zero, -1f);
    private float _totalTime;
    private Vector2 _mps;
    private bool _emptyQueue;
    private bool _isAnimating;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!_isAnimating)
        {
            if (_animationQueue.Count > 0)
            {
                _currentAnimation = _animationQueue.Dequeue();
                _mps = _currentAnimation.Item1 - _rectTransform.anchoredPosition;
                _mps /= _currentAnimation.Item2;
                _isAnimating = true;
            }
            else
            {
                return;
            }
        }

        if (_emptyQueue)
        {
            // End current animation immediately
            _rectTransform.anchoredPosition = _currentAnimation.Item1;

            // Flush remaining queue except the last item
            for (int i = 0; i < _animationQueue.Count - 1; i++)
            {
                Tuple<Vector2, float> obj = _animationQueue.Dequeue();
                _rectTransform.anchoredPosition = obj.Item1;
            }

            _totalTime = 0f;
            _currentAnimation = new Tuple<Vector2, float>(Vector2.zero, -1f);
            _isAnimating = false;
            _emptyQueue = false;
            return;
        }

        float dt = Time.deltaTime;
        _totalTime += dt;

        _rectTransform.anchoredPosition += _mps * dt;

        if (_totalTime < _currentAnimation.Item2) { return; }

        // Animation finished
        _rectTransform.anchoredPosition = _currentAnimation.Item1;
        _totalTime = 0f;
        _currentAnimation = new Tuple<Vector2, float>(Vector2.zero, -1f);
        _isAnimating = false;
    }

    public void AddAnimationToQueue(Vector2 endPos, float time, bool? force = null)
    {
        _animationQueue.Enqueue(new Tuple<Vector2, float>(endPos, time));

        if (force == true && _animationQueue.Count > 1)
        {
            _emptyQueue = true;
        }
    }
}