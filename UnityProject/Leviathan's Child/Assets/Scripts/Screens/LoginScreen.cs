using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    private bool development = true;

    [SerializeField]
    private InputField nickInput;
    [SerializeField]
    private InputField passwordInput;
    [SerializeField]
    private GameObject InfosText;
    [SerializeField]
    private GameObject loginButton;

    private bool errorInRequests = false;
    private bool jobsRequestOkay = false;
    private bool elementsRequestOkay = false;
    private bool amuletsRequestOkay = false;

    void Start()
    {
        StartCoroutine(GetResourcersFromApi());
    }

    private void ConfigureInfoText(string text, Color color, bool active)
    {
        InfosText.GetComponent<Text>().text = text;
        InfosText.GetComponent<Text>().color = color;
        InfosText.SetActive(active);
    }

    IEnumerator GetResourcersFromApi()
    {
        ConfigureInfoText("Carregando...", Color.white, true);
        yield return StartCoroutine(GetJobs());
        yield return StartCoroutine(GetElements());
        yield return StartCoroutine(GetAmulets());
        if (errorInRequests)
        {
            ConfigureInfoText("Algo deu errado", Color.red, true);
            yield break;
        }

        do
        {
            yield return new WaitForSeconds(0.1f);
        } while (!jobsRequestOkay || !elementsRequestOkay || !amuletsRequestOkay);

        ConfigureInfoText("", Color.white, false);
        if (development)
        {
            UserService.instance.Authentication("DimiSilva", "123456789");
            StartCoroutine(TreatAuthenticationRequisition());
        }
        else
            loginButton.GetComponent<Button>().interactable = true;
    }

    IEnumerator GetJobs()
    {
        JobService.instance.GetAll();
        yield return StartCoroutine(TreatGetJobsRequisition());
        jobsRequestOkay = true;
    }

    IEnumerator TreatGetJobsRequisition()
    {
        if (JobService.instance.processing)
            yield return new WaitForSeconds(0.5f);

        if (JobService.instance.responseText != null && JobService.instance.responseText.Length > 0)
        {
            List<Job> jobs = new List<Job>();
            foreach (JSONObject job in JobService.instance.responseJson.list)
            {
                string id = job.GetField("id").ToString().Replace("\"", "");
                string name = job.GetField("name").ToString().Replace("\"", "");
                string description = job.GetField("description").ToString().Replace("\"", "");
                int baseHp = int.Parse(job.GetField("baseHp").ToString().Replace("\"", ""));
                int baseStrength = int.Parse(job.GetField("baseStrength").ToString().Replace("\"", ""));
                int baseAgility = int.Parse(job.GetField("baseAgility").ToString().Replace("\"", ""));
                int baseIntelligence = int.Parse(job.GetField("baseIntelligence").ToString().Replace("\"", ""));
                int baseXpToUp = int.Parse(job.GetField("baseXpToUp").ToString().Replace("\"", ""));
                float xpToUpMultiplier = float.Parse(job.GetField("xpToUpMultiplier").ToString().Replace("\"", ""));
                float hpMultiplier = float.Parse(job.GetField("hpMultiplier").ToString().Replace("\"", ""));
                float strengthMultiplier = float.Parse(job.GetField("strengthMultiplier").ToString().Replace("\"", ""));
                float agilityMultiplier = float.Parse(job.GetField("agilityMultiplier").ToString().Replace("\"", ""));
                float intelligenceMultiplier = float.Parse(job.GetField("intelligenceMultiplier").ToString().Replace("\"", ""));
                int levelToBlessing = int.Parse(job.GetField("levelToBlessing").ToString().Replace("\"", ""));
                jobs.Add(new Job(id, name, description, baseHp, baseStrength, baseAgility, baseIntelligence, baseXpToUp, xpToUpMultiplier, hpMultiplier, strengthMultiplier, agilityMultiplier, intelligenceMultiplier, levelToBlessing));
            }
            GameController.instance.SetJobsList(jobs);
        }
        else
            errorInRequests = true;
    }

    IEnumerator GetElements()
    {
        ElementService.instance.GetAll();
        yield return StartCoroutine(TreatGetElementsRequisition());
        elementsRequestOkay = true;
    }

    IEnumerator TreatGetElementsRequisition()
    {
        if (ElementService.instance.processing)
            yield return new WaitForSeconds(0.5f);

        if (ElementService.instance.responseText != null && ElementService.instance.responseText.Length > 0)
        {
            List<Element> elements = new List<Element>();
            foreach (JSONObject element in ElementService.instance.responseJson.list)
            {
                string id = element.GetField("id").ToString().Replace("\"", ""); ;
                string name = element.GetField("name").ToString().Replace("\"", ""); ;
                string description = element.GetField("description").ToString().Replace("\"", ""); ;
                float hpMultiplier = float.Parse(element.GetField("hpMultiplier").ToString().Replace("\"", ""));
                float strengthMultiplier = float.Parse(element.GetField("strengthMultiplier").ToString().Replace("\"", ""));
                float agilityMultiplier = float.Parse(element.GetField("agilityMultiplier").ToString().Replace("\"", ""));
                float intelligenceMultiplier = float.Parse(element.GetField("intelligenceMultiplier").ToString().Replace("\"", ""));
                elements.Add(new Element(id, name, description, hpMultiplier, strengthMultiplier, agilityMultiplier, intelligenceMultiplier));
            }
            GameController.instance.SetElementsList(elements);
        }
        else
            errorInRequests = true;
    }

    IEnumerator GetAmulets()
    {
        AmuletService.instance.GetAll();
        yield return StartCoroutine(TreatGetAmuletsRequisition());
        amuletsRequestOkay = true;
    }

    IEnumerator TreatGetAmuletsRequisition()
    {
        if (AmuletService.instance.processing)
            yield return new WaitForSeconds(0.5f);

        if (AmuletService.instance.responseText != null && AmuletService.instance.responseText.Length > 0)
        {
            List<Amulet> amulets = new List<Amulet>();
            foreach (JSONObject amulet in AmuletService.instance.responseJson.list)
            {
                string id = amulet.GetField("id").ToString().Replace("\"", "");
                string name = amulet.GetField("name").ToString().Replace("\"", "");
                string description = amulet.GetField("description").ToString().Replace("\"", "");
                float hpMultiplier = float.Parse(amulet.GetField("hpMultiplier").ToString().Replace("\"", ""));
                float strengthMultiplier = float.Parse(amulet.GetField("strengthMultiplier").ToString().Replace("\"", ""));
                float agilityMultiplier = float.Parse(amulet.GetField("agilityMultiplier").ToString().Replace("\"", ""));
                float intelligenceMultiplier = float.Parse(amulet.GetField("intelligenceMultiplier").ToString().Replace("\"", ""));
                int baseXpToUp = int.Parse(amulet.GetField("baseXpToUp").ToString().Replace("\"", ""));
                float xpToUpMultiplier = float.Parse(amulet.GetField("xpToUpMultiplier").ToString().Replace("\"", ""));
                amulets.Add(new Amulet(id, name, description, hpMultiplier, strengthMultiplier, agilityMultiplier, intelligenceMultiplier, baseXpToUp, xpToUpMultiplier));
            }
            GameController.instance.SetAmuletsList(amulets);
        }
        else
            errorInRequests = true;
    }

    public void OnPress_LoginButton()
    {
        if (!NickInputIsValid())
        {
            SetInputBackgroundColor(nickInput, new Color(1f, 0.7f, 0.7f));
            return;
        }
        else SetInputBackgroundColor(nickInput, new Color(1f, 1f, 1f));

        if (!PasswordInputIsValid())
        {
            SetInputBackgroundColor(passwordInput, new Color(1f, 0.7f, 0.7f));
            return;
        }
        else SetInputBackgroundColor(passwordInput, new Color(1f, 1f, 1f));

        UserService.instance.Authentication(nickInput.text, passwordInput.text);
        StartCoroutine(TreatAuthenticationRequisition());
    }

    private bool NickInputIsValid() => nickInput.text.Length >= 4;

    private void SetInputBackgroundColor(InputField input, Color color) => input.image.color = color;

    private bool PasswordInputIsValid() => passwordInput.text.Length >= 8;

    IEnumerator TreatAuthenticationRequisition()
    {
        if (UserService.instance.processing)
            yield return new WaitForSeconds(0.5f);

        if (UserService.instance.responseText != null && UserService.instance.responseText.Length > 0)
        {
            ConfigureInfoText("", Color.white, false);
            GameController.instance.SetLogedUser(new User(UserService.instance.responseJson.GetField("id").ToString().Replace("\"", ""), UserService.instance.responseJson.GetField("firstName").ToString().Replace("\"", ""), UserService.instance.responseJson.GetField("emailAdress").ToString().Replace("\"", "")));
            InitialSceneUIController.instance.characterSelectionScreen.SetActive(true);
            this.gameObject.SetActive(false);
        }
        else
            ConfigureInfoText("Nick ou senha inválido", Color.red, true);
    }

    public void OnPress_CreateAccount()
    {
        Application.OpenURL("www.google.com.br");
    }
}
