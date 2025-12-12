using System.Text;
using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text leaderboardText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (leaderboardText == null) return;

        var lb = LeaderboardFile.Instance;
        if (lb == null)
        {
            leaderboardText.text = "Leaderboard not available.";
            return;
        }

        var entries = lb.GetEntries();
        if (entries.Count == 0)
        {
            leaderboardText.text = "No records yet.";
            return;
        }

        var sb = new StringBuilder();
        for (int i = 0; i < entries.Count; i++)
        {
            sb.Append($"{i + 1}. {entries[i].name}  -  {FormatTime(entries[i].timeSeconds)}\n");
        }

        leaderboardText.text = sb.ToString();
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        int ms = Mathf.FloorToInt((seconds * 1000f) % 1000f);
        return $"{m:00}:{s:00}.{ms:000}";
    }
}
