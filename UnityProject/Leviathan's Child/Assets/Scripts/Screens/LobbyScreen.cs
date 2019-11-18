using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreen : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonLobby.instance.ConnectToMaster();
    }

    public void OnPress_EnterRoomButton()
    {
        InitialSceneUIController.instance.waitingForPlayerScreen.SetActive(true);
        PhotonLobby.instance.TryEnterArena();
        InitialSceneUIController.instance.lobbyScreen.SetActive(false);
    }

    public void OnPress_OptionsButton()
    {
        InitialSceneUIController.instance.optionsScreen.SetActive(true);
        InitialSceneUIController.instance.lobbyScreen.SetActive(false);
    }

    public void OnPress_ExitButton()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }

}
