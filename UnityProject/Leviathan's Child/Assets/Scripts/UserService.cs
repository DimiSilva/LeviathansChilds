using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AuthenticationData
{
    public string nick;
    public string password;
}

public class UserService : MonoBehaviour
{
    public static UserService instance = null;

    private string responseText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public User Authentication(string nick, string password)
    {
        AuthenticationData data = new AuthenticationData() { nick = nick, password = password };
        StartCoroutine(Post("http://localhost:5000/api/user/authentication", data));
        return null;
    }

    IEnumerator Post(string uri, AuthenticationData data)
    {
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
                yield break;
            }
            else
            {
                Debug.Log("Form upload complete!");
            }

            if (req.downloadHandler.text.Length <= 0)
            {
                Debug.Log("mistake");
                yield break;
            }

            JSONObject json = new JSONObject(req.downloadHandler.text);
            Debug.Log(json.ToString());
        }
    }
}
