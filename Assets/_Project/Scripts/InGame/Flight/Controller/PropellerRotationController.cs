using UnityEngine;

public class PropellerRotationController : MonoBehaviour
{
    [Header("비행체 RigidBody")]
    [SerializeField] private Rigidbody flightRigidBody;
    [Header("프로펠러 회전 속도 배율")]
    [SerializeField] private float rotationSpeedMultiplier = 500f;

    void Start()
    {
        if (flightRigidBody == null)
        {
            Debug.LogWarning($"{this.name}: {flightRigidBody.name}의 연결을 확인하세요.");
            enabled = false;

            return;
        }
    }

    void Update()
    {
        float currentSpeed = flightRigidBody.linearVelocity.magnitude;
        float rotationSpeed = currentSpeed * rotationSpeedMultiplier;

        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
    }
}
