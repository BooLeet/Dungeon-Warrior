using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour {
    public float delay;

	void Start () {
        Destroy(gameObject, delay);
	}

}
