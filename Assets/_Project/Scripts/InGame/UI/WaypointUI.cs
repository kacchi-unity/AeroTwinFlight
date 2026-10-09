using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WaypointUI : MonoBehaviour
{
    [Header("UI Connection")]
    [SerializeField] private CanvasGroup confirmButtonCanvasGroup;
    [SerializeField] private Toggle waypointToggle;
    [SerializeField] private CanvasGroup waypointScrollCanvasGroup;
    [SerializeField] private TextMeshProUGUI waypointText;

    [Header("Component Connection")]
    [SerializeField] private GeoMapGenerator geoMapGenerator;

    [Header("Set Base Station Name")]
    [SerializeField] private string baseStationName = "Base Station";

    private void OnEnable()
    {
        waypointToggle.onValueChanged.AddListener(OnToggleValueChanged);
        geoMapGenerator.ChangedWaypoint += UpdateWaypointUI;
        geoMapGenerator.ChangedWaypoint += UpdateConfirmButtonState;
    }

    private void OnDisable()
    {
        waypointToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        geoMapGenerator.ChangedWaypoint -= UpdateWaypointUI;
        geoMapGenerator.ChangedWaypoint -= UpdateConfirmButtonState;
    }

    private void Start()
    {
        WaypointData baseStateData = new WaypointData(
                baseStationName,
                0,
                0,
                0,
                true,
                true
            );

        UpdateWaypointUI(baseStateData);

        UpdateConfirmButtonState(baseStateData);

        OnToggleValueChanged(waypointToggle.isOn);
    }

    private void UpdateWaypointUI(WaypointData targetData)
    {
        waypointText.text = $"{targetData.WaypointName}";
    }

    private void UpdateConfirmButtonState(WaypointData targetData)
    {
        //Is active only targetData is Base Station
        bool isButtonActive = targetData.IsBaseStation;

        confirmButtonCanvasGroup.interactable = isButtonActive;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        waypointScrollCanvasGroup.alpha = isOn ? 1f : 0f;

        waypointScrollCanvasGroup.blocksRaycasts = isOn;

        waypointScrollCanvasGroup.interactable = isOn;
    }
}
