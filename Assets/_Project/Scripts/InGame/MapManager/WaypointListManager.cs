using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointListManager : MonoBehaviour
{
    [Header("컴포넌트 참조")]
    [SerializeField] private RectTransform contentRectTransform;
    [SerializeField] private Button confirmButton;

    [Header("Base 본대 이름 설정")]
    [SerializeField] private String baseStationName = "Base Station";

    private List<WaypointCollector> waypointCollectors = new List<WaypointCollector>();
    private List<WaypointData> waypointDatas = new List<WaypointData>();

    public event Action<IReadOnlyList<WaypointData>> WaypointUpdated;


    private void OnEnable()
    {
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    private void OnDisable()
    {
        confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
    }

    private void Start()
    {
        contentRectTransform.GetComponentsInChildren<WaypointCollector>(true, waypointCollectors);
    }

    private void OnConfirmButtonClicked()
    {
        this.waypointDatas.Clear();

        //First: 사용자의 유효 데이터 (0~5개) 좌표 등록
        WaypointData targetWaypointData;

        for (int i=0; i<waypointCollectors.Count; i++)
        {
            targetWaypointData = waypointCollectors[i].GetWaypointData();

            //IsValid 검사: 유효한 데이터만 리스트에 등록
            if (targetWaypointData.IsValid)
            {
                this.waypointDatas.Add(targetWaypointData);
            }
        }

        //Second: 데이터 개수 검사 후 Waypoint -> Base Station 고정 삽입
        if (this.waypointDatas.Count > 0)
        {
            WaypointData groundCubeData = new WaypointData(
                this.baseStationName,
                0, 
                0,
                0,
                true,
                true //IsBaseStation -> True
            );
            this.waypointDatas.Add(groundCubeData);

            WaypointUpdated?.Invoke(this.waypointDatas);
        }

        else
        {
            Debug.LogWarning("[Waypoint] 유효한 웨이포인트 데이터가 없습니다.");
        }
        
    }
}
