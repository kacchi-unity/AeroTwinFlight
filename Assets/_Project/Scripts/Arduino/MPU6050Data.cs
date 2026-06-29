public struct MPU6050Data
{
    public float accelX;
    public float accelY;
    public float accelZ;
    public float gyroX;
    public float gyroY;
    public float gyroZ;

    public MPU6050Data(ArduinoSensorData rawData)
    {
        accelX = rawData.accelX;
        accelY = rawData.accelY;
        accelZ = rawData.accelZ;
        gyroX = rawData.gyroX;
        gyroY = rawData.gyroY;
        gyroZ = rawData.gyroZ;
    }
}