using System.Collections;
using UnityEngine;

public class SetActiveButtonScript : MonoBehaviour
{
    public GameObject[] targetObjects;
    public void ToggleActiveAfterDelay(float delay)
    {
        StartCoroutine(ToggleActiveCoroutine(delay));
    }

    private IEnumerator ToggleActiveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var obj in targetObjects)
        {
            if (obj != null)
                obj.SetActive(!obj.activeSelf);
        }

        gameObject.SetActive(!gameObject.activeSelf);
    }
}
