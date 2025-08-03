using UnityEngine;
using System.IO;

public class LogManager : MonoBehaviour
{
    private string _logFilePath;

    private string _version;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _logFilePath = Path.Combine(Application.persistentDataPath, "log.txt");

        // app start and important info
        string dInfo = $"{System.DateTime.Now} [Log] APP START\n" +
                       $"\tDevice: {SystemInfo.deviceModel}\n" +
                       $"\tPlatform: {Application.platform}\n" +
                       $"\tUnity version: {Application.unityVersion}\n" +
                       $"\tGame version: {Application.version}";

        File.AppendAllText(_logFilePath, dInfo + "\n");
        Debug.Log(dInfo);

        // This catches all Debug.Logs
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
        {
        string logEntry = $"{System.DateTime.Now} [{type}] {logString}";

        if (type is LogType.Error or LogType.Exception)
        {
            logEntry += "\n" + stackTrace;
        }

        File.AppendAllText(_logFilePath, logEntry + "\n");
    }

    public string GetLogFilePath()
    {
        return _logFilePath;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
