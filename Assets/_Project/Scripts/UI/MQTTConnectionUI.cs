using UnityEngine;

public class MQTTConnectionUI : MonoBehaviour
{
    [SerializeField] private MQTTManager mQTTManager;
    [SerializeField] private CanvasGroup mQTTDisconnectedGUI;
    [SerializeField] private CanvasGroup messageDisconnectedGUI;

    private void OnEnable()
    {
        mQTTManager.ChangedMQTTConnection += ShowConnectionErrorUI;
    }

    private void OnDisable()
    {
        mQTTManager.ChangedMQTTConnection -= ShowConnectionErrorUI;
    }

    private void ShowConnectionErrorUI(ConnectionChanged targetState)
    {
        switch (targetState)
        {
            case ConnectionChanged.Connected:
                mQTTDisconnectedGUI.alpha = 0;
                messageDisconnectedGUI.alpha = 0;
                break;

            case ConnectionChanged.MQTTDisconnected:
                mQTTDisconnectedGUI.alpha = 1;
                messageDisconnectedGUI.alpha = 0;
                break;

            case ConnectionChanged.MessageDisconnected:
                mQTTDisconnectedGUI.alpha = 0;
                messageDisconnectedGUI.alpha = 1;
                break;

            default:
                break;
        }
    }
}
