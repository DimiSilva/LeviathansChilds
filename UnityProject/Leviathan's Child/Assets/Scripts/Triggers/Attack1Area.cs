using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1Area : MonoBehaviour
{
    public Warrior parentScript;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("attack area 1 - player " + parentScript.playerNumber + collider.ToString());
        if (collider.CompareTag("Player" + (parentScript.playerNumber == 1 ? "2" : "1")))
            parentScript.GiveDamageForAttack1();
    }
}
