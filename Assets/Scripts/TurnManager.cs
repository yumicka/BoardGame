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
    private int totalStepsThisRun = 0;
    private int baseScore = 10000;
    private float timePenalty = 20f;
    private int stepPenalty = 50;
    private float runStartTime;

    private bool wasLanded = false;
    private bool processing = false;
    private bool gameOver = false;

    private Dictionary<PlayerMover, int> stepsByPlayer = new Dictionary<PlayerMover, int>();

    private void Start()
    {
        if (dice == null) dice = FindFirstObjectByType<DiceRollScript>();
        StartCoroutine(InitAfterSpawn());
    }

    private IEnumerator InitAfterSpawn()
    {
        yield return null;

        players = PlayerScript.SpawnedPlayers;

        if (players == null || players.Count == 0)
        {
            Debug.LogError("Игроки не заспавнились или на префабах нет PlayerMover.");
            yield break;
        }

        runStartTime = Time.time;

        stepsByPlayer.Clear();
        foreach (var p in players)
            stepsByPlayer[p] = 0;

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


        var p = players[currentPlayer];
        stepsByPlayer[p] += steps;

        var landedCell = GetCurrentCellTransform(p);
        if (landedCell != null)
        {
            var death = landedCell.GetComponent<DeathCell>();
            if (death != null)
            {
                yield return p.DieAndReturnToStart(death.DieAnimTime);
            }
        }

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

    private Transform GetCurrentCellTransform(PlayerMover mover)
    {
        
        var board = GameObject.Find("Path");
        if (board == null)
            return null;

        if (mover.CellIndex < 0 || mover.CellIndex >= board.transform.childCount) 
            return null;

        return board.transform.GetChild(mover.CellIndex);
    }



    private void DeclareWinner(PlayerMover winner)
    {
        gameOver = true;

        string winnerName = winner.GetPlayerName();

        float time = Time.time - runStartTime;

        int winnerSteps = stepsByPlayer.TryGetValue(winner, out var s) ? s : 0;

        int score = Mathf.Max(0,
            Mathf.RoundToInt(baseScore - time * timePenalty - winnerSteps * stepPenalty));

        LeaderboardFile.Instance?.AddResult(winnerName, time, winnerSteps, score);


        if (winnerText != null)
            if (winnerText != null)
                winnerText.text = $"{winnerName} wins!\nTime: {time:0.00}s\nSteps: {winnerSteps}\nScore: {score}";

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
