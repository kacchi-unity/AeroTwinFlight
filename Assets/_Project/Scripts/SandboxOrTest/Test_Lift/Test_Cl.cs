using TMPro;
using UnityEngine;

public class Test_Cl : MonoBehaviour
{
    [SerializeField] private Transform tr;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private TextMeshProUGUI tmp;

    [Header("Important settings")]
    [SerializeField] AnimationCurve clCurve;
    [SerializeField] AnimationCurve clCurve2;
    [SerializeField] AnimationCurve clCurve3;
    [SerializeField] AnimationCurve clCurve4;
    [SerializeField] private float incidenceAngle = 5f;

    float cl=0;

    void Awake()
    {
        clCurve3 = new AnimationCurve();

        float[] aoaValues = { -10f, -5f, 0f, 5f, 10f, 12f, 15f, 18f, 20f, 25f };
        float[] clValues = { -0.4f, 0f, 0.3f, 0.7f, 1.1f, 1.25f, 1.35f, 1.30f, 1.15f, 0.6f};

        for (int i = 0; i< aoaValues.Length; i++)
        {
            clCurve3.AddKey(aoaValues[i], clValues[i]);
        }

        for (int i = 0; i < clCurve3.length; i++)
        {
            clCurve3.SmoothTangents(i, 0.5f);
        }

        //----------

        clCurve4 = new AnimationCurve();

        float[] aoaValues2 =
        {
            0f, 3f, 5f, 8f, 11f, 14f, 17f, 20f, 23f, 26f
        };

        float[] clValues2 =
        {
            0.25f, 0.65f, 1.20f, 1.38f, 1.52f, 1.65f, 1.75f, 1.80f, 1.45f, 0.90f
        };

        for (int i = 0; i < aoaValues2.Length; i++)
        {
            clCurve4.AddKey(aoaValues2[i], clValues2[i]);
        }

        for (int i = 0; i < clCurve4.length; i++)
        {
            clCurve4.SmoothTangents(i, 0.5f);
        }
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        //Pitch
        float pitch =
            Mathf.Atan2(
                Vector3.Dot(tr.forward, Vector3.up),
                Vector3.ProjectOnPlane(tr.forward, Vector3.up).magnitude
            ) * Mathf.Rad2Deg;

        //Velocity -> horizontal
        float horizontalSpeed =
            Vector3.ProjectOnPlane(velocity, Vector3.up).magnitude;

        //Velocity angle with horizontal
        float flightPathAngle =
            Mathf.Atan2(
                velocity.y,
                horizontalSpeed
            ) * Mathf.Rad2Deg;

        // Pitch와 비행경로의 차이 = AoA (Degree)
        float aoa = pitch - flightPathAngle;

        cl = clCurve4.Evaluate(aoa + incidenceAngle);

        tmp.text =
            $"AOA = {aoa:F1}°\n" +
            $"Pitch = {-pitch:F1}°\n" +
            $"Cl = {cl:F1}";
    }

    public float GetCL()
    {
        return cl;
    }


    //참고
    public float GetCLValue(float aoa)
    {
        return clCurve4.Evaluate(aoa + incidenceAngle);
    }
}
