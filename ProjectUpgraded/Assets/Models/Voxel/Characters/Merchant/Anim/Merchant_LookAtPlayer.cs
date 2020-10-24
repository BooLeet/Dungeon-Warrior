using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant_LookAtPlayer : MonoBehaviour
{
    public Transform headTransform;
    public float eyeOffset;

    void Update()
    {
        PlayerCamera playerCamera = PlayerCamera.instance;
        if (!playerCamera)
            return;

        if (Utility.WithinAngle(headTransform.position, transform.forward, playerCamera.transform.position, 180))
        {
            Vector3 camPosition = playerCamera.transform.position + Vector3.down * eyeOffset;
            headTransform.rotation = Quaternion.Lerp(headTransform.rotation, Quaternion.LookRotation(camPosition - headTransform.position), Time.deltaTime * 10);
        }
        else
            headTransform.localRotation = Quaternion.Lerp(headTransform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * 10);
    }
}
