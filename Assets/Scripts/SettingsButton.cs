using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField]
    private GameObject blackFade;

    private SceneChanger _changer;

    private void Start()
    {
        _changer = blackFade.GetComponent<SceneChanger>();
    }

    public void OpenSettings()
    {
        _changer.FadeToScene(2);
    }
}
