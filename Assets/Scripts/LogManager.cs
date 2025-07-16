using UnityEngine;
using System.IO;

public class LogManager : MonoBehaviour
{
    private string logFilePath;

    private string version;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        logFilePath = Path.Combine(Application.persistentDataPath, "log.txt");


        // get version data
        string VersionFile = Path.Combine(Application.streamingAssetsPath, "VERSION.txt");

        bool Exists = File.Exists(VersionFile);
        if (!Exists)
        {
            File.WriteAllText(logFilePath, $"FAILED TO FIND VERSION FILE!!!");
            Debug.Log($"FAILED TO FIND VERSION FILE!!!");
        }

        version = File.ReadAllText(VersionFile);


        // write app start and important info
        string dInfo = $"Device: {SystemInfo.deviceModel}\n" +
                       $"Platform: {Application.platform}\n" +
                       $"Unity version: {Application.unityVersion}\n" +
                       $"Game version: {version}\n";

        File.WriteAllText(logFilePath, $"{System.DateTime.Now} [Log] APP START\n");
        File.WriteAllText(logFilePath, dInfo);
        Debug.Log(dInfo);

        // This catches all Debug.Logs
        Application.logMessageReceived += HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
        {
        string logEntry = $"{System.DateTime.Now} [{type}] {logString}";

        if (type == LogType.Error || type == LogType.Exception)
        {
            logEntry += "\n" + stackTrace;
        }

        File.AppendAllText(logFilePath, logEntry + "\n");
    }

    public string GetLogFilePath()
    {
        return logFilePath;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
