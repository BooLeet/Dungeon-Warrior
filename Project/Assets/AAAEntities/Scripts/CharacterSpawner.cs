using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour {
    public GameObject prefab;

	void Start () {
        Instantiate(prefab, transform.position, transform.rotation);
        Destroy(gameObject);
	}
}
