using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionScreen : MonoBehaviourPunCallbacks
{
    void Start()
    {
        GetUserCharacters();
    }

    public void OnPress_CreateCharacterButton()
    {
        if (GameController.instance.userCharacters.Count >= 3) return;
        InitialSceneUIController.instance.characterCreationScreen.SetActive(true);
        InitialSceneUIController.instance.characterSelectionScreen.SetActive(false);
    }

    public void GetUserCharacters()
    {
        CharacterService.instance.GetAllForUser(GameController.instance.GetLogedUser().id.ToString());
        StartCoroutine(TreatGetAllForUserRequisition());
    }

    IEnumerator TreatGetAllForUserRequisition()
    {
        if (CharacterService.instance.processing)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(TreatGetAllForUserRequisition());
            yield break;
        }

        if (CharacterService.instance.responseText != null && CharacterService.instance.responseText.Length > 0)
        {
            List<Character> characters = new List<Character>();
            foreach (JSONObject character in CharacterService.instance.responseJson.list)
            {
                string id = character.GetField("id").ToString().Replace("\"", "");
                string name = character.GetField("name").ToString().Replace("\"", "");
                Job job = GameController.instance.jobsList.Find(jobInList => jobInList.id.ToString() == character.GetField("job").ToString().Replace("\"", ""));
                Element element = GameController.instance.elementsList.Find(elementInList => elementInList.id.ToString() == character.GetField("element").ToString().Replace("\"", ""));
                Amulet amulet = GameController.instance.amuletsList.Find(amuletInList => amuletInList.id.ToString() == character.GetField("amulet").ToString().Replace("\"", ""));
                int amuletLevel = int.Parse(character.GetField("amuletLevel").ToString().Replace("\"", ""));
                int amuletExperience = int.Parse(character.GetField("amuletExperience").ToString().Replace("\"", ""));
                float hp = float.Parse(character.GetField("hp").ToString().Replace("\"", ""));
                float strength = float.Parse(character.GetField("strength").ToString().Replace("\"", ""));
                float agility = float.Parse(character.GetField("agility").ToString().Replace("\"", ""));
                float intelligence = float.Parse(character.GetField("intelligence").ToString().Replace("\"", ""));
                int battlesNumber = int.Parse(character.GetField("battlesNumber").ToString().Replace("\"", ""));
                int victorysNumber = int.Parse(character.GetField("victorysNumber").ToString().Replace("\"", ""));
                int losesNumber = int.Parse(character.GetField("losesNumber").ToString().Replace("\"", ""));
                int battleTimeInSeconds = int.Parse(character.GetField("battleTimeInSeconds").ToString().Replace("\"", ""));
                int xp = int.Parse(character.GetField("xp").ToString().Replace("\"", ""));
                int xpToUp = int.Parse(character.GetField("xpToUp").ToString().Replace("\"", ""));
                int level = int.Parse(character.GetField("level").ToString().Replace("\"", ""));
                characters.Add(new Character(id, name, job, element, amulet, amuletLevel, amuletExperience, hp, strength, agility, intelligence, battlesNumber, victorysNumber, losesNumber, battleTimeInSeconds, xp, xpToUp, level));
            }
            GameController.instance.SetUserCharacters(characters);
            RenderUserCharactersFrames();
        }
    }

    private void RenderUserCharactersFrames()
    {
        int holderChilds = InitialSceneUIController.instance.charactersHolder.transform.childCount;
        GameObject holder = InitialSceneUIController.instance.charactersHolder;
        for (int i = 0; i < holderChilds; i++)
            GameObject.Destroy(holder.transform.GetChild(i).gameObject);

        foreach (Character character in GameController.instance.userCharacters)
        {
            Job characterJob = character.job;
            GameObject charFrame = Instantiate(InitialSceneUIController.instance.characterFrame, holder.transform);
            charFrame.GetComponent<CharacterFrame>().SetCharacter(character);
            charFrame.transform.GetChild(0).GetComponent<Image>().sprite = characterJob.name == "Guerreiro" ? InitialSceneUIController.instance.warriorSprite : characterJob.name == "Mago" ? InitialSceneUIController.instance.arcaneSprite : characterJob.name == "Arqueiro" ? InitialSceneUIController.instance.archerSprite : null;
            charFrame.transform.GetChild(2).GetComponent<Text>().text = character.name;
        }
    }
}
