using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;

    private int _loadLevel;

    void Awake()
    {
        transform.SetAsLastSibling();
    }

    public void FadeToScene(int index)
    {
        _loadLevel = index;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(_loadLevel);
    }
}
