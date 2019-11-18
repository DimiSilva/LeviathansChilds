using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AmuletService : MonoBehaviour
{
    public static AmuletService instance = null;
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

    public void GetById(string id)
    {
        StartCoroutine(Get($"{ServicesController.apiUrl}/api/amulet/{id}"));
    }

    public void GetAll()
    {
        StartCoroutine(Get($"{ServicesController.apiUrl}/api/amulet"));
    }

    IEnumerator Post(string uri, AuthenticationData data)
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
}
