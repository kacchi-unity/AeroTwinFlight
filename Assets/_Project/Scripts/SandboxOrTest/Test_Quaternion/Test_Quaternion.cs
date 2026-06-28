using Unity.VisualScripting;
using UnityEngine;
using System;

public class AirplaneController : MonoBehaviour
{
    //누적할 오일러 각
    private Vector3 currentEulerAngles = new Vector3(-90f,0f,0f);
    private float rotationSpeed = 45f;
    [Header("Test Type")]
    [SerializeField] private bool useQuaternion = false;

    void Update()
    {
        if (!useQuaternion)
        {
            if (Input.GetKey(KeyCode.Q)) currentEulerAngles.y += 45f * Time.deltaTime;
            if (Input.GetKey(KeyCode.E)) currentEulerAngles.y -= 45f * Time.deltaTime;

            if (Input.GetKey(KeyCode.W)) currentEulerAngles.x += 45f * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) currentEulerAngles.x -= 45f * Time.deltaTime;

            if (Input.GetKey(KeyCode.A)) currentEulerAngles.z += 45f * Time.deltaTime;
            if (Input.GetKey(KeyCode.D)) currentEulerAngles.z -= 45f * Time.deltaTime;

            transform.eulerAngles = currentEulerAngles;
        }
        else
        {
            float xSpeed = 0f;
            float ySpeed = 0f;
            float zSpeed = 0f;

            if (Input.GetKey(KeyCode.W)) xSpeed = rotationSpeed;
            if (Input.GetKey(KeyCode.S)) xSpeed = -rotationSpeed;

            if (Input.GetKey(KeyCode.Q)) ySpeed = rotationSpeed;
            if (Input.GetKey(KeyCode.E)) ySpeed = -rotationSpeed;

            if (Input.GetKey(KeyCode.A)) zSpeed = rotationSpeed;
            if (Input.GetKey(KeyCode.D)) zSpeed = -rotationSpeed;

            Quaternion deltaRotation = Quaternion.Euler(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, zSpeed * Time.deltaTime);
            transform.rotation = transform.rotation * deltaRotation;
        }
    }
}