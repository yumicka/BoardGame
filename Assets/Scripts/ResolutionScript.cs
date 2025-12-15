using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ResolutionSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private const string ResolutionKey = "ResolutionIndex";

    private readonly Vector2Int[] allowedResolutions =
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160),
    };

    private List<Vector2Int> availableResolutions;

    private void Start()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();

        BuildAvailableResolutions();
        InitDropdown();
        InitValue();

        dropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    // 1?? Сравниваем allowedResolutions с тем, что реально поддерживается
    private void BuildAvailableResolutions()
    {
        var systemResolutions = Screen.resolutions
            .Select(r => new Vector2Int(r.width, r.height))
            .Distinct()
            .ToHashSet();

        availableResolutions = allowedResolutions
            .Where(r => systemResolutions.Contains(r))
            .ToList();

        // fallback, если вдруг вообще ничего не совпало
        if (availableResolutions.Count == 0)
        {
            var current = Screen.currentResolution;
            availableResolutions.Add(new Vector2Int(current.width, current.height));
        }
    }

    // 2?? Заполняем dropdown
    private void InitDropdown()
    {
        dropdown.ClearOptions();

        var options = new List<string>();
        foreach (var r in availableResolutions)
            options.Add($"{r.x} x {r.y}");

        dropdown.AddOptions(options);
    }

    // 3?? Выбираем стартовое значение
    private void InitValue()
    {
        int saved = PlayerPrefs.GetInt(ResolutionKey, -1);

        if (saved >= 0 && saved < availableResolutions.Count)
        {
            ApplyResolution(saved);
            dropdown.SetValueWithoutNotify(saved);
            return;
        }

        // дефолт — Full HD
        int fullHdIndex = availableResolutions.FindIndex(r => r.x == 1920 && r.y == 1080);
        if (fullHdIndex < 0)
            fullHdIndex = availableResolutions.Count - 1;

        ApplyResolution(fullHdIndex);
        dropdown.SetValueWithoutNotify(fullHdIndex);
        PlayerPrefs.SetInt(ResolutionKey, fullHdIndex);
    }

    // 4?? Применяем разрешение
    private void ApplyResolution(int index)
    {
        var r = availableResolutions[index];
        Screen.SetResolution(r.x, r.y, Screen.fullScreen);
    }

    // 5?? Реакция на выбор пользователя
    private void OnResolutionChanged(int index)
    {
        ApplyResolution(index);
        PlayerPrefs.SetInt(ResolutionKey, index);
    }
}
