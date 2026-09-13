using UnityEngine;
using UnityEngine.UI;
using System;

public class InGameButtonManager : MonoBehaviour
{
    [Header("Button UI Inspector")]
    [Tooltip("버튼을 연결하세요")]
    [SerializeField] private Button calibrateButton;

    public static event Action onCalibrateButtonClicked;

    private void OnEnable()
    {
        calibrateButton.onClick.AddListener(ProcessTiltButton);
    }

    private void OnDisable()
    {
        calibrateButton.onClick.RemoveListener(ProcessTiltButton);
    }

    void ProcessTiltButton()
    {
        onCalibrateButtonClicked?.Invoke();
    }
}
