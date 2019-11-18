using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterCreationData
{
    public string name;
    public string job;
    public string user;
    public string element;
    public string amulet;
}

public class CharacterService : MonoBehaviour
{
    public static CharacterService instance = null;
    public string responseText { get; private set; }
    public JSONObject responseJson { get; private set; }
    public bool processing { get; private set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void GetAllForUser(string id) =>
        StartCoroutine(Get($"{ServicesController.apiUrl}/api/character/user/{id}"));

    public void Create(string characterName, Job job, User user, Element element, Amulet amulet) =>
        StartCoroutine(Post($"{ServicesController.apiUrl}/api/character", new CharacterCreationData() { name = characterName, job = job.id.ToString(), user = user.id.ToString(), element = element.id.ToString(), amulet = amulet.id.ToString() }));

    public void Remove(string id) =>
        StartCoroutine(Delete($"{ServicesController.apiUrl}/api/character/{id}"));

    IEnumerator Get(string uri)
    {
        responseJson = null;
        responseText = null;
        processing = true;
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        using (var req = new UnityWebRequest(uri, "GET"))
        {
            req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
                processing = false;
                yield break;
            }

            if (req.downloadHandler.text.Length <= 0)
            {
                processing = false;
                yield break;
            }

            this.responseText = req.downloadHandler.text;
            JSONObject json = new JSONObject(req.downloadHandler.text);
            this.responseJson = json;
            processing = false;
        }
    }

    IEnumerator Post(string uri, CharacterCreationData data)
    {
        responseJson = null;
        responseText = null;
        processing = true;
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));

        using (var req = new UnityWebRequest(uri, "POST"))
        {
            req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
                processing = false;
                yield break;
            }

            if (req.downloadHandler.text.Length <= 0)
            {
                processing = false;
                yield break;
            }

            this.responseText = req.downloadHandler.text;
            JSONObject json = new JSONObject(req.downloadHandler.text);
            this.responseJson = json;
            processing = false;
        }
    }

    IEnumerator Delete(string uri)
    {
        responseJson = null;
        responseText = null;
        processing = true;
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        using (var req = new UnityWebRequest(uri, "DELETE"))
        {
            req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
                processing = false;
                yield break;
            }

            if (req.downloadHandler.text.Length <= 0)
            {
                processing = false;
                yield break;
            }

            this.responseText = req.downloadHandler.text;
            JSONObject json = new JSONObject(req.downloadHandler.text);
            this.responseJson = json;
            processing = false;
        }
    }
}
