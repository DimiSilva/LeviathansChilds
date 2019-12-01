using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack3Area : MonoBehaviour
{
    public Warrior parentScript;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("attack area 3 - player " + parentScript.playerNumber + collider.ToString());
        if (collider.CompareTag("Player" + (parentScript.playerNumber == 1 ? "2" : "1")))
            parentScript.GiveDamageForAttack3();
    }
}
