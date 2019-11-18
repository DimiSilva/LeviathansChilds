using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationFrame : MonoBehaviour
{
    public Job job { get; private set; }

    public void SetJob(Job job) => this.job = job;

    public void OnPress()
    {
        InitialSceneUIController.instance.characterCreationFormScreen.SetActive(true);
        InitialSceneUIController.instance.characterCreationFormScreen.GetComponent<CharacterCreationFormScreen>().selectedJob = job;
        InitialSceneUIController.instance.characterCreationScreen.SetActive(false);
    }
}
