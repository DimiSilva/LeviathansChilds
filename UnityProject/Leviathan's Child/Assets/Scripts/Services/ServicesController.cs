using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServicesController : MonoBehaviour
{
    public static ServicesController instance = null;
    public static string apiUrl = "https://leviathanschilds.azurewebsites.net";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
