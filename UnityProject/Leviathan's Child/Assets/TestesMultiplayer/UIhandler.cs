using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class UIhandler : MonoBehaviourPunCallbacks
{
    public InputField createRoomIF;
    public InputField joinRoomIF;

    public void OnClick_JoinRoom()
    {
        Debug.Log($"Joining in room {joinRoomIF.text}");
        PhotonNetwork.JoinRoom(joinRoomIF.text, null);
    }

    public void OnClick_CreateRoom()
    {
        Debug.Log($"Creating room {createRoomIF.text}");
        PhotonNetwork.CreateRoom(createRoomIF.text, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined Success");
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room join failed: " + returnCode + "/" + message);
    }
}
