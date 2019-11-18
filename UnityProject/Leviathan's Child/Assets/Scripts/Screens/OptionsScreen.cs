using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsScreen : MonoBehaviour
{
    public void OnPress_BackButton()
    {
        InitialSceneUIController.instance.lobbyScreen.SetActive(true);
        InitialSceneUIController.instance.optionsScreen.SetActive(false);
    }
}
