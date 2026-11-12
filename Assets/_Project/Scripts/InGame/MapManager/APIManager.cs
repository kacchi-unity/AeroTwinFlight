using UnityEngine;
using CesiumForUnity;
using System.IO;

[System.Serializable]
public class MapConfig
{
    public string apiKey;
}

public class APIManager : MonoBehaviour
{
    [Tooltip("Cesium3DTileset 오브젝트 연결")]
    public Cesium3DTileset cesium3DTileset;

    void Start()
    {
        LoadAndApplyApiKey();
    }

    void LoadAndApplyApiKey()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("API/MapConfig");

        if (jsonFile == null)
        {
            Debug.LogError("MapConfig.json을 찾을 수 없습니다. Assets/Resources/ 폴더를 확인");
            return;
        }

        MapConfig config = JsonUtility.FromJson<MapConfig>(jsonFile.text);

        if (string.IsNullOrEmpty(config.apiKey) || config.apiKey.Contains("YOUR_GOOGLE_MAPS_API_KEY"))
        {
            Debug.LogError("유효한 Google API 키가 설정되지 않았습니다.");
            return;
        }

        if (cesium3DTileset != null)
        {
            cesium3DTileset.url = $"https://tile.googleapis.com/v1/3dtiles/root.json?key={config.apiKey}";
            Debug.Log("Google Photorealistic 3D Tiles URL이 성공적으로 설정되었습니다.");
        }
        else
        {
            Debug.LogError("Cesium3DTileset 컴포넌트가 연결되지 않았습니다.");
        }
    }
}