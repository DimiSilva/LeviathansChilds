using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Manager : MonoBehaviour
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
