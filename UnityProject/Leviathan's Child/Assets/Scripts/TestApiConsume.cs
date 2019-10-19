using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;

class user
{
    public string id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string nick { get; set; }
    public string email { get; set; }
}

public class TestApiConsume : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetTest());
    }

    IEnumerator GetTest()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        UnityWebRequest teste = UnityWebRequest.Get("http://localhost:5000/api/user");
        yield return teste.SendWebRequest();
        JSONObject json = new JSONObject(teste.downloadHandler.text);
        var t = json.list;
        foreach (JSONObject j in t)
        {
            Debug.Log(j.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
