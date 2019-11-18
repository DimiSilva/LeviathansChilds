using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonRoom instance;
    private PhotonView PV;

    // public bool isGameLoaded;
    public int currentScene;
    public int multiplayerScene;

    // Player info;
    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    // public int playersInGame;

    private bool playerInArena = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else
        {
            if (instance != this)
            {
                Destroy(instance.gameObject);
                instance = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
        PV = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("joined in room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = GameController.instance.selectedCharacter.name;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            StartGame();
        // DebugRoomInfos();
    }

    public void DebugRoomInfos()
    {
        if (!PhotonNetwork.InRoom) { Debug.Log("not in room"); return; }
        Debug.Log($"in room: {PhotonNetwork.InRoom}");
        Debug.Log($"room name: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"players count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        Debug.Log($"my number in room: " + myNumberInRoom);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 2)
            StartGame();
    }

    public void StartGame() => PhotonNetwork.LoadLevel(multiplayerScene);

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == multiplayerScene && !playerInArena)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        playerInArena = true;
        string prefabName = myNumberInRoom == 1 ? "Player1" : "Player2";
        string characterType = GameController.instance.jobsList.Find(job => job.id == GameController.instance.selectedCharacter.job).name;
        string prefabToInstantiateName = characterType == "Guerreiro" ? "warrior" : characterType == "Mago" ? "arcane" : characterType == "Arqueiro" ? "archer" : null;

        Vector3 spawnPoint = myNumberInRoom == 1 ? MatchController.instance.spawnPoints[0] : MatchController.instance.spawnPoints[1];

        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabToInstantiateName), spawnPoint, Quaternion.identity, 0);
        player.GetComponent<Warrior>().playerNumber = myNumberInRoom;
        player.name = prefabName;
        player.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = GameController.instance.selectedCharacter.name;
    }
}