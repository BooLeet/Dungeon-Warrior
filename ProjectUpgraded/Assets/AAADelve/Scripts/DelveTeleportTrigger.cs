using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelveTeleportTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>())
            GameModeDelve.instance.NextRoom();
    }
}
