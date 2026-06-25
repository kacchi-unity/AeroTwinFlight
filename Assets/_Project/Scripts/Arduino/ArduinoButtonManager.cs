using UnityEngine;

public class ArduinoButtonManager : MonoBehaviour
{
    private void OnEnable()
    {
        SensorManager.OnButtonDown += ProcessButtonPress;
    }

    private void OnDisable()
    {
        SensorManager.OnButtonDown -= ProcessButtonPress;
    }

    void ProcessButtonPress()
    {
        Debug.Log("버튼 클릭 감지");
    }
}
