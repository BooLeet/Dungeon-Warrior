using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeTreeNode : LandscapeNodeFunction
{
    public GameObject[] treePrefabs;
    public byte treesToSpawn = 1;

    public override void Function(float nodeSize)
    {
        if (treePrefabs.Length == 0)
            return;

        for (byte i = 0; i < treesToSpawn; ++i)
        {
            Vector3 position = transform.position + 
                                transform.forward * Random.value * nodeSize / 2 + 
                                transform.right * Random.value * nodeSize / 2;
            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            if(prefab)
                Instantiate(prefab, position, Quaternion.Euler(0, 360 * Random.value, 0)).transform.parent = transform;
        }
    }
}
