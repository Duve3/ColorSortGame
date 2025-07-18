using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sych.ShareAssets.Runtime;
using TMPro;
using UnityEngine.UI;

public class ExportLogsButton : MonoBehaviour
{
    private LogManager logManager;

    [SerializeField]
    private TMP_Text ErrorText;

    void Start()
    {
        Debug.Log("ErrorText: " + ErrorText);
        logManager = GameObject.Find("LogManager").GetComponent<LogManager>();
    }

    public void ExportLogs()
    {
        Debug.Log($"Exporting LogFile!");
        if (!Share.IsPlatformSupported)
        {
            Debug.Log("Platform is not supported! (!Share.IsPlatformSupported)");
            StartCoroutine(OperationFailed(0.2f, 0.05f));
            return;
        }

        string LogPath = logManager.GetLogFilePath();

        Share.Item(LogPath, success => {
            Debug.Log($"Sharing LogFile was {(success ? "success" : "failed")}");
        });
    }

    IEnumerator OperationFailed(float wait, float increment)
    {
        // makes it visible
        ErrorText.gameObject.SetActive(true);

        // for simplicity
        Color c = ErrorText.color;

        while (ErrorText.color.a > 0) {
            ErrorText.color = new Color(c.r, c.g, c.b, ErrorText.color.a - increment);
            yield return new WaitForSeconds(wait);
        }

        ErrorText.gameObject.SetActive(false);
        ErrorText.color = c;
    }
}
