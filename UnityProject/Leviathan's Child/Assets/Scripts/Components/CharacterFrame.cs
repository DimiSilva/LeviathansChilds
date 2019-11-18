using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFrame : MonoBehaviour
{
    public Character character { get; private set; }

    public void SetCharacter(Character character) => this.character = character;

    public void OnPress()
    {
        GameController.instance.SetSelectedCharacter(character);
        InitialSceneUIController.instance.lobbyScreen.SetActive(true);
        InitialSceneUIController.instance.characterSelectionScreen.SetActive(false);
    }
    public void OnPress_DeleteCharacterLink()
    {
        CharacterService.instance.Remove(character.id.ToString());
        StartCoroutine(TreatRemoveCharacterRequisition());
    }

    IEnumerator TreatRemoveCharacterRequisition()
    {
        if (CharacterService.instance.processing)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(TreatRemoveCharacterRequisition());
            yield break;
        }
        if (CharacterService.instance.responseText != null && CharacterService.instance.responseText.Length > 0)
        {
            InitialSceneUIController.instance.characterSelectionScreen.GetComponent<CharacterSelectionScreen>().GetUserCharacters();
        }
    }
}
