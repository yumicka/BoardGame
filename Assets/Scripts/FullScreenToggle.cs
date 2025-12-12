using UnityEngine;
using UnityEngine.UI;

public class FullScreenToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private const string FullScreenKey = "fullscreen";

    private void Start()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();

        bool isFullScreen = PlayerPrefs.GetInt(FullScreenKey, 1) == 1;

        toggle.isOn = isFullScreen;
        Screen.fullScreen = isFullScreen;

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt(FullScreenKey, value ? 1 : 0);
    }
}
