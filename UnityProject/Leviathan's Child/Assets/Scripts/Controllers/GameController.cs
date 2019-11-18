using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;
    private User logedUser;
    public List<Job> jobsList { get; private set; }
    public List<Element> elementsList { get; private set; }
    public List<Amulet> amuletsList { get; private set; }
    public List<Character> userCharacters { get; private set; }

    public Character selectedCharacter { get; private set; }

    public string[] indisponibleCharacters = new string[] { "Mago", "Arqueiro" };

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SetLogedUser(User user) =>
        instance.logedUser = user;

    public User GetLogedUser() =>
        instance.logedUser;

    public void SetUserCharacters(List<Character> characters) =>
        instance.userCharacters = characters;

    public void SetJobsList(List<Job> jobs) =>
        instance.jobsList = jobs;

    public void SetElementsList(List<Element> elements) =>
        instance.elementsList = elements;

    public void SetAmuletsList(List<Amulet> amulets) =>
        instance.amuletsList = amulets;

    public void SetSelectedCharacter(Character character) =>
        instance.selectedCharacter = character;
}
