using UnityEngine;
using System;

public class TriggerDetector : MonoBehaviour
{
    private LayerMask targetLayer;
    public bool IsTouchingTarget { get; private set; } = false;

    public event Action<bool> TouchStateChanged;

    //외부에서 호출 (설정 주입, 초기화)
    public void Initialize(LayerMask layerMask)
    {
        targetLayer = layerMask;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            IsTouchingTarget = true;
            TouchStateChanged?.Invoke(IsTouchingTarget);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0 )
        {
            IsTouchingTarget = false;
            TouchStateChanged?.Invoke(IsTouchingTarget);
        }
    }
}
