using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    [SerializeField] private Material[] skyboxMaterials;
    [SerializeField] private KeyCode skyboxChangeKeyCode = KeyCode.Alpha1;

    private int currentIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(skyboxChangeKeyCode))
        {
            ChangeNextSkybox();
        }
    }

    private void ChangeNextSkybox()
    {
        if (skyboxMaterials == null || skyboxMaterials.Length == 0)
        {
            return;
        }
        
        currentIndex = (currentIndex + 1) % skyboxMaterials.Length;

        Material targetMaterial = skyboxMaterials[currentIndex];

        RenderSettings.skybox = targetMaterial;
    }
}
