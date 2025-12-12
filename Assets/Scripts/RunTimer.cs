using UnityEngine;

public class RunTimer : MonoBehaviour
{
    public static RunTimer Instance { get; private set; }
    private float startTime;

    private void Awake()
    {
        Debug.Log("RunTimer Awake at: " + Time.realtimeSinceStartup);
        Instance = this;
        startTime = Time.realtimeSinceStartup;
    }


    public float GetElapsedSeconds()
    {
        return Time.realtimeSinceStartup - startTime;
    }
}

