using UnityEngine;

public class AddTubeHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject AddTubeDialog;

    public void OpenDialog()
    {
        AddTubeDialog.SetActive(true);
    }

    public void CloseDialog()
    {
        AddTubeDialog.SetActive(false);
    }
}
