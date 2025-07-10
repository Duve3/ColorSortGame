using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;

    private int loadLevel;

    // Update is called once per frame
    void Update()
    {

    }

    public void FadeToScene(int index)
    {
        loadLevel = index;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(loadLevel);
    }
}
