using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class TestesMultiplayer : MonoBehaviourPunCallbacks
{

    public GameObject connectPanel;
    public GameObject connectedPanel;
    public GameObject disconnectedPanel;

    public void OnClick_ConnectBtn()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClick_DisconnectBtn()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected, cause: {cause.ToString()}");
        connectedPanel.SetActive(false);
        disconnectedPanel.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined in Lobby");
        connectPanel.SetActive(false);
        connectedPanel.SetActive(true);
    }
}
