using TMPro;
using UnityEngine;

public class ChooseCharacterScript : MonoBehaviour
{
    public GameObject[] characters;
    int characterIndex;

    public GameObject inputField;
    public GameObject playerCountInput;
    string characterName;
    string playerCount;
    public SceneChanger sceneChanger;

    private void Awake()
    {
        characterIndex = 0;
        foreach(GameObject character in characters)
        {
            character.SetActive(false);
        }

        characters[characterIndex].SetActive(true);
    }

    public void NextCharacter()
    {
        characters[characterIndex].SetActive(false);
        characterIndex++;
        if(characterIndex == characters.Length)
        {
            characterIndex = 0;
        }
        characters[characterIndex].SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[characterIndex].SetActive(false);
        characterIndex--;
        if (characterIndex == -1)
        {
            characterIndex = characters.Length - 1;
        }
        characters[characterIndex].SetActive(true);
    }

    //public void Play()
    //{
    //    characterName = inputField.GetComponent<TMPro.TMP_InputField>().text;
    //    playerCount = playerCountInput.GetComponent<TMPro.TMP_InputField>();
    //    if (characterName.Length >= 3)
    //    {
    //        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
    //        PlayerPrefs.SetString("PlayerName", characterName);
    //        PlayerPrefs.SetInt("PlayerCount", playerCount);
    //        StartCoroutine(sceneChanger.Delay("play", characterIndex, characterName));
    //    }
    //    else
    //    {
    //        inputField.GetComponent<TMPro.TMP_InputField>().Select();
    //    }

    //    if (!int.TryParse(playerCount, out int playerCount))
    //    {
    //        playerCountInput.Select();
    //        playerCountInput.ActivateInputField();
    //        return;
    //    }
    //}

    public void Play()
    {


        TMP_InputField nameField = inputField.GetComponent<TMP_InputField>();
        TMP_InputField countField = playerCountInput.GetComponent<TMP_InputField>();

        string characterName = nameField.text.Trim();
        string playersText = countField.text.Trim();
 
        if (characterName.Length < 3)
        {
            nameField.Select();
            nameField.ActivateInputField();
            return;
        }


        if (!int.TryParse(playersText, out int playerCount))
        {
            countField.Select();
            countField.ActivateInputField();
            return;
        }

        // (опционально) ограничения
        if (playerCount <= 1 || playerCount > 7)
        {
            countField.Select();
            countField.ActivateInputField();
            return;
        }

        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.SetString("PlayerName", characterName);
        PlayerPrefs.SetInt("PlayerCount", playerCount);
        PlayerPrefs.Save();

        StartCoroutine(sceneChanger.Delay("play", characterIndex, characterName));
    }

}
