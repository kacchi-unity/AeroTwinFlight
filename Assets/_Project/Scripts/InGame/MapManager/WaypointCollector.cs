using TMPro;
using UnityEngine;

public class WaypointCollector : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField waypointInputField;
    [SerializeField] private TMP_InputField latitudeInputField;
    [SerializeField] private TMP_InputField longitudeInputField;
    [SerializeField] private TMP_InputField heightInputField;

    private readonly bool isBaseStation = false;

    public WaypointData GetWaypointData()
    {
        string nameInputField = string.IsNullOrEmpty(waypointInputField.text) ? "Next Point" : waypointInputField.text;

        bool isLatitudeValid = double.TryParse(latitudeInputField.text, out double latitude);
        bool isLongitudeValid = double.TryParse(longitudeInputField.text, out double longitude);
        bool isHeightValid = double.TryParse(heightInputField.text, out double height);

        bool isValid = isLatitudeValid && isLongitudeValid && isHeightValid;

        return new WaypointData(
            nameInputField,
            latitude,
            longitude,
            height,
            isValid,
            isBaseStation //IsBaseState -> false
        );

    }
}
