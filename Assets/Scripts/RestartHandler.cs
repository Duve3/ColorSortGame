using UnityEngine;

public class RestartHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject RestartDialog;

    public void OpenDialog()
    {
        RestartDialog.SetActive(true);
    }

    public void CloseDialog()
    {
        RestartDialog.SetActive(false);
    }
}
