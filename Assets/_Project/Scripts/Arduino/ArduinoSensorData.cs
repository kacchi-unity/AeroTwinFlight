// ====================================================================
// * 패킷 구조 규칙
// - index [0~2] : 가속도 (Accel X, Y, Z / short)
// - index [3~5] : 각속도 (Gyro X, Y, Z / short)
// - index [6~8] : 버튼 누름 횟수 데이터 (ClickCount / unsigned int_32)
// - index [9] : 가변 저항 슬라이더 (SliderValue / float)
// ====================================================================

using UnityEngine;
using System;

public struct ArduinoSensorData
{
    //Data index enum
    private enum DataIndex
    {
        AccelX = 0,
        AccelY,
        AccelZ,

        GyroX,
        GyroY,
        GyroZ,

        CalibrationButton,
        MapChangeButton,
        CameraChangeButton,

        SliderValue,

        TotalDataCount //Auto count
    }

    //Data fields
    public short AccelX { get; private set; }
    public short AccelY { get; private set; }
    public short AccelZ { get; private set; }

    public short GyroX { get; private set; }
    public short GyroY { get; private set; }
    public short GyroZ { get; private set; }

    public uint CalibrationButton { get; private set; }
    public uint MapChangeButton { get; private set; }
    public uint CameraChangeButton { get; private set; }

    public float SliderValue { get; private set; }

    /// <summary>
    /// 패킷의 Byte 배열을 받아 구조체 형태로 변환하는 멤버 메서드
    /// </summary>
    public static ArduinoSensorData ParseData(byte[] packetDatas)
    {
        if (packetDatas == null)
        {
            Debug.LogError("[ArduinoSensorData] 파싱 실패: 패킷 데이터를 확인하세요.");

            //return default struct
            return default;
        }

        ArduinoSensorData data = new ArduinoSensorData();
        int offset = 0;

        data.AccelX = BitConverter.ToInt16(packetDatas, offset); offset += sizeof(short); // 2 (Byte 단위)
        data.AccelY = BitConverter.ToInt16(packetDatas, offset); offset += sizeof(short);
        data.AccelZ = BitConverter.ToInt16(packetDatas, offset); offset += sizeof(short);

        data.GyroX = BitConverter.ToInt16(packetDatas, offset); offset += sizeof(short);
        data.GyroY = BitConverter.ToInt16(packetDatas, offset); offset += sizeof(short);
        data.GyroZ = BitConverter.ToInt16(packetDatas, offset); offset += sizeof(short);

        data.CalibrationButton = BitConverter.ToUInt32(packetDatas, offset); offset += sizeof(uint);
        data.MapChangeButton = BitConverter.ToUInt32(packetDatas, offset); offset += sizeof(uint);
        data.CameraChangeButton = BitConverter.ToUInt32(packetDatas, offset); offset += sizeof(uint);

        data.SliderValue = BitConverter.ToSingle(packetDatas, offset); offset += sizeof(float);
        return data;
    }

    /// <summary>
    /// int.TryParse로 검증된 rawDatas를 받아 구조체 데이터를 완성시키는 메서드
    /// </summary>
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

        data.AccelX = (short)rawDatas[(int)DataIndex.AccelX];
        data.AccelY = (short)rawDatas[(int)DataIndex.AccelY];
        data.AccelZ = (short)rawDatas[(int)DataIndex.AccelZ];

        data.GyroX = (short)rawDatas[(int)DataIndex.GyroX];
        data.GyroY = (short)rawDatas[(int)DataIndex.GyroY];
        data.GyroZ = (short)rawDatas[(int)DataIndex.GyroZ];

        data.CalibrationButton = (uint)rawDatas[(int)DataIndex.CalibrationButton];

        return data;
    }

    public static int GetTotalDataCount()
    {
        return (int)DataIndex.TotalDataCount;
    }
}
