using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private GameObject blackFade;

    private SceneChanger _changer;

    private void Start()
    {
        _changer = blackFade.GetComponent<SceneChanger>();    
    }

    public void PlayGame()
    {
        _changer.FadeToScene(1);
    }
}
