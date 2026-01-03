using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameObject blackFade;

    private SceneChanger m_changer;

    private void Start()
    {
        m_changer = blackFade.GetComponent<SceneChanger>();
    }

    public void PlayGame()
    {
        m_changer.FadeToScene(1);
    }
}
