using UnityEngine;

public class ReportBugsButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Open()
    {
        Application.OpenURL("https://github.com/Duve3/ColorSortGame/issues/new?template=bug_report.md");
    }
}
