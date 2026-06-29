# AeroTwinFlight
유니티 3D 제작 두 번째 포트폴리오 프로젝트

## 프로젝트 소개
* **개발 기간**: 2026.06 ~ 진행 중
* **개발 환경**: Unity 3D
* **개발 인원**: 1인 (kacchi)

## 주요 기능 (구현 예정)
* 3D 비행기 조작 및 카메라 시스템
* 프레임 매니저 및 최적화 시스템
* UI 연동

## 개발 기록
### 6/19
- 프로젝트 구조 설계 및 기획
- 아두이노 메인 스크립트("AeroTwinFlight_Arduino") 연동 및 최상위 Hardware 폴더 구조 생성

### 6/20
- MPU6050 센서 연결
- 가속도, 각속도 Raw Data 수집 구조 설계

### 6/21
- 멀티 스레드 프레임 유지 기능 테스트

### 6/24
- 아두이노 센서 Raw Data 유티니 전송 Serial 통신 처리
- 정제 값 오프셋 계산 로직 추가
- 버튼 클릭 메서드 추가

### 6/25
- 실제 물리 데이터 처리 테스트 검증 완료 및 유니티 씬 내 도입(Test Cube 사용)
- Raw Data를 유의미한 정제 데이터로 처리하는 기능 추가 (MPU6050 기울기, 아두이노 버튼 클릭)
- 원하는 시점에 기울기 데이터를 보정하는 기능 추가
- 기울기의 초기 Offset 계산 및 적용. 조작이 없을 시 가지고 있는 센서 고유 오차 보정 가능
- ![기울기 데이터 Offset 보정 테스트](https://github.com/user-attachments/assets/39a0ca56-4712-41bf-ab06-735bac9307a8)

### 6/28
- 비행기 모델 추가
- 오브젝트 오일러 및 쿼터니안 회전 테스트

### 6/29
- MPU6050 센서 데이터 수 처리 개수 확장 (테스트 2개 -> 가속도 및 각속도 데이터 6개)
- MPU6050 센서 데이터 보정 처리 구조 개선 (MPU 단독 센서 데이터 및 오프셋 결과 데이터 구조체 추가)

## 기술적 도전 (Technical Challenges)
### 유니티 멀티 스레드 및 싱글 스레드 프레임 테스트
- **문제 정의**: 동기식 데이터 대기 처리로 인한 유니티 메인 스레드 렌더링 프레임 드랍을 예상함.
- **테스트 내용**:
- Thread.Sleep(200)을 이용해, 멀티스레드 미사용 시와 사용 시 게임 씬 내 테스트 큐브 회전 퍼포먼스 대조 실험.
- 싱글 스레드 환경(Test_NotUseThread.cs): 매 프레임 마다 생기는 메인 스레드의 0.2초 지연으로 인해 프레임 드랍이 일어남.
- ![싱글 스레드 프레임 테스트](https://private-user-images.githubusercontent.com/262567585/610849576-3791c235-72be-4928-8fe5-0c91bf67e7a9.gif?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3ODIwMzAxMzMsIm5iZiI6MTc4MjAyOTgzMywicGF0aCI6Ii8yNjI1Njc1ODUvNjEwODQ5NTc2LTM3OTFjMjM1LTcyYmUtNDkyOC04ZmU1LTBjOTFiZjY3ZTdhOS5naWY_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjYwNjIxJTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI2MDYyMVQwODE3MTNaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT0xODI2ZTA3MzgxYjNlY2Y4YTBlNjkzOTM0YmI3YmI2ZTM5MjJkMGFhNjhjY2UxMDFkMzIzOGUxODVmNjQzMzNhJlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCZyZXNwb25zZS1jb250ZW50LXR5cGU9aW1hZ2UlMkZnaWYifQ.TiwObPiS8hvzFqI6B8YPezCkqPD743kSZZyqyzD1dhc)

- 멀티 스레드 환경(Test_Thread.cs): 서브 스레드 선언 및 처리를 통해 매 반복문 내 0.2초 또는 그 이상으로 지연을 주어도 메인 스레드의 렌더링 처리에 영향을 주지 않음. 매끄러운 프레임 유지
- ![멀티 스레드 프레임 테스트](https://private-user-images.githubusercontent.com/262567585/610849575-0bfa55d8-41db-479b-9acf-c14b8e732338.gif?jwt=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3ODIwMzAxMzMsIm5iZiI6MTc4MjAyOTgzMywicGF0aCI6Ii8yNjI1Njc1ODUvNjEwODQ5NTc1LTBiZmE1NWQ4LTQxZGItNDc5Yi05YWNmLWMxNGI4ZTczMjMzOC5naWY_WC1BbXotQWxnb3JpdGhtPUFXUzQtSE1BQy1TSEEyNTYmWC1BbXotQ3JlZGVudGlhbD1BS0lBVkNPRFlMU0E1M1BRSzRaQSUyRjIwMjYwNjIxJTJGdXMtZWFzdC0xJTJGczMlMkZhd3M0X3JlcXVlc3QmWC1BbXotRGF0ZT0yMDI2MDYyMVQwODE3MTNaJlgtQW16LUV4cGlyZXM9MzAwJlgtQW16LVNpZ25hdHVyZT1hZTk2Yjc1ZTMzN2IzZDFhNTM4YTQ5MmE2NDRhODIxMTQ2ZDM2YjA1ODU0MzdlMWJmYWQ4NWRiYjliZGQzNGY4JlgtQW16LVNpZ25lZEhlYWRlcnM9aG9zdCZyZXNwb25zZS1jb250ZW50LXR5cGU9aW1hZ2UlMkZnaWYifQ.xOk892XHqXKGM5DN6sgHXDl4tvkQL-Qolt5YGbp9SU8)
- **핵심 성과**: 아두이노 Raw Data 처리를 메인-서브 스레드 간 안전한 데이터 교환 구조로 처리하도록 계획할 수 있게됨.

### 짐벌락 처리 및 쿼터니안 사용
- **문제 정의**: 유니티 오일러 오브젝트 회전의 짐벌락 문제 고려
- **테스트 내용**:
- 유니티 월드 좌표축을 기준으로 세 비행기 오브젝트를 준비.
	1. x축 회전이 0인 기준 오브젝트
	2. x축 회전이 -90도이며 오일러 회전이 적용된 짐벌락 오브젝트
	3. x축 회전이 같은 -90도이며 쿼터니안 회전을 적용한 오브젝트
	단, x축은 Pitch, y축은 Yaw, z축은 Roll 회전으로 정의
	![비행기 축 정의](https://github.com/user-attachments/assets/8c80f4c5-3b58-4daa-9c51-04af58d27d76)
	- <sub>[출처] Wikiversity</sub>

- Z축(Roll) 회전 테스트
- ![Z축 회전 테스트](https://github.com/user-attachments/assets/d01b4763-b6ae-4887-bf69-649018fb790e)
- > Z축 회전 시에는 오일러 각과 쿼터니언 모두 의도한 대로 문제없이 회전함. (Roll)

- Y축(Yaw) 회전 테스트
- ![Y축 회전 테스트](https://github.com/user-attachments/assets/5dd42592-a9a3-4a98-a865-968ffad413c3)
- > 오일러 회전: 짐벌 락으로 인해 Y축(Yaw) 회전이 Z축(Roll) 회전처럼 인식, 작동해 의도하지 않은 축 방향 회전 발생.
- > 쿼터니언 회전: 의도대로 잘 회전함. (Yaw)

- **핵심 성과**:
- 유니티 엔진 내에선 오일러 회전 시, Z축 -> X축 -> Y축 순서로 회전시키는 점을 알아냄.
- 상위 구조 순서로 부모 Y축, 자식 X축, 자손 Z축 계층구조로 되어있음. Z축 회전시 독립적으로, X축 회전시 Z축도 함께, Y축 회전시 모든 축이 함께 회전함. (짐벌 구조)
- 오일러 회전 테스트 시, X축 -90도 회전으로 인해 Z축이 함께 회전하여 움직이지 않던 Y축과 축이 겹쳐버려 자유도(Degree of Freedom)를 상실함. 같은 회전으로 인식하는 문제 발생.
- ![짐벌락](https://github.com/user-attachments/assets/03aed650-3e8e-404e-8b21-4145b8e797a4)
- <sub>[출처] 구글 이미지 검색</sub>
- 오일러 회전은 변수 간의 종속성(coupling)이 발생하여 독립적인 제어력을 상실할 수 있음. (R = Rz Ry Rx)

- 쿼터니안 회전은 3개의 독립된 x, y, z축을 순서대로 회전시키는 대신, 4차원 공간의 하나의 회전축 w을 기준으로 물체를 한 번에 회전시키기 때문에 짐벌락이 일어나지 않음.
- 쿼터니안 회전은 복소 사원수 w + xi + yj + zk를 이용해 4차원으로 확장하여 한번에 이동함. 순차적인 축 회전이라는 개념이 없으므로 짐벌락이 일어나지 않음.
- 결론: 해당 프로젝트에서 아두이노 센서 데이터를 받아 오일러 회전 각을 쿼터니안으로 변환, 저장하는 Quaternion.Euler(Vector3)을 적극 사용할 예정.
- 데이터 사용 과정에서 오브젝트 제어의 오염과 짐벌락을 막을 예정.
