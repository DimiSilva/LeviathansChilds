using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public static Launcher instance;
    [SerializeField]
    private GameObject photonRoomPrefab;

    private bool firstInitialization = true;

    private void Awake()
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

        DontDestroyOnLoad(this.gameObject);
        if (firstInitialization)
        {
            Instantiate(photonRoomPrefab, transform.position, Quaternion.identity, null);
            firstInitialization = false;
        }
    }
}
