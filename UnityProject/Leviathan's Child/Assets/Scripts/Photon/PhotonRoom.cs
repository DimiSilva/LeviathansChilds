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

    public int currentScene;
    public int multiplayerScene;

    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    private bool playerInArena = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
                return;
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
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = GameController.instance.selectedCharacter.name;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            StartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 2)
            StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(CheckIfSomeoneHasLeftTheRoom());
        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    IEnumerator CheckIfSomeoneHasLeftTheRoom()
    {
        yield return new WaitForSeconds(4);
        if (!PhotonNetwork.InRoom) yield break;

        if (PhotonNetwork.CurrentRoom.PlayerCount != 2 && currentScene == multiplayerScene)
        {
            QuitRooom();
            yield break;
        }

        StartCoroutine(CheckIfSomeoneHasLeftTheRoom());
        yield break;
    }

    public void QuitRooom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
        playerInArena = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (currentScene == multiplayerScene)
        {
            InitialSceneUIController.instance.loginScreen.SetActive(false);
            InitialSceneUIController.instance.lobbyScreen.SetActive(true);
        }
        currentScene = scene.buildIndex;
        if (currentScene == multiplayerScene && !playerInArena)
            CreatePlayer();
    }

    private void CreatePlayer()
    {
        string characterType = GameController.instance.selectedCharacter.job.name;
        string prefabToInstantiateTypeName = characterType == "Guerreiro" ? "Warrior" : characterType == "Mago" ? "Arcane" : characterType == "Arqueiro" ? "Archer" : null;
        string prefabToInstantiateName = prefabToInstantiateTypeName + myNumberInRoom.ToString();

        Vector3 spawnPoint = myNumberInRoom == 1 ? MatchController.instance.spawnPoints[0] : MatchController.instance.spawnPoints[1];

        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Characters", prefabToInstantiateTypeName, prefabToInstantiateName), spawnPoint, Quaternion.identity, 0);

        playerInArena = true;
    }
}