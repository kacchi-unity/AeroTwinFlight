using TMPro;
using UnityEngine;

public class HeightUI : MonoBehaviour
{
    [SerializeField] private HeightCalculator heightCalculator;
    [SerializeField] private TextMeshProUGUI heightText;

    private void Update()
    {
        if (heightText != null)
        {
            heightText.text = $"고도: {heightCalculator.GetCurrentHeight():F1} m";
        }
    }
}
