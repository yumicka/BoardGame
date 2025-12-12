using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeaderboardFile : MonoBehaviour
{
    public static LeaderboardFile Instance { get; private set; }

    [Serializable]
    public class Entry
    {
        public string name;
        public float timeSeconds;
        public string dateUtc; // чтобы было не просто "кто-то когда-то"
    }

    [Serializable]
    private class EntryList
    {
        public List<Entry> entries = new List<Entry>();
    }

    [SerializeField] private int maxEntries = 10;
    [SerializeField] private string fileName = "leaderboard.json";

    private EntryList data = new EntryList();

    public string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public IReadOnlyList<Entry> GetEntries() => data.entries;

    public void AddResult(string playerName, float timeSeconds)
    {
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Unknown";
        if (timeSeconds < 0f) timeSeconds = 0f;

        data.entries.Add(new Entry
        {
            name = playerName.Trim(),
            timeSeconds = timeSeconds,
            dateUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
        });

        data.entries.Sort((a, b) => a.timeSeconds.CompareTo(b.timeSeconds));

        if (data.entries.Count > maxEntries)
            data.entries.RemoveRange(maxEntries, data.entries.Count - maxEntries);

        Save();
    }

    public void Clear()
    {
        data.entries.Clear();
        Save();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
            Debug.Log($"Leaderboard saved: {FilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save leaderboard: " + e.Message);
        }
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                data = new EntryList();
                return;
            }

            string json = File.ReadAllText(FilePath);
            data = JsonUtility.FromJson<EntryList>(json) ?? new EntryList();
            if (data.entries == null) data.entries = new List<Entry>();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load leaderboard: " + e.Message);
            data = new EntryList();
        }
    }
}
