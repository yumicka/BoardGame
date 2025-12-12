using System.Collections.Generic;
using UnityEngine;

public class BoardOccupancy : MonoBehaviour
{
    public static BoardOccupancy Instance { get; private set; }

    private readonly Dictionary<int, List<PlayerMover>> occupants = new();

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 RegisterAndGetOffset(PlayerMover player, int cellIndex, float spread)
    {
        Unregister(player);

        if (!occupants.TryGetValue(cellIndex, out var list))
        {
            list = new List<PlayerMover>();
            occupants[cellIndex] = list;
        }

        list.Add(player);

        int slot = list.Count - 1;

        if (slot == 0) return Vector3.zero;

        float angle = slot * 45f * Mathf.Deg2Rad;
        float radius = spread * (1f + slot * 0.15f);

        return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
    }

    public void Unregister(PlayerMover player)
    {
        foreach (var kv in occupants)
        {
            if (kv.Value.Remove(player))
                break;
        }
    }
}
