using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaUIController : MonoBehaviour
{
    public static ArenaUIController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else
        {
            if (instance != this)
            {
                Destroy(instance.gameObject);
                instance = this;
            }
        }
    }
}
