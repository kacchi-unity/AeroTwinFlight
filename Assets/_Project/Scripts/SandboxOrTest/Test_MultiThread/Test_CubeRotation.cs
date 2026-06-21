using UnityEngine;
using System;

public class Test_CubeRotation : MonoBehaviour
{
    [SerializeField] private Transform cubeTransform;
    void Update()
    {
        cubeTransform.Rotate(0f, 0f, 200* Time.deltaTime);
    }
}
