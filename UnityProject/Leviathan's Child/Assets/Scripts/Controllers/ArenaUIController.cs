using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaUIController : MonoBehaviour
{
    public static ArenaUIController instance;

    public GameObject loadingPanel;
    public GameObject victoryPanel;
    public GameObject losePanel;

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
    }
}
