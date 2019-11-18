using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Manager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position, playerPrefab.transform.rotation);
    }
}
