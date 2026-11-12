public struct MPU6050OffsetData
{
    public float accelX;
    public float accelY;
    public float accelZ;
    public float gyroX;
    public float gyroY;
    public float gyroZ;

    public MPU6050OffsetData(float accelX, float accelY, float accelZ, float gyroX, float gyroY, float gyroZ)
    {
        this.accelX = accelX;
        this.accelY = accelY;
        this.accelZ = accelZ;
        this.gyroX = gyroX;
        this.gyroY = gyroY;
        this.gyroZ = gyroZ;
    }
}