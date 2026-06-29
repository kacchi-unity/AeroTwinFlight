#include <Wire.h>

const int MPU_ADDR = 0x68; // MPU 고유 주소
int16_t accelX, accelY, accelZ;        // 가속도 Raw Data
int16_t gyroX, gyroY, gyroZ;        // 자이로 각속도 Raw Data

const float aCoeff = 16384;
const float gCoeff = 131;

const int buttonPin = 2;
int lastButtonState = HIGH;

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

  pinMode(buttonPin, INPUT_PULLUP);
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
  accelX = Wire.read() << 8 | Wire.read();
  accelY = Wire.read() << 8 | Wire.read();
  accelZ = Wire.read() << 8 | Wire.read();
  int16_t temp = Wire.read() << 8 | Wire.read(); // 온도 데이터 미사용
  gyroX = Wire.read() << 8 | Wire.read();
  gyroY = Wire.read() << 8 | Wire.read();
  gyroZ = Wire.read() << 8 | Wire.read();

  Serial.print(accelX); Serial.print(",");
  Serial.print(accelY); Serial.print(",");
  Serial.print(accelZ); Serial.print(",");
  Serial.print(gyroX); Serial.print(",");
  Serial.print(gyroY); Serial.print(",");
  Serial.print(gyroZ); Serial.print(",");

  // //Serial Monitor test
  // Serial.print("가속도 X: "); Serial.print(ax/aCoeff); Serial.print(" g");
  // Serial.print(" | Y: "); Serial.print(ay/aCoeff); Serial.print(" g");
  // Serial.print(" | Z: "); Serial.print(az/aCoeff); Serial.print(" g");
  
  // Serial.print("  ||  자이로 X: "); Serial.print(gx/gCoeff); Serial.print(" °/s");
  // Serial.print(" | Y: "); Serial.print(gy/gCoeff); Serial.print(" °/s");
  // Serial.print(" | Z: "); Serial.print(gz/gCoeff); Serial.println(" °/s");

  // //Serial Plotter test
  // float dataX = gx/gCoeff;
  // float dataY = gy/gCoeff;
  // float dataZ = gz/gCoeff;

  // Serial.print("Min:-250.0 "); 
  // Serial.print("Max:250.0 ");
  // Serial.print("dataX:"); Serial.print(dataX); Serial.print(" ");
  // Serial.print("dataY:"); Serial.print(dataY); Serial.print(" ");
  // Serial.print("dataZ:"); Serial.print(dataZ); Serial.print(" ");

  int buttonState = digitalRead(buttonPin);
  int clickSignal = 0;
  if (buttonState == LOW && lastButtonState == HIGH)
  {
    clickSignal = 1;
    delay(10);
  }

  lastButtonState = buttonState;

  Serial.print(clickSignal);
  Serial.println();
  delay(20); // 0.2 seconds
}