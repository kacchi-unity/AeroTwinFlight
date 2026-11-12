#include <Wire.h>
#include <WiFi.h>
#include <PubSubClient.h>

//<Data Packet>

struct DataPacket {
  int16_t accelX, accelY, accelZ;

  int16_t gyroX, gyroY, gyroZ;

  uint32_t clickCounts[3];

  float sliderValue;

}__attribute__((packed));

//MPU6050
const int pinSDA = 4;
const int pinSCL = 5;
const int MPU_ADDR = 0x68; // MPU 고유 주소
int16_t accelX, accelY, accelZ;        // 가속도 Raw Data
int16_t gyroX, gyroY, gyroZ;        // 자이로 각속도 Raw Data
const float aCoeff = 16384; //Serial Monitor test
const float gCoeff = 131; //Serial Monitor test

//Button Counts
const int NUM_BUTTONS = 3;
const int buttonPins[] = {10, 1, 3}; //버튼 핀 지정
uint32_t clickCounts[NUM_BUTTONS]; //Setup
bool lastButtonStates[NUM_BUTTONS]; //Setup

//Slider
const int sliderPin = 0;
const float sliderCoefficient = 4095.f;
const float sliderMinDeadZone = 0.08;
const bool sliderValueReverse = true;

//<Connection>
const char* ssid = "Yesol";     // 사용 중인 2.4GHz WIFI|Hotspot 이름
const char* password = "s1994s10hei321!"; // WIFI|Hotspot 비밀번호
const char* mqtt_HostName = "192.168.137.1"; // 브로커 PC 이름

const char* dataTopic = "flight/sensor";

WiFiClient espClient;
PubSubClient mqttClient(espClient);

unsigned long lastMqttAttempt = 0; //저장 누적 시간
const unsigned long mqttRetryInterval = 3000; //재연결 간격 (ms)

unsigned long lastWiFiAttempt = 0;
const unsigned long wifiRetryInterval = 5000;

const int internalLedPin = 8; //ESP32-C3 내장 led



void setup()
{
  Serial.begin(115200);
  delay(1000);

  //<Data Packet Setup>
  
  Wire.begin(pinSDA, pinSCL); //C3 Super Mini의 GPIO 4(SDA), GPIO 5(SCL) 명시

  // MPU6050 신호 송신
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x6B); // 전원 관리 레지스터 주소
  Wire.write(0);    //on
  Wire.endTransmission();
  
  //Button 초기화
  for (int i = 0; i< NUM_BUTTONS; i++)
  {
    pinMode(buttonPins[i], INPUT_PULLUP);
    clickCounts[i] = 0;
    lastButtonStates[i] = HIGH;
  }

  // Slider ADC 전압 측정 범위 설정
  analogReadResolution(12);
  analogSetPinAttenuation(sliderPin, ADC_11db);

  //<Connection Setup>

  delay(1000);

  pinMode(internalLedPin, OUTPUT);
  digitalWrite(internalLedPin, HIGH); //Active Low -> HIGH is led off

  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.setAutoReconnect(true); //WiFi 자동 재접속 활성화
  WiFi.begin(ssid, password);

  int timeout = 0;
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    timeout++;

    // 10초 이상 접속 실패 시 실패 상태로 Setup 탈출
    if (timeout > 20) {
      Serial.println("\nInitial Wi-Fi connection deferred. Will retry in loop.");
      break;
    }
  }

  if (WiFi.status() == WL_CONNECTED) {
    Serial.println("\nWi-Fi connected successfully!");
    Serial.print("IP Address: ");
    Serial.println(WiFi.localIP());
  }

  mqttClient.setServer(mqtt_HostName, 1883); //브로커 IP와 포트 지정 (단순 설정, 1회 충분)

}

void loop()
{
  //<Connection Loop>
  handleConnection();

  //<Data Extraction Loop>
  if (mqttClient.connected()) {

    mqttClient.loop();

    DataPacket currentPacket;
    
    readSensorData(currentPacket);

    //Publish Data Packet
    //uint8_t*: 8비트씩 쪼갠 array로 취급
    mqttClient.publish(dataTopic, (uint8_t*)&currentPacket, sizeof(currentPacket));
    
    //SerialMonitorTest(currentPacket);
  }

  delay(20); // 0.02 seconds
}


void handleConnection()
{
  //WiFi Connection Check
  if (WiFi.status() != WL_CONNECTED) {
    digitalWrite(internalLedPin, HIGH);
    
    unsigned long currentMillis = millis();

    if (currentMillis - lastWiFiAttempt >= wifiRetryInterval)
    {
      lastWiFiAttempt = currentMillis;
      
      Serial.println("Wi-Fi disconnected! Reconnecting...");

      WiFi.begin(ssid, password);
    }

    return;
  }

  // MQTT Connection Check
  if (!mqttClient.connected()) {
    digitalWrite(internalLedPin, HIGH);

    unsigned long currentMillis = millis();

    // 클라이언트 ID("ESP32Client")를 이용해 접속 시도
    if (currentMillis - lastMqttAttempt >= mqttRetryInterval) {
      lastMqttAttempt = currentMillis;

      Serial.print("Connecting to MQTT broker...");
      
      if (mqttClient.connect("ESP32Client")) 
      {
        Serial.println(" connected!");
        mqttClient.subscribe(dataTopic);
        
        mqttClient.loop();
      } 

      else {
        Serial.print(" failed, Error Code=");
        Serial.print(mqttClient.state());
        Serial.println(" try again in 3 seconds");
      }
    }

    return;
  }

  digitalWrite(internalLedPin, LOW); //Connection Success: Led On
  mqttClient.loop(); //MQTT 연결 유지 메서드
}

void readSensorData(DataPacket &packet)
{
  readMPU6050(packet);

  readButtonCounts(packet);

  readSliderValue(packet);
}

void readMPU6050(DataPacket &packet)
{
  // 0x3B: 필요 데이터 시작 레지스터 주소 (가속도 x축)
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B); 
  Wire.endTransmission(false);

  // 연속 14바이트 (가속도 6 + 온도 2 + 자이로 6 Byte) 데이터 요구
  Wire.requestFrom(MPU_ADDR, 14, true);

  // SDA line data: 10진수 변환
  packet.accelX = Wire.read() << 8 | Wire.read();
  packet.accelY = Wire.read() << 8 | Wire.read();
  packet.accelZ = Wire.read() << 8 | Wire.read();
  int16_t temp = Wire.read() << 8 | Wire.read(); // 온도 데이터 미사용
  packet.gyroX = Wire.read() << 8 | Wire.read();
  packet.gyroY = Wire.read() << 8 | Wire.read();
  packet.gyroZ = Wire.read() << 8 | Wire.read();
}

void readButtonCounts(DataPacket &packet)
{
  //Button signal
  for (int i = 0; i < NUM_BUTTONS; i++)
  {
    bool currentState = digitalRead(buttonPins[i]);
    
    if (currentState == LOW && lastButtonStates[i] == HIGH)
    {
      clickCounts[i]++;
      Serial.print("Button Press Detected. Button Index: ");
      Serial.print(i);
      Serial.print(", Press Count: ");
      Serial.println(clickCounts[i]);
    }
    
    lastButtonStates[i] = currentState;
    packet.clickCounts[i] = clickCounts[i];
  }
  

  /*
  if (currentState != lastButtonState)
  {
    lastDebounceTime = millis();
  }

  if ((millis() - lastDebounceTime) > debounceDelay)
  {
    if (currentState != lastSteadyState)
    {
      lastSteadyState = currentState;

      if (lastSteadyState == LOW)
      {
        clickSignal = 1;
      }
    }
  }*/

  /*
  if (currentState == LOW && lastButtonState == HIGH)
  {
    clickSignal = 1;
    Serial.println("Button Press");
  }*/
}

void readSliderValue(DataPacket &packet)
{
  int rawSliderValue = analogRead(sliderPin);

  float normalizedSliderValue = rawSliderValue/sliderCoefficient; 

  // 3. 최소로 내렸을 때 0.02 남는 현상(2번 문제) 해결용 데드존 처리
  if (normalizedSliderValue < sliderMinDeadZone) {
    normalizedSliderValue = 0.0;
  }
  
  if (sliderValueReverse)
  {
    normalizedSliderValue = (1.0 - normalizedSliderValue);
  }
  
  packet.sliderValue = normalizedSliderValue;

}

void SerialMonitorTest(DataPacket packet) {
  //Serial Monitor test
  Serial.print("가속도 X: "); Serial.print(packet.accelX/aCoeff); Serial.print(" g");
  Serial.print(" | Y: "); Serial.print(packet.accelY/aCoeff); Serial.print(" g");
  Serial.print(" | Z: "); Serial.print(packet.accelZ/aCoeff); Serial.print(" g");
  
  Serial.print("  ||  자이로 X: "); Serial.print(packet.gyroX/gCoeff); Serial.print(" °/s");
  Serial.print(" | Y: "); Serial.print(packet.gyroY/gCoeff); Serial.print(" °/s");
  Serial.print(" | Z: "); Serial.print(packet.gyroZ/gCoeff); Serial.println(" °/s");
}



