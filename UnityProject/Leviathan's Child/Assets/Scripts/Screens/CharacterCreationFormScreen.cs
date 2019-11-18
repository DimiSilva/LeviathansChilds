using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationFormScreen : MonoBehaviour
{
    public Job selectedJob;
    public string characterName;
    public Element selectedElement;
    public Amulet selectedAmulet;

    public void Start()
    {
        RenderSelectedJobName();
        SetupElementsInDropdown();
        SetupAmuletsInDropdown();
    }

    private void RenderSelectedJobName() =>
        InitialSceneUIController.instance.CharacterCreationFormJobNameText.GetComponent<Text>().text = selectedJob.name;

    private void SetupElementsInDropdown()
    {
        List<Dropdown.OptionData> elements = new List<Dropdown.OptionData>();
        foreach (Element element in GameController.instance.elementsList)
        {
            elements.Add(new Dropdown.OptionData(element.name));
        }
        selectedElement = GameController.instance.elementsList[0];
        InitialSceneUIController.instance.CharacterCreationFormCharacterElementDropdown.GetComponent<Dropdown>().options = elements;
    }

    private void SetupAmuletsInDropdown()
    {
        List<Dropdown.OptionData> amulets = new List<Dropdown.OptionData>();
        foreach (Amulet amulet in GameController.instance.amuletsList)
        {
            amulets.Add(new Dropdown.OptionData(amulet.name));
        }
        selectedAmulet = GameController.instance.amuletsList[0];
        InitialSceneUIController.instance.CharacterCreationFormCharacterAmuletDropdown.GetComponent<Dropdown>().options = amulets;
    }

    public void OnChange_ElementsDropdown(int index) =>
        selectedElement = GameController.instance.elementsList[index];

    public void OnChange_AmuletsDropdown(int index) =>
        selectedAmulet = GameController.instance.amuletsList[index];

    public void OnPress_BackButton()
    {
        InitialSceneUIController.instance.characterSelectionScreen.SetActive(true);
        InitialSceneUIController.instance.characterCreationFormScreen.SetActive(false);
    }

    public void OnPress_CreateButton()
    {
        characterName = InitialSceneUIController.instance.CharacterCreationFormCharacterNameInput.GetComponent<InputField>().text;
        if (characterName.Length < 4) return;
        InitialSceneUIController.instance.CharacterCreationFormCreateCharacterButton.GetComponent<Button>().interactable = false;

        CharacterService.instance.Create(characterName, selectedJob, GameController.instance.GetLogedUser(), selectedElement, selectedAmulet);
        StartCoroutine(TreatCreateCharacterRequisition());
    }

    IEnumerator TreatCreateCharacterRequisition()
    {
        if (CharacterService.instance.processing)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(TreatCreateCharacterRequisition());
            yield break;
        }

        if (CharacterService.instance.responseText != null && CharacterService.instance.responseText.Length > 0)
        {
            InitialSceneUIController.instance.characterSelectionScreen.SetActive(true);
            InitialSceneUIController.instance.characterSelectionScreen.GetComponent<CharacterSelectionScreen>().GetUserCharacters();
            InitialSceneUIController.instance.CharacterCreationFormCreateCharacterButton.GetComponent<Button>().interactable = true;
            InitialSceneUIController.instance.characterCreationFormScreen.SetActive(false);
        }
        else
            InitialSceneUIController.instance.CharacterCreationFormCreateCharacterButton.GetComponent<Button>().interactable = true;
    }
}
