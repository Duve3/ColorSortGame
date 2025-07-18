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

        // app start and important info
        string dInfo = $"{System.DateTime.Now} [Log] APP START\n" +
                       $"\tDevice: {SystemInfo.deviceModel}\n" +
                       $"\tPlatform: {Application.platform}\n" +
                       $"\tUnity version: {Application.unityVersion}\n" +
                       $"\tGame version: {Application.version}";

        File.AppendAllText(logFilePath, dInfo + "\n");
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
