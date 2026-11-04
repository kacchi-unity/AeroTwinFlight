using UnityEngine;
using System.Collections.Generic;

public class CameraFollowManager : MonoBehaviour
{
    [Header("메인 카메라 컴포넌트")]
    [SerializeField] private Transform mainCamera;
    [Header("비행기 오브젝트 컴포넌트")]
    [SerializeField] private Transform targetFlight;
    [Header("버튼 이벤트 컴포넌트")]
    [SerializeField] private ArduinoButtonManager arduinoButtonManager;
    [Header("서브 카메라 컴포넌트 연결 리스트")]
    [Tooltip("True: Yaw 축만 회전 / False : 전체 축 기준 회전")]
    [SerializeField] private List<CameraPreset> cameraPresets;

    [System.Serializable]
    private struct CameraPreset
    {
        public Transform cameraTransform;
        public bool followOnlyYaw;
    }

    private List<Vector3> initialLocalOffsets = new List<Vector3>();
    private List<Quaternion> initialLocalRotations = new List<Quaternion>();

    private int currentIndex = 0;

    private void OnEnable()
    {
        arduinoButtonManager.CameraChangeButtonClicked += ChangeCameraIndex;
    }

    private void OnDisable()
    {
        arduinoButtonManager.CameraChangeButtonClicked -= ChangeCameraIndex;
    }

    private void ChangeCameraIndex()
    {
        currentIndex = (currentIndex + 1) % cameraPresets.Count;
    }

    private void Start()
    {
        if (targetFlight == null || cameraPresets == null || cameraPresets.Count == 0)
        {
            Debug.LogError($"{this.name}: 데이터 셋을 확인하세요");
            enabled = false;
            return;
        }

        SaveOffsets(cameraPresets);
    }

    private void SaveOffsets(List<CameraPreset> targetTransforms)
    {
        for (int i = 0; i < targetTransforms.Count; i++)
        {
            Vector3 positionOffset = targetFlight.InverseTransformPoint(targetTransforms[i].cameraTransform.position);
            initialLocalOffsets.Add(positionOffset);

            Quaternion rotationOffset = Quaternion.Inverse(targetFlight.rotation) * targetTransforms[i].cameraTransform.rotation;
            initialLocalRotations.Add(rotationOffset);
        }
    }

    private void LateUpdate()
    {
        FollowCamera(
          initialLocalOffsets[currentIndex],
          initialLocalRotations[currentIndex],
          cameraPresets[currentIndex].followOnlyYaw
          );
    }

    private void FollowCamera(Vector3 positionOffset, Quaternion rotationOffset, bool followOnlyYaw)
    {
        // Case: Follow Only Yaw Axis Rotation
        if (followOnlyYaw)
        {
            Vector3 flatForward = targetFlight.forward;
            flatForward.y = 0f;

            if (flatForward.sqrMagnitude < 0.001f)
                return;

            flatForward.Normalize();

            Quaternion yawRotation =
              Quaternion.LookRotation(flatForward, Vector3.up);

            mainCamera.position =
              targetFlight.position +
              yawRotation * positionOffset;

            mainCamera.rotation =
              yawRotation * rotationOffset;
        }

        // Case: Follow All Axis Rotation
        else
        {
            mainCamera.position = targetFlight.position + targetFlight.rotation * positionOffset;
            mainCamera.rotation = targetFlight.rotation * rotationOffset;
        }
    }
}