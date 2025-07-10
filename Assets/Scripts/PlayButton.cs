using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private GameObject BlackFade;

    private SceneChanger Changer;

    void Start()
    {
        Changer = BlackFade.GetComponent<SceneChanger>();    
    }

    public void PlayGame()
    {
        Changer.FadeToScene(1);
    }
}
