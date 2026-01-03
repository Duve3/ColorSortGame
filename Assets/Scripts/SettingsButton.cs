using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject blackFade;

    private SceneChanger m_changer;

    private void Start()
    {
        m_changer = blackFade.GetComponent<SceneChanger>();
    }

    public void OpenSettings()
    {
        m_changer.FadeToScene(2);
    }
}
