using UnityEngine;
using System.Collections; //IEnumerator
using System.Collections.Generic; //List
using CesiumForUnity; //Cesium API 사용
using Unity.Mathematics;
using System; //double3
//using NUnit.Framework; //Cesium double3 type 연산 사용

public class GeoMapGenerator : MonoBehaviour
{
    [Header ("Cesium Connection (3D 타일 맵 오브젝트)")]
    [SerializeField] private GameObject cesiumObject;
    [SerializeField] private CesiumGeoreference cesiumGeoreference;

    [Header("Base Connection (본대 오브젝트)")]
    [SerializeField] private GameObject baseStationObject;
    [SerializeField] private Transform baseStationObjectTransform;

    [Header("Component Connection")]
    [SerializeField] private Transform flightTransform;
    [SerializeField] private Rigidbody flightRigidbody;
    [SerializeField] private WaypointListManager waypointListManager;


    /*
        [Header("World Target Rotation (지구 좌표)")]
        [Tooltip("위도")]
        [SerializeField] private double targetLatitude = 34.7052;
        [Tooltip("경도")]
        [SerializeField] private double targetLongitude = 135.4906;
        [Tooltip("고도 [m]")]
        [SerializeField] private double targetHeight = 15.0;*/

    [Header("오프셋 설정")]
    [SerializeField] private float heightOffset = 130f;
    [SerializeField] private float forwardOffset = 250f;
    [SerializeField] private float fogEffectTime = 10f;

    [Header("Fog 설정")]
    [SerializeField] private Material fogMaterial;
    [Range(0f,1f)][SerializeField] private float clearFogRatio = 0f;
    [Range(0f, 1f)][SerializeField] private float denseFogRatio = 1f;
    [SerializeField] private float fogFadeDuration = 2f;

    private static readonly int FogAmount = Shader.PropertyToID("_FogAmount");

    private bool isGeneratingMap = false;
    private bool isWaypointChangeAllowed = false;
    private IReadOnlyList<WaypointData> savedWaypointDatas;
    private int currentWaypointDataIndex = -1;

    public event Action<WaypointData> ChangedWaypoint;

    public void SetIsWaypointChangeAllowed(bool targetBoolean)
    {
        isWaypointChangeAllowed = targetBoolean;
    }

    private void OnEnable()
    {
        waypointListManager.WaypointUpdated += UpdateWaypointDatas;
    }

    private void OnDisable()
    {
        waypointListManager.WaypointUpdated -= UpdateWaypointDatas;
    }

    private void UpdateWaypointDatas(IReadOnlyList<WaypointData> waypointDatas)
    {
        this.savedWaypointDatas = waypointDatas;
        this.currentWaypointDataIndex = -1;
    }

    private void Start()
    {
        this.cesiumObject.SetActive(false);
        this.baseStationObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && 
            isWaypointChangeAllowed &&
            this.savedWaypointDatas != null &&
            this.savedWaypointDatas.Count >= 2 && 
            !isGeneratingMap
            )
        {
            //자동 인덱스 순회 이동 (반복 사이클)
            int nextWaypointDataIndex = (currentWaypointDataIndex + 1) % this.savedWaypointDatas.Count;
            
            StartCoroutine(GenerateNextMapRoutine(nextWaypointDataIndex));

        }
    }

    private IEnumerator GenerateNextMapRoutine(int targetIndex)
    {
        isGeneratingMap = true;

        WaypointData targetData = this.savedWaypointDatas[targetIndex];

        //Update Waypoint Info
        currentWaypointDataIndex = targetIndex;

        this.ChangedWaypoint?.Invoke(targetData);

        //Fog fade in
        yield return StartCoroutine(FadeFogRoutine(clearFogRatio, denseFogRatio, fogFadeDuration));

        float currentSpeed = flightRigidbody.linearVelocity.magnitude;

        float moveDistanceDuringFog = currentSpeed * fogEffectTime;

        float totalForwardDistance = moveDistanceDuringFog + forwardOffset;

        Vector3 horizontalForward = Vector3.ProjectOnPlane(flightTransform.forward, Vector3.up).normalized;

        //Exception: case of horizontalForward vector is close to zero
        if (horizontalForward.sqrMagnitude < 0.001f)
        {
            horizontalForward = flightTransform.forward;
        }

        Vector3 targetWorldPosition = flightTransform.position
                                   + (horizontalForward * totalForwardDistance)
                                   + (Vector3.down * heightOffset);

        //현재 월드 맵 대상이 Base Station일 경우
        if (targetData.IsBaseStation && baseStationObjectTransform != null)
        {
            this.baseStationObject.SetActive(true);
            this.cesiumObject.SetActive(false);

            //baseStationObject SetActive 기다리기 -> 한 프레임 대기 (base 내부 데이터 갱신 보장)
            yield return null;

            baseStationObjectTransform.position = targetWorldPosition;
        }

        //현재 월드 맵 대상이 Base Station이 아닐 경우
        else
        {
            this.cesiumObject.SetActive(true);
            this.baseStationObject.SetActive(false);

            //cesium SetActive 기다리기 -> 한 프레임 대기 (Cesium 내부 데이터 갱신 보장)
            yield return null;

            if (cesiumGeoreference != null)
            {
                double3 targetEcef = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(
                    new double3(targetData.Longitude, targetData.Latitude, targetData.Height)
                );

                double3 currentEcefAtTarget = cesiumGeoreference.TransformUnityPositionToEarthCenteredEarthFixed(
                    new double3(targetWorldPosition.x, targetWorldPosition.y, targetWorldPosition.z)
                );

                double3 ecefDelta = targetEcef - currentEcefAtTarget;

                cesiumGeoreference.SetOriginEarthCenteredEarthFixed(
                    cesiumGeoreference.ecefX + ecefDelta.x,
                    cesiumGeoreference.ecefY + ecefDelta.y,
                    cesiumGeoreference.ecefZ + ecefDelta.z
                );
            }
        }

        //안개 Fade in/out 소요 시간 고려
        float waitDuration = Mathf.Max(0f, fogEffectTime - fogFadeDuration * 2f);
        yield return new WaitForSeconds(waitDuration);

        //Fog fade out
        yield return StartCoroutine(FadeFogRoutine(denseFogRatio, clearFogRatio, fogFadeDuration));

        isGeneratingMap = false;
    }

    private IEnumerator FadeFogRoutine(float startRatio, float endRatio, float duration)
    {
        if (fogMaterial == null)
        {
            yield break;
        }

        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentFogAmount = Mathf.Lerp(
                startRatio,
                endRatio, 
                elapsedTime / duration
            );

            fogMaterial.SetFloat(FogAmount, currentFogAmount);
            yield return null;
        }

        fogMaterial.SetFloat(FogAmount, endRatio);
    }
}
