using UnityEngine;

public class BuildLogCleaner : MonoBehaviour
{
    void Awake()
    {
        
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
    }
}
