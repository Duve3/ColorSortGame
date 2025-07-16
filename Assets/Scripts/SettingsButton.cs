using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField]
    private GameObject BlackFade;

    private SceneChanger Changer;

    void Start()
    {
        Changer = BlackFade.GetComponent<SceneChanger>();
    }

    public void OpenSettings()
    {
        Changer.FadeToScene(2);
    }
}
