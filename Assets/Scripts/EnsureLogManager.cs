using UnityEngine;

public class EnsureLogManager : MonoBehaviour
{
    void Awake()
    {
        // Check if LogManager already exists
        if (FindAnyObjectByType<LogManager>() == null)
        {
            GameObject logObj = new("LogManagerObject");
            logObj.AddComponent<LogManager>();
            // now we kill ourselves because we arent needed
            DestroyImmediate(this.gameObject);
        }
    }
}
