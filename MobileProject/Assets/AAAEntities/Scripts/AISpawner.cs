using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour {
    public GameObject prefab;
    public float spawnDistance = 20;
    public float spawnRadius = 0;
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
            float randomAngle = Random.Range(0, Mathf.PI * 2);
            Vector3 offset = Random.Range(0, Mathf.Abs(spawnRadius)) * new Vector3(Mathf.Sin(randomAngle), 0, Mathf.Cos(randomAngle));
            Instantiate(prefab, transform.position + offset, transform.rotation);
            Destroy(gameObject);
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
