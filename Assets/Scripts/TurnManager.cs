using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private DiceRollScript dice;
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text rolledText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winnerText;


    private List<PlayerMover> players;
    private int currentPlayer = 0;

    private bool wasLanded = false;
    private bool processing = false;
    private bool gameOver = false;

    private void Start()
    {
        if (dice == null) dice = FindFirstObjectByType<DiceRollScript>();

        // ∆дЄм 1 кадр, чтобы PlayerScript успел заспавнить игроков
        StartCoroutine(InitAfterSpawn());
    }

    private IEnumerator InitAfterSpawn()
    {
        yield return null;

        players = PlayerScript.SpawnedPlayers;

        if (players == null || players.Count == 0)
        {
            Debug.LogError("»гроки не заспавнились или на префабах нет PlayerMover.");
            yield break;
        }

        UpdateUI("?");
    }

    private void Update()
    {
        if (dice == null || processing || players == null || players.Count == 0 || gameOver) return;

        if (!wasLanded && dice.isLanded)
        {
            wasLanded = true;

            StartCoroutine(HandleDiceResult());
        }
        else if (wasLanded && !dice.isLanded)
        {
            wasLanded = false;
        }
    }

    private IEnumerator HandleDiceResult()
    {
        processing = true;

        if (!int.TryParse(dice.diceFaceNum, out int steps))
        {
            UpdateUI("?");
            processing = false;
            yield break;
        }

        UpdateUI(steps.ToString());

        dice.canRoll = false;
        yield return players[currentPlayer].MoveSteps(steps);
        dice.canRoll = true;


        if (players[currentPlayer].IsAtFinish())
        {
            DeclareWinner(players[currentPlayer]);
            yield break;
        }


        currentPlayer = (currentPlayer + 1) % players.Count;

        processing = false;
        UpdateUI(steps.ToString());
    }


    private void DeclareWinner(PlayerMover winner)
    {
        gameOver = true;

        string winnerName = winner.GetPlayerName();

        
        float time = RunTimer.Instance != null ? RunTimer.Instance.GetElapsedSeconds() : 0f;

        if (LeaderboardFile.Instance != null)
            LeaderboardFile.Instance.AddResult(winnerName, time);


        if (winnerText != null)
            winnerText.text = $"{winnerName} wins!";

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        int ms = Mathf.FloorToInt((seconds * 1000f) % 1000f);
        return $"{m:00}:{s:00}.{ms:000}";
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateUI(string rolled)
    {
        if (turnText != null)
        {
            string playerName = players[currentPlayer].GetPlayerName();
            turnText.text = $"It is {playerName}'s turn!";
        }

        if (rolledText != null)
            rolledText.text = rolled;
    }
}
