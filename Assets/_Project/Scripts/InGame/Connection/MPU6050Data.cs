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
        accelX = rawData.AccelX;
        accelY = rawData.AccelY;
        accelZ = rawData.AccelZ;
        gyroX = rawData.GyroX;
        gyroY = rawData.GyroY;
        gyroZ = rawData.GyroZ;
    }
}