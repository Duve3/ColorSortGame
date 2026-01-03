using UnityEngine;
using Sych.ShareAssets.Runtime;

public class ExportLogsButton : MonoBehaviour
{
    private LogManager m_logManager;

    [SerializeField]
    private PopupToast popupToast;

    private void Awake()
    {
        var logManagerObject = GameObject.Find("LogManagerObject");
        if (logManagerObject != null)
        {
            m_logManager = logManagerObject.GetComponent<LogManager>();
        }
    }

    public void ExportLogs()
    {
        Debug.Log("Exporting LogFile!");
        if (!Share.IsPlatformSupported)
        {
            Debug.Log("Platform is not supported! (!Share.IsPlatformSupported)");
            if (popupToast != null)
                StartCoroutine(popupToast.ShowToast("Platform not supported! (Report this!)", 2f));
            return;
        }

        if (m_logManager == null)
        {
            Debug.Log("LogManager is null!");
            if (popupToast != null)
                StartCoroutine(popupToast.ShowToast("Failed to find log manager! (Report this!)", 2f));
            return;
        }

        string logPath = m_logManager.GetLogFilePath();

        Share.Item(logPath, success => {
            Debug.Log($"Sharing LogFile was a {(success ? "success" : "failure")}");
            if (!success && popupToast != null)
                StartCoroutine(popupToast.ShowToast("Failed to share log file! (Report this!)", 2f));
        });
    }
}
