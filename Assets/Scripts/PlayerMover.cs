using System.Collections;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private Transform[] path;   
    [SerializeField] private float speed = 4f;
    [SerializeField] private float heightOffset = 0.5f;
    public int CellIndex { get; private set; } = 0;
    public bool IsMoving { get; private set; } = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetPath(Transform[] newPath)
    {
        path = newPath;
        SetToStart();
    }

    public void SetToStart()
    {
        CellIndex = 0;
        if (path != null && path.Length > 0 && path[0] != null)
            transform.position = path[0].position + Vector3.up * heightOffset;

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
            Vector3 target = path[CellIndex].position + Vector3.up * heightOffset;

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    speed * Time.deltaTime
                );
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

}
