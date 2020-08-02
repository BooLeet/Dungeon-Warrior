using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Camera : MonoBehaviour {
    public Camera mainCamera;
    private Camera thisCamera;

    void Start()
    {
        thisCamera = gameObject.GetComponent<Camera>();
    }

	void LateUpdate () {
        thisCamera.fieldOfView = mainCamera.fieldOfView;
	}
}
