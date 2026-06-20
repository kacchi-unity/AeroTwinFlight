#include <Wire.h>

const int MPU_ADDR = 0x68; // MPU 고유 주소
int16_t ax, ay, az;        // 가속도
int16_t gx, gy, gz;        // 자이로 각속도

const float aCoeff = 16384;
const float gCoeff = 131;

void setup()
{
  Serial.begin(115200);
  Wire.begin();

  // MPU6050 신호 송신
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x6B); // 전원 관리 레지스터 주소
  Wire.write(0);    //on
  Wire.endTransmission();
  
  Serial.println("Editor: MPU6050 is on...");
}

void loop()
{
  // 0x3B: 필요 데이터 시작 레지스터 주소 (가속도 x축)
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B); 
  Wire.endTransmission(false);

  // 연속 14바이트 (가속도 6 + 온도 2 + 자이로 6 Byte) 데이터 요구
  Wire.requestFrom(MPU_ADDR, 14, true);

  // SDA line data: 10진수 변환
  ax = Wire.read() << 8 | Wire.read();
  ay = Wire.read() << 8 | Wire.read();
  az = Wire.read() << 8 | Wire.read();
  int16_t temp = Wire.read() << 8 | Wire.read(); // 온도 데이터 미사용
  gx = Wire.read() << 8 | Wire.read();
  gy = Wire.read() << 8 | Wire.read();
  gz = Wire.read() << 8 | Wire.read();

  //Serial Monitor test
  Serial.print("가속도 X: "); Serial.print(ax/aCoeff); Serial.print(" g");
  Serial.print(" | Y: "); Serial.print(ay/aCoeff); Serial.print(" g");
  Serial.print(" | Z: "); Serial.print(az/aCoeff); Serial.print(" g");
  
  Serial.print("  ||  자이로 X: "); Serial.print(gx/gCoeff); Serial.print(" °/s");
  Serial.print(" | Y: "); Serial.print(gy/gCoeff); Serial.print(" °/s");
  Serial.print(" | Z: "); Serial.print(gz/gCoeff); Serial.println(" °/s");

  //Serial Plotter test
  float dataX = gx/gCoeff;
  float dataY = gy/gCoeff;
  float dataZ = gz/gCoeff;

  Serial.print("Min:-250.0 "); 
  Serial.print("Max:250.0 ");
  Serial.print("dataX:"); Serial.print(dataX); Serial.print(" ");
  Serial.print("dataY:"); Serial.print(dataY); Serial.print(" ");
  Serial.print("dataZ:"); Serial.print(dataZ); Serial.print(" ");

  delay(10); // 0.2 seconds
}