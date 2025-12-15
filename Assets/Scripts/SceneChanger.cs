using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public SaveLoadScript saveLoadScript;
    public FadeScript fadeScript;
    public void closeGame()
    {
        StartCoroutine(Delay("quit", -1, ""));
    }

    public IEnumerator Delay(string command, int characterIndex, string characterName)
    {
        if(string.Equals(command, "quit", System.StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.3f);
            PlayerPrefs.DeleteAll();

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else if(string.Equals(command, "play", System.StringComparison.OrdinalIgnoreCase))
        {
           
            yield return fadeScript.FadeOut(0.2f);
            saveLoadScript.SaveGame(characterIndex, characterName);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }else if(string.Equals(command, "menu", System.StringComparison.OrdinalIgnoreCase))
        {

            yield return fadeScript.FadeOut(0.2f);
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(Delay("menu", -1, ""));
    }
}
