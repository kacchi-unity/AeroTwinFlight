// ====================================================================
// * 아두이노 통신 프로토콜 규칙
// * 아두이노 전송 형태: "데이터1,데이터2, ... \n"
// (ex: "512,1\n")
// 
// index [0] : 자이로 x (int), x축 각속도
// index [1] : 자이로 y (int), y축 각속도
// index [2] : 버튼 데이터 (int)
// ====================================================================

public struct ArduinoSensorData
{
    //public short accelX;
    //public short accelY;
    //public short accelZ;
    public short gyroX;
    public short gyroY;
    //public short gyroZ;

    public bool isButtonPressed;

    public static ArduinoSensorData ParseData(int[] rawDatas)
    {
        ArduinoSensorData data = new ArduinoSensorData();

        int length = rawDatas.Length;

        if (length > 0) data.gyroX = (short)rawDatas[0];

        if (length > 1) data.gyroY = (short)rawDatas[1];

        if (length > 2) data.isButtonPressed = (rawDatas[2] == 1);

        return data;
    }
}
