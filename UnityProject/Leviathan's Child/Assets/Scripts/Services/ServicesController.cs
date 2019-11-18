using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServicesController : MonoBehaviour
{
    public static ServicesController instance = null;
    // public static string apiUrl = "http://192.168.1.18:80";
    // public static string apiUrl = "http://192.168.15.86:80";
    public static string apiUrl = "http://10.123.65.217:80";
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
