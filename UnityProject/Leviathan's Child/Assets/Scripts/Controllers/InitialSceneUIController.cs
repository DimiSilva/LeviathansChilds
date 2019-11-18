using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSceneUIController : MonoBehaviour
{
    public static InitialSceneUIController instance = null;

    public GameObject loginScreen;
    public GameObject characterSelectionScreen;
    public GameObject characterCreationScreen;
    public GameObject characterCreationFormScreen;
    public GameObject lobbyScreen;
    public GameObject optionsScreen;
    public GameObject waitingForPlayerScreen;

    public GameObject charactersHolder;
    public GameObject charactersCreationHolder;

    public GameObject characterFrame;
    public GameObject characterCreationFrame;

    public GameObject CharacterCreationFormJobNameText;
    public GameObject CharacterCreationFormCharacterNameInput;
    public GameObject CharacterCreationFormCharacterElementDropdown;
    public GameObject CharacterCreationFormCharacterAmuletDropdown;
    public GameObject CharacterCreationFormCreateCharacterButton;

    public GameObject enterRoomButton_Lobby;

    public Sprite warriorSprite;
    public Sprite arcaneSprite;
    public Sprite archerSprite;

    public Character selectedCharacter { get; private set; }
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {

    }
}
