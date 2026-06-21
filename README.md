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
