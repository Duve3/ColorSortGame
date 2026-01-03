using UnityEngine;

public class ReportBugsButton : MonoBehaviour
{
    public void Open()
    {
        Application.OpenURL("https://github.com/Duve3/ColorSortGame/issues/new?template=bug_report.md");
    }
}
