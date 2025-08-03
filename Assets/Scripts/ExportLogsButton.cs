using System.Collections;
using UnityEngine;
using Sych.ShareAssets.Runtime;
using TMPro;

public class ExportLogsButton : MonoBehaviour
{
    private LogManager _logManager;

    [SerializeField]
    private TMP_Text errorText;

    private void Start()
    {
        Debug.Log("ErrorText: " + errorText);
        try
        {
            _logManager = GameObject.Find("LogManagerObject").GetComponent<LogManager>();
        } catch
        {
            Debug.Log("Could not find log manager, (GameObject.Find(`LogManagerObject`))");
            errorText.text = "Failed to find log manager!";
            StartCoroutine(OperationFailed(0.2f, 0.05f));
        }
    }

    public void ExportLogs()
    {
        Debug.Log("Exporting LogFile!");
        if (!Share.IsPlatformSupported)
        {
            Debug.Log("Platform is not supported! (!Share.IsPlatformSupported)");
            StartCoroutine(OperationFailed(0.2f, 0.05f));
            return;
        }

        string logPath = _logManager.GetLogFilePath();

        Share.Item(logPath, success => {
            Debug.Log($"Sharing LogFile was a {(success ? "success" : "failure")}");
        });
    }

    private IEnumerator OperationFailed(float wait, float increment)
    {
        // makes it visible
        errorText.gameObject.SetActive(true);

        // for simplicity
        Color c = errorText.color;

        while (errorText.color.a > 0) {
            errorText.color = new Color(c.r, c.g, c.b, errorText.color.a - increment);
            yield return new WaitForSeconds(wait);
        }

        errorText.gameObject.SetActive(false);
        errorText.color = c;
    }
}
