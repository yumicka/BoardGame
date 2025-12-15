using UnityEngine;
using TMPro;

public class QualityDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private const string QualityKey = "qualityLevel";
    private int initialQuality;

    private void Awake()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();

        InitOptions();

        int savedQuality = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
        savedQuality = Mathf.Clamp(savedQuality, 0, QualitySettings.names.Length - 1);

        initialQuality = savedQuality;

        dropdown.value = savedQuality;
        dropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(savedQuality);

        dropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    private void InitOptions()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
    }

    private void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt(QualityKey, index);
    }

    public void ResetSettings()
    {
        dropdown.SetValueWithoutNotify(initialQuality);
        dropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(initialQuality);
        PlayerPrefs.SetInt(QualityKey, initialQuality);
    }
}
