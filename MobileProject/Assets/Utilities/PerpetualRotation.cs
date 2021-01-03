using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerpetualRotation : MonoBehaviour {
    public float angularSpeed = 2f;

	void Start () {

    }
	
	void Update () {
        transform.Rotate(0, Time.deltaTime * angularSpeed, 0);
	}
}
