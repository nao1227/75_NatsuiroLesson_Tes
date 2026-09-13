using UnityEngine;

public class TestLogger : MonoBehaviour
{
    public void LogExecuted()
    {
        Debug.Log("★ScriptExecuteEventが発火した!★");
    }

    public void LogCancelled()
    {
        Debug.Log("☆ScriptExecuteEventがキャンセルされた☆");
    }
}