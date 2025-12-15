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
        public int steps;
        public int score;
        public string dateUtc;
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

    public void AddResult(string playerName, float timeSeconds, int steps, int score)
    {
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Unknown";
        if (timeSeconds < 0f) timeSeconds = 0f;
        if (steps < 0) steps = 0;

        data.entries.Add(new Entry
        {
            name = playerName.Trim(),
            timeSeconds = timeSeconds,
            steps = steps,
            score = score,
            dateUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
        });

        // ?? Сортируем по очкам (больше = лучше), при равенстве по времени (меньше = лучше)
        data.entries.Sort((a, b) =>
        {
            int byScore = b.score.CompareTo(a.score);
            if (byScore != 0) return byScore;
            return a.timeSeconds.CompareTo(b.timeSeconds);
        });

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
