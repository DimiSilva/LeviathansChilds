using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationScreen : MonoBehaviour
{
    void Start()
    {
        RenderJobs();
    }

    private void RenderJobs()
    {
        foreach (Job job in GameController.instance.jobsList)
        {
            GameObject charFrame = Instantiate(InitialSceneUIController.instance.characterCreationFrame, InitialSceneUIController.instance.charactersCreationHolder.transform);
            charFrame.GetComponent<CharacterCreationFrame>().SetJob(job);
            charFrame.transform.GetChild(0).GetComponent<Image>().sprite = job.name == "Guerreiro" ? InitialSceneUIController.instance.warriorSprite : job.name == "Mago" ? InitialSceneUIController.instance.arcaneSprite : job.name == "Arqueiro" ? InitialSceneUIController.instance.archerSprite : null;
            charFrame.transform.GetChild(1).GetComponent<Text>().text = job.name;
            charFrame.SetActive(!GameController.instance.indisponibleCharacters.Contains(job.name));
        }
    }

    public void OnPress_BackButton()
    {
        InitialSceneUIController.instance.characterSelectionScreen.SetActive(true);
        InitialSceneUIController.instance.characterCreationScreen.SetActive(false);
    }
}
