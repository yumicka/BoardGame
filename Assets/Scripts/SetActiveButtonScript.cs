using System.Collections;
using UnityEngine;

public class SetActiveButtonScript : MonoBehaviour
{
    public GameObject[] allScreens;       
    public GameObject targetScreen;

    public void ToggleActiveAfterDelay(float delay)
    {
        StartCoroutine(ToggleActiveCoroutine(delay));
    }

    private IEnumerator ToggleActiveCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        
        foreach (var screen in allScreens)
        {
            if (screen != null)
                screen.SetActive(false);
        }

        if (targetScreen != null)
            targetScreen.SetActive(true);
    }
}
