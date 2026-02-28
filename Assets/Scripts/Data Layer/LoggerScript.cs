using UnityEngine;

public class Logger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isLogging;
    [SerializeField] private bool isFrameLogging;

    /// <summary>
    ///  Logs the message to the console if logging is enabled.
    /// </summary>
    public void Log(object message)
    {
        if (isLogging) {
            Debug.Log($"{this.name}: " + message);
        }
    }

    /// <summary>
    ///  Logs the message to the console if logging is enabled. For logging in methods that will run every frame or spam the console alot.
    /// </summary>
    public void LogFrame(object message)
    {
        if (isFrameLogging) {
            Debug.Log($"{this.name}: " + message);
        }
    }
}
