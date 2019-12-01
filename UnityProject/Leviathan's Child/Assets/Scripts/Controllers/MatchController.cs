using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    public static MatchController instance;

    public Vector3[] spawnPoints = new Vector3[] { new Vector3(-1.7f, 4.89f, 0), new Vector3(1.7f, 4.89f, 0) };

    public GameObject player1;
    public string player1Job;

    public GameObject player2;
    public string player2Job;

    public int battleTimeInSeconds = 0;

    public bool matchFinished = false;

    void Awake()
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
    }

    void Start()
    {
        StartCoroutine(TimerToInactiveMainCamera());
        StartCoroutine(InitCountBattleSeconds());
    }

    IEnumerator TimerToInactiveMainCamera()
    {
        ArenaUIController.instance.loadingPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        GameObject.Find("Main Camera").SetActive(false);
        yield return new WaitForSeconds(2);
        ArenaUIController.instance.loadingPanel.SetActive(false);
    }

    IEnumerator InitCountBattleSeconds()
    {
        yield return new WaitForSeconds(3);
        while (!matchFinished)
        {
            battleTimeInSeconds++;
            yield return new WaitForSeconds(1);
        }
        yield break;
    }

    public void FinishMatch(int winnerNumber)
    {
        bool iAmWinner = winnerNumber == PhotonRoom.instance.myNumberInRoom;
        matchFinished = true;

        if (iAmWinner)
            ArenaUIController.instance.victoryPanel.SetActive(true);
        else
            ArenaUIController.instance.losePanel.SetActive(true);

        Character myCharacterInformations = GameController.instance.selectedCharacter;

        CharacterService.instance.Change(myCharacterInformations.id.ToString(), myCharacterInformations.amuletExperience + (iAmWinner ? 20 : 10), myCharacterInformations.battlesNumber + 1, myCharacterInformations.victorysNumber + (iAmWinner ? 1 : 0), myCharacterInformations.losesNumber + (iAmWinner ? 0 : 1), myCharacterInformations.battleTimeInSeconds + battleTimeInSeconds, myCharacterInformations.xp + (iAmWinner ? 50 : 20));
        StartCoroutine(TreatUpdateCharacterRequisition());
    }

    IEnumerator TreatUpdateCharacterRequisition()
    {
        if (CharacterService.instance.processing)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(TreatUpdateCharacterRequisition());
            yield break;
        }

        if (CharacterService.instance.responseText != null && CharacterService.instance.responseText.Length > 0)
        {
            CharacterService.instance.GetAllForUser(GameController.instance.GetLogedUser().id.ToString());
            StartCoroutine(TreatGetAllForUserRequisition());
        }
    }

    IEnumerator TreatGetAllForUserRequisition()
    {
        if (CharacterService.instance.processing)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(TreatGetAllForUserRequisition());
            yield break;
        }

        if (CharacterService.instance.responseText != null && CharacterService.instance.responseText.Length > 0)
        {
            List<Character> characters = new List<Character>();
            foreach (JSONObject character in CharacterService.instance.responseJson.list)
            {
                string id = character.GetField("id").ToString().Replace("\"", "");
                string name = character.GetField("name").ToString().Replace("\"", "");
                Job job = GameController.instance.jobsList.Find(jobInList => jobInList.id.ToString() == character.GetField("job").ToString().Replace("\"", ""));
                Element element = GameController.instance.elementsList.Find(elementInList => elementInList.id.ToString() == character.GetField("element").ToString().Replace("\"", ""));
                Amulet amulet = GameController.instance.amuletsList.Find(amuletInList => amuletInList.id.ToString() == character.GetField("amulet").ToString().Replace("\"", ""));
                int amuletLevel = int.Parse(character.GetField("amuletLevel").ToString().Replace("\"", ""));
                int amuletExperience = int.Parse(character.GetField("amuletExperience").ToString().Replace("\"", ""));
                float hp = float.Parse(character.GetField("hp").ToString().Replace("\"", ""));
                float strength = float.Parse(character.GetField("strength").ToString().Replace("\"", ""));
                float agility = float.Parse(character.GetField("agility").ToString().Replace("\"", ""));
                float intelligence = float.Parse(character.GetField("intelligence").ToString().Replace("\"", ""));
                int battlesNumber = int.Parse(character.GetField("battlesNumber").ToString().Replace("\"", ""));
                int victorysNumber = int.Parse(character.GetField("victorysNumber").ToString().Replace("\"", ""));
                int losesNumber = int.Parse(character.GetField("losesNumber").ToString().Replace("\"", ""));
                int battleTimeInSeconds = int.Parse(character.GetField("battleTimeInSeconds").ToString().Replace("\"", ""));
                int xp = int.Parse(character.GetField("xp").ToString().Replace("\"", ""));
                int xpToUp = int.Parse(character.GetField("xpToUp").ToString().Replace("\"", ""));
                int level = int.Parse(character.GetField("level").ToString().Replace("\"", ""));
                characters.Add(new Character(id, name, job, element, amulet, amuletLevel, amuletExperience, hp, strength, agility, intelligence, battlesNumber, victorysNumber, losesNumber, battleTimeInSeconds, xp, xpToUp, level));
            }
            GameController.instance.SetUserCharacters(characters);
            GameController.instance.SetSelectedCharacter(characters.Find(character => character.id == GameController.instance.selectedCharacter.id));
            yield return new WaitForSeconds(3);
            PhotonRoom.instance.QuitRooom();
        }
    }
}
