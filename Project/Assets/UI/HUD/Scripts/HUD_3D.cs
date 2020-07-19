using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_3D : MonoBehaviour {
    public PlayerCamera playerCamera;
    public Transform transformToFollow;
    public Vector3 offset;

	void Start () {
        GetComponent<RectTransform>().SetParent(null);
	}
	
	
	void LateUpdate () {
        Follow();
	}

    private void Follow()
    {
        Vector3 targetPosition = transformToFollow.position + offset.x * playerCamera.CameraRight + offset.y * Vector3.up + offset.z * playerCamera.CameraForward;
        Quaternion targetRotation = Quaternion.Euler(playerCamera.angleV * 2/3, -playerCamera.angleU, 0);
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}
