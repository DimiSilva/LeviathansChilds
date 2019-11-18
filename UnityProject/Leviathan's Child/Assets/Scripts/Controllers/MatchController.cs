using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    public static MatchController instance;

    public Vector3[] spawnPoints = new Vector3[] { new Vector3(-1.7f, 4.89f, 0), new Vector3(1.7f, 4.89f, 0) };

    void Awake()
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
