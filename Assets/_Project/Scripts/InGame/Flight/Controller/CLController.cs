using Unity.Hierarchy;
using UnityEngine;

public class CLController : MonoBehaviour, ICLProvider
{
    [SerializeField] private Rigidbody flightRigidbody;
    [Header("초기 장착각 (degree)")]
    [SerializeField] private float incidenceAngle = 5f;
    [Header("CL-AOA 그래프")]
    [SerializeField] AnimationCurve CLCurve;

    private float cl = 0f;

    private float aoa = 0f;

    void Awake()
    {
        CLCurve = new AnimationCurve();

        float[] aoaValues =
        {
            0f, 3f, 5f, 8f, 11f, 14f, 17f, 20f, 23f, 26f
        };

        float[] clValues =
        {
            0.25f, 0.65f, 1.20f, 1.38f, 1.52f, 1.65f, 1.75f, 1.80f, 1.45f, 0.90f
        };

        if (aoaValues.Length != clValues.Length)
        {
            Debug.LogError($"{this.name}: 그래프 포인트 컴포넌트 개수를 확인하세요");
            enabled = false;
        }

        else
        {
            for (int i = 0; i < aoaValues.Length; i++)
            {
                CLCurve.AddKey(aoaValues[i], clValues[i]);
            }

            for (int i = 0; i < CLCurve.length; i++)
            {
                CLCurve.SmoothTangents(i, 0.5f);
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 velocity = flightRigidbody.linearVelocity;

        //Pitch
        float pitch = Mathf.Atan2(
            Vector3.Dot(transform.forward, Vector3.up),
            Vector3.ProjectOnPlane(transform.forward, Vector3.up).magnitude
            ) * Mathf.Rad2Deg;

        //Velocity's speed in horizontal
        float horizontalSpeed =
            Vector3.ProjectOnPlane(velocity, Vector3.up).magnitude;

        //Velocity angle in horizontal
        float flightVelocityAngle = Mathf.Atan2(
            velocity.y,
            horizontalSpeed
            ) * Mathf.Rad2Deg;

        //AOA: Pitch와 실제 Velocity의 각도 차이
        this.aoa = pitch - flightVelocityAngle;

        this.cl = CLCurve.Evaluate(aoa + incidenceAngle);

    }

    public float GetCL()
    {
        return this.cl;
    }

    public float GetAOA()
    {
        return (this.aoa + this.incidenceAngle);
    }
}
