using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class WaitingForPlayerScreen : MonoBehaviourPunCallbacks
{
    public void OnPress_BackButton()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        PhotonNetwork.Disconnect();
        PhotonLobby.instance.ConnectToMaster();
        InitialSceneUIController.instance.lobbyScreen.SetActive(true);
        InitialSceneUIController.instance.waitingForPlayerScreen.SetActive(false);
    }
}
