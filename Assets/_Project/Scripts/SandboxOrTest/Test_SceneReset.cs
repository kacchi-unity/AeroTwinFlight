using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_SceneReset : MonoBehaviour
{
    [SerializeField] private KeyCode resetKey;
    void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}