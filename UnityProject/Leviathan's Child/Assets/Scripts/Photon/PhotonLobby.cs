using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby instance;

    void Awake()
    {
        instance = this;
    }

    public void ConnectToMaster()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "sa";
        PhotonNetwork.GameVersion = "1.0.0";
        Debug.Log("connecting to master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (InitialSceneUIController.instance.enterRoomButton_Lobby.GetComponent<Button>().interactable)
            InitialSceneUIController.instance.enterRoomButton_Lobby.GetComponent<Button>().interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        base.OnConnectedToMaster();
        StartCoroutine(WaitServerBeReadyToContinueConnection());
    }

    IEnumerator WaitServerBeReadyToContinueConnection()
    {
        yield return new WaitForSeconds(1);
        InitialSceneUIController.instance.enterRoomButton_Lobby.GetComponent<Button>().interactable = true;
    }

    public void TryEnterArena()
    {
        Debug.Log("trying to find a random room");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("failed to find a random room");
        CreateRoom();
    }

    public void CreateRoom()
    {
        Debug.Log("creating room");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("failed to create room");
        base.OnCreateRoomFailed(returnCode, message);
        CreateRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("room created");
        base.OnCreatedRoom();
    }
}
