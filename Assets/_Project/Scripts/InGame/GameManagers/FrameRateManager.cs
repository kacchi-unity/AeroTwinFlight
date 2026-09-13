using UnityEngine;

public class FrameManager : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = 60;

        

        Debug.Log("System: 타겟 프레임이 60FPS로 고정되었습니다.");
    }
}
