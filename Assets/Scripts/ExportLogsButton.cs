using UnityEngine;
using Sych.ShareAssets.Runtime;

public class ExportLogsButton : MonoBehaviour
{
    private LogManager logManager;

    void Start()
    {
        logManager = GameObject.Find("LogManager").GetComponent<LogManager>();
    }

    public void ExportLogs()
    {
        Debug.Log($"Exporting LogFile!");
        if (!Share.IsPlatformSupported)
        {
            Debug.Log("Platform is not supported! (!Share.IsPlatformSupported)");
            return;
        }

        string LogPath = logManager.GetLogFilePath();

        Share.Item(LogPath, success => {
            Debug.Log($"Sharing LogFile was {(success ? "success" : "failed")}");
        });
    }
}
