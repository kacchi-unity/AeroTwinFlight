using UnityEngine;

public class Test_Trigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;       // 인스펙터에서 Ground 레이어 선택
    [SerializeField] private BoxCollider[] landingColliders; // 박스 A, B 배열에 넣기

    private TriggerDetector detectorA;
    private TriggerDetector detectorB;

    private void Start()
    {
        // 박스들이 제대로 들어왔는지 방어 코드
        if (landingColliders == null || landingColliders.Length < 2)
        {
            Debug.LogError("착지용 BoxCollider가 2개 이상 등록되지 않았습니다!");
            return;
        }

        // 우리가 앞서 만든 'SetupDetector 공장'과 똑같은 로직을 여기서 테스트해봅니다.
        detectorA = SetupDetector(landingColliders[0]);
        detectorB = SetupDetector(landingColliders[1]);

        Debug.Log("LandingTest 세팅 완료! 바닥에 가져다 대보세요.");
    }

    private void Update()
    {
        // 센서들이 잘 생성되었고, 둘 다 Ground에 닿았는지 매 프레임 검사
        if (detectorA != null && detectorB != null)
        {
            if (detectorA.IsTouchingTarget && detectorB.IsTouchingTarget)
            {
                Debug.Log("[성공] 박스 A와 B 모두 Ground에 닿았습니다!");
            }
        }
    }

    // 아까 그 리팩토링했던 조립 메서드 그대로 가져옴
    private TriggerDetector SetupDetector(BoxCollider box)
    {
        box.isTrigger = true; // 트리거 강제 활성화

        var detector = box.GetComponent<TriggerDetector>();
        if (detector == null)
        {
            detector = box.gameObject.AddComponent<TriggerDetector>();
        }

        detector.Initialize(groundLayer);
        return detector;
    }
}