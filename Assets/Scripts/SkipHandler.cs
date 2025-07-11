using UnityEngine;

public class SkipHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject SkipDialog;

    public void OpenDialog()
    {
        SkipDialog.SetActive(true);
    }

    public void CloseDialog()
    {
        SkipDialog.SetActive(false);
    }
}
