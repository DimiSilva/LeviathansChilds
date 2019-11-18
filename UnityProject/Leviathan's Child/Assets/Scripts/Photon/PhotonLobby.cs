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

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        // PhotonNetwork.AutomaticallySyncScene = true; 
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
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("failed to create room");
        CreateRoom();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("room created");
        // Debug.Log("")
    }
}
