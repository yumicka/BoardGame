using System.Collections;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private Transform[] path;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private float spread = 0.25f;
    public int CellIndex { get; private set; } = 0;
    public bool IsMoving { get; private set; } = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetPath(Transform[] newPath)
    {
        path = newPath;
        SetToStart();
    }

    public void SetToStart()
    {
        CellIndex = 0;

        if (path == null || path.Length == 0 || path[0] == null)
            return;

        transform.position = GetCellPosition(CellIndex);
    }

    public string GetPlayerName()
    {
        NameScript nameScript = GetComponent<NameScript>();
        if (nameScript != null && !string.IsNullOrEmpty(nameScript.PlayerName))
            return nameScript.PlayerName;

        return gameObject.name;
    }

    private Vector3 GetCellPosition(int cellIndex)
    {
        if (path == null || path.Length == 0) return transform.position;
        cellIndex = Mathf.Clamp(cellIndex, 0, path.Length - 1);

        Vector3 basePos = path[cellIndex].position + Vector3.up * heightOffset;

        Vector3 offset = Vector3.zero;
        if (BoardOccupancy.Instance != null)
            offset = BoardOccupancy.Instance.RegisterAndGetOffset(this, cellIndex, spread);

        return basePos + offset;
    }

    public bool IsAtFinish()
    {
        return path != null && path.Length > 0 && CellIndex >= path.Length - 1;
    }


    public IEnumerator MoveSteps(int steps)
    {
        if (path == null || path.Length == 0) yield break;

        IsMoving = true;

        if (animator != null)
            animator.SetBool("walk", true);

        for (int i = 0; i < steps; i++)
        {
            int next = Mathf.Min(CellIndex + 1, path.Length - 1);
            if (next == CellIndex) break;

            CellIndex = next;
            Vector3 target = GetCellPosition(CellIndex);

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
        }

        IsMoving = false;

        if (animator != null)
        {
            animator.SetBool("walk", false);
            animator.SetBool("idle", true);
        }
    }

    public IEnumerator DieAndReturnToStart(float dieAnimTime = 1.0f)
    {
        IsMoving = true;

        if (animator != null)
        {
            animator.SetBool("walk", false);
            animator.SetBool("idle", false);

            // Лучше триггером, чем bool'ом
            animator.SetTrigger("die"); // сделай Trigger "die" в Animator
        }

        // ждём пока проиграется смерть
        yield return new WaitForSeconds(dieAnimTime);

        // телепорт на старт
        CellIndex = 0;

        // важно: пересчитать позицию через GetCellPosition, чтобы сохранились оффсеты от BoardOccupancy
        transform.position = GetCellPosition(CellIndex);

        if (animator != null)
        {
            animator.ResetTrigger("die");
            animator.SetBool("idle", true);
        }

        IsMoving = false;
    }

}
