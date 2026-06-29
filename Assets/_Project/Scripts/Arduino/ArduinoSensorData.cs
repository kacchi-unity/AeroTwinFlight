// ====================================================================
// * 아두이노 통신 프로토콜 규칙
// * 아두이노 전송 형태: "데이터1,데이터2, ... \n"
// (ex: "512,1\n")
// 
// index [0] : x축 각속도 (short)
// index [1] : y축 각속도 (short)
// index [2] : z축 각속도 (short)

// index [3] : x축 각속도 (short)
// index [4] : y축 각속도 (short)
// index [5] : z축 각속도 (short)

// index [6] : 버튼 데이터 (bool)
// ====================================================================

using UnityEngine;

public struct ArduinoSensorData
{
    //Data index enum
    private enum DataIndex
    {
        AccelX = 0,
        AccelY = 1,
        AccelZ = 2,

        GyroX = 3,
        GyroY = 4,
        GyroZ = 5,

        IsButtonPressed = 6,

        TotalDataCount //Auto count
    }

    //Data field
    public short accelX;
    public short accelY;
    public short accelZ;

    public short gyroX;
    public short gyroY;
    public short gyroZ;

    public bool isButtonPressed;

    //Data method: int.TryParse로 검증된 rawDatas를 받아 구조체 데이테를 완성시키는 메서드
    public static ArduinoSensorData ParseData(int[] rawDatas)
    {
        //Check rawDatas
        if (rawDatas == null || rawDatas.Length < (int)DataIndex.TotalDataCount)
        {
            Debug.LogError($"[ArduinoSensorData] 파싱 실패: 데이터 개수가 부족합니다." +
                $"(기대: {(int)DataIndex.TotalDataCount}개, 입력: {rawDatas?.Length ?? 0}개)");

            //return default struct
            return default;
        }

        ArduinoSensorData data = new ArduinoSensorData();

        data.accelX = (short)rawDatas[(int)DataIndex.AccelX];

        data.accelY = (short)rawDatas[(int)DataIndex.AccelY];

        data.accelZ = (short)rawDatas[(int)DataIndex.AccelZ];

        data.gyroX = (short)rawDatas[(int)DataIndex.GyroX];

        data.gyroY = (short)rawDatas[(int)DataIndex.GyroY];

        data.gyroZ = (short)rawDatas[(int)DataIndex.GyroZ];

        data.isButtonPressed = ( rawDatas[(int)DataIndex.IsButtonPressed] == 1 );

        return data;
    }
}
