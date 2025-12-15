using UnityEngine;
using UnityEngine.UI;

public class FullScreenToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private const string FullScreenKey = "fullscreen";
    private bool initialFullScreen;

    private void Start()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();

        bool saved = PlayerPrefs.GetInt(FullScreenKey, 1) == 1;

        initialFullScreen = saved;
        toggle.isOn = saved;
        Screen.fullScreen = saved;

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt(FullScreenKey, value ? 1 : 0);
    }

    public void ResetSettings()
    {
        toggle.SetIsOnWithoutNotify(initialFullScreen);
        Screen.fullScreen = initialFullScreen;
        PlayerPrefs.SetInt(FullScreenKey, initialFullScreen ? 1 : 0);
    }
}
