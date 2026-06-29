using UnityEngine;

public class FlightRotatioController : MonoBehaviour
{
    private void OnEnable()
    {
        //MPU6050DataConverter.OnDataCalibrated += 
    }

    private void OnDisable()
    {
        
    }

    void Update() //테스트용 삭제 예정
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("");
        }
    }
}
