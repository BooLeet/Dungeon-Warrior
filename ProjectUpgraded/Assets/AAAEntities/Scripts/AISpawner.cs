using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour {
    public GameObject prefab;
    public float spawnDistance = 20;
    private PlayerCharacter player;


    private void Update()
    {
        if (!player)
        {
            player = PlayerCharacter.instance;
            if (!player)
                return;
        }

        if(Vector3.Distance(transform.position,player.Position) <= spawnDistance)
        {
            Instantiate(prefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }

    }
}
