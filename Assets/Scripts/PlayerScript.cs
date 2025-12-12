using UnityEngine;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public GameObject spawnPoint;
    [SerializeField] private Transform boardPath; // сюда перетащи BoardPath из Hierarchy
    public static List<PlayerMover> SpawnedPlayers = new List<PlayerMover>();

    private int characterIndex;
    private int[] otherPlayers;
    private int index;

    private const string textFileName = "PlayerNames";

    void Start()
    {
        SpawnedPlayers.Clear();

        if (boardPath == null)
        {
            return;
        }

        // Собираем путь из детей BoardPath (Cell_0, Cell_1, ...)
        Transform[] path = BuildPathFromBoard(boardPath);

        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Главный игрок
        GameObject mainCharacter = Instantiate(
            playerPrefabs[characterIndex],
            spawnPoint.transform.position,
            Quaternion.identity
        );

        mainCharacter.GetComponent<NameScript>()
            .SetName(PlayerPrefs.GetString("PlayerName", "Jane Doe"));

        AssignPath(mainCharacter, path);

        // Остальные игроки
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 2);
        otherPlayers = new int[playerCount];

        string[] nameArray = ReadLinesFromFile(textFileName);

        for (int i = 0; i < otherPlayers.Length - 1; i++)
        {
            spawnPoint.transform.position += new Vector3(0.2f, 0, 0.08f);

            index = Random.Range(0, playerPrefabs.Length);

            GameObject otherPlayer = Instantiate(
                playerPrefabs[index],
                spawnPoint.transform.position,
                Quaternion.identity
            );

            if (nameArray.Length > 0)
            {
                otherPlayer.GetComponent<NameScript>()
                    .SetName(nameArray[Random.Range(0, nameArray.Length)]);
            }
            else
            {
                otherPlayer.GetComponent<NameScript>().SetName($"Player {i + 2}");
            }

            AssignPath(otherPlayer, path);
        }
    }

    private void AssignPath(GameObject playerObj, Transform[] path)
    {
        PlayerMover mover = playerObj.GetComponent<PlayerMover>();
        if (mover == null)
        {
            return;
        }

        mover.SetPath(path);

        if (!SpawnedPlayers.Contains(mover))
            SpawnedPlayers.Add(mover);

    }

    private Transform[] BuildPathFromBoard(Transform board)
    {
        Transform[] raw = board.GetComponentsInChildren<Transform>();

        if (raw.Length <= 1)
        {
            return new Transform[0];
        }

        Transform[] result = new Transform[raw.Length - 1];
        for (int i = 1; i < raw.Length; i++)
            result[i - 1] = raw[i];

        return result;
    }

    private string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' },
                System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogWarning("File not found: " + fileName);
            return new string[0];
        }
    }
}
