using UnityEngine;
using TMPro;

public class QualityDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private const string QualityKey = "qualityLevel";

    private void Awake()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();

        InitOptions();
        InitValue();

        dropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    private void InitOptions()
    {
        dropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>(QualitySettings.names);
        dropdown.AddOptions(options);
    }

    private void InitValue()
    {
        int savedQuality = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
        savedQuality = Mathf.Clamp(savedQuality, 0, QualitySettings.names.Length - 1);

        dropdown.value = savedQuality;
        dropdown.RefreshShownValue();

        QualitySettings.SetQualityLevel(savedQuality);
    }

    private void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt(QualityKey, index);
    }
}
