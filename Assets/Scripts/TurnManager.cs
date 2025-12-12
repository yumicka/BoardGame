using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private DiceRollScript dice;
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text rolledText;

    private List<PlayerMover> players;
    private int currentPlayer = 0;

    private bool wasLanded = false;
    private bool processing = false;

    private void Start()
    {
        if (dice == null) dice = FindFirstObjectByType<DiceRollScript>();

        // Ждём 1 кадр, чтобы PlayerScript успел заспавнить игроков
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

        UpdateUI("?");
    }

    private void Update()
    {
        if (dice == null || processing || players == null || players.Count == 0) return;

        // ловим момент: НЕ приземлилась -> приземлилась
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

        yield return players[currentPlayer].MoveSteps(steps);

        currentPlayer = (currentPlayer + 1) % players.Count;

        processing = false;
        UpdateUI(steps.ToString());
    }

    private void UpdateUI(string rolled)
    {
        if (turnText != null)
            turnText.text = $"Ход: Игрок {currentPlayer + 1}";

        if (rolledText != null)
            rolledText.text = rolled;
    }
}
