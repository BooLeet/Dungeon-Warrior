using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera cam;
    public float defaultFov = 75;
    private float targetFov;

    [Header("Position")]
    public Transform transformToFollow;
    [Space]
    public float angleU = 0;
    public float angleV = 60;
    private float angleURad;
    private float angleVRad;

    public float distance = 20;
    public float distanceEpsilon = 0.1f;
    public bool enableLerping = false;
    public float followLerpParameter = 0;
    public Vector3 pivotOffset;
    private Vector3 pivot;
    private Vector3 smoothVelocity;

    [Range(0,90)]
    public float maxAngle = 60;
    public Vector3 CameraForward { get; private set; }
    public Vector3 CameraRight { get; private set; }

    [Header("Shake")]
    public float shakeDistance = 10;
    public float shakeAngleSmoothing = 100;
    public float shakeIntensity = 1;
    public float shakeSpeed = 1;

    private float shakeTime = 0;
    private float shakeParameter = 0;
    private float currentShakeAngle = 0;
    private float desiredShakeAngle = 0;
    private Vector3 shakeVector;

    [Header("Recoil Effect")]
    public float recoilEffectDuration = 0.2f;
    public float recoilEffectDefaultFovIncrement = 0.5f;
    private float recoilEffectFovIncrement;
    private float currentRecoilEffectFovIncrement;
    private float recoilEffectTime = 0;

    [Header("Bobbing")]
    public bool cameraBobbing = false;
    public float cameraBobbingMagnitude = 1;
    private float cameraBobbingZ = 1f, cameraBobbingX = 2f;
    private float cameraBobbingZLerp, cameraBobbingXLerp;
    private float cameraBobbingTimeCounter = 0;
    private Vector3 cameraBobbingPreviousPosition;

    [Header("Zoom")]
    private bool isZoomedIn = false;
    public float zoomFovMultiplier = 0.7f;
    public float zoomLerpParameter = 10;

    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        instance = this;
    }

    void Start()
    {
        targetFov = defaultFov;
        transform.parent = null;
        pivot = transform.position;
    }


    public void UpdateCamera()
    {
        ShakeUpdate();
        RecoilEffectUpdate();
        CameraBobbing();

        AngleAndDistanceCorrection();
        ApplyRotation();

        FOVUpdate();
        Follow();
    }

    public void FOVUpdate()
    {
        targetFov = Mathf.Lerp(targetFov, defaultFov * (isZoomedIn ? zoomFovMultiplier : 1), Time.deltaTime * zoomLerpParameter);
        cam.fieldOfView = targetFov + currentRecoilEffectFovIncrement;
    }

    public void SetZoom(bool val)
    {
        isZoomedIn = val;
    }

    #region Shake
    private void ShakeUpdate()
    {
        if (shakeTime <= 0)
        {
            shakeTime = 0;
            shakeParameter = 0;
            desiredShakeAngle = 0;
        }
        else
        {
            desiredShakeAngle = shakeIntensity * Mathf.Sin(shakeParameter * shakeSpeed);
            shakeParameter += Time.deltaTime;
            shakeTime -= Time.deltaTime;
        }
        currentShakeAngle = Mathf.Lerp(currentShakeAngle, desiredShakeAngle, Time.deltaTime * shakeAngleSmoothing);
    }

    public static void ScreenShake(float time, Vector3 position)
    {
        if (!instance || Vector3.Distance(instance.transform.position, position) > instance.shakeDistance)
            return;
        instance.Shake(time);
    }

    public static void ScreenShake(float time)
    {
        if (!instance)
            return;
        instance.Shake(time);
    }

    private void Shake(float time)
    {
        if (shakeTime > time)
            return;

        shakeTime += time - shakeTime;
    }

    #endregion

    #region Recoil
    private void RecoilEffectUpdate()
    {
        if (recoilEffectTime <= 0)
        {
            recoilEffectTime = 0;
        }
        currentRecoilEffectFovIncrement = Mathf.Lerp(0, recoilEffectFovIncrement, recoilEffectTime / recoilEffectDuration);

        recoilEffectTime -= Time.deltaTime;
    }

    private void RecoilEffect(float fovIncrement)
    {
        recoilEffectFovIncrement = fovIncrement;
        recoilEffectTime = recoilEffectDuration;
    }

    public static void Recoil()
    {
        if (instance)
            instance.RecoilEffect(instance.recoilEffectDefaultFovIncrement);
    }

    public static void Recoil(float fovIncrement)
    {
        if (instance)
            instance.RecoilEffect(fovIncrement);
    }
    #endregion

    #region Position and Rotation
    public void Follow()
    {
        if (transformToFollow)
        {
            Vector3 localPosition = new Vector3(Mathf.Cos(angleURad) * Mathf.Cos(angleVRad),
                                                  Mathf.Sin(angleVRad),
                                                  Mathf.Sin(angleURad) * Mathf.Cos(angleVRad));

            Vector3 offset = pivotOffset.x * CameraRight + pivotOffset.y * Vector3.up + pivotOffset.z * CameraForward;
            //pivot = transformToFollow.position;
            pivot = Vector3.Lerp(pivot, transformToFollow.position,enableLerping? Time.deltaTime * followLerpParameter : 1);

            float currentDistance = distance;
            Ray ray = new Ray(pivot + offset, localPosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray,out rayHit,distance))
                currentDistance = rayHit.distance - distanceEpsilon;

            transform.position = pivot + offset + currentDistance * localPosition;
        }
    }

    private void AngleAndDistanceCorrection(float minDistance = 0f, float maxDistance = 100f)
    {
        angleV = Mathf.Clamp(angleV, -maxAngle, maxAngle);
        angleU %= 360;
        if (distance < minDistance)
            distance = minDistance;
        else if (distance > maxDistance)
            distance = maxDistance;
    }

    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(angleV + cameraBobbingMagnitude * cameraBobbingXLerp, -angleU, cameraBobbingMagnitude * cameraBobbingZLerp + currentShakeAngle);
        angleURad = (angleU + 90) * Mathf.Deg2Rad;
        angleVRad = (180 - angleV - currentShakeAngle) * Mathf.Deg2Rad;

        CameraForward = new Vector3(Mathf.Cos(angleURad), 0, Mathf.Sin(angleURad));
        CameraRight = Quaternion.AngleAxis(90, Vector3.up) * CameraForward;
    }

    #endregion

    public bool IsVisible(Vector3 position)
    {
        Vector3 positionOnScreen = cam.WorldToViewportPoint(position);
        return positionOnScreen.x < 1 && positionOnScreen.x > 0
                && positionOnScreen.y < 1 && positionOnScreen.y > 0;
    }

    public float DistanceFromTheCenter(Vector3 position)
    {
        return (cam.WorldToViewportPoint(position) - new Vector3(0.5f, 0.5f, 0)).magnitude;
    }

    private void CameraBobbing()
    {
        if (!cameraBobbing)
        {
            cameraBobbingZLerp = 0;
            cameraBobbingXLerp = 0;
            return;
        }
        float angleZ = 0;
        float angleX = 0;

        float bobbingParameter = Vector3.Distance(transform.position, cameraBobbingPreviousPosition) / Time.deltaTime;
        bobbingParameter = Mathf.Clamp(bobbingParameter, 0, 5) / 5f;

        if (Vector3.Distance(transform.position, cameraBobbingPreviousPosition) > 0)
        {
            angleZ = cameraBobbingZ;
            angleX = cameraBobbingX;
        }

        cameraBobbingZLerp = bobbingParameter * Mathf.Lerp(cameraBobbingZLerp, angleZ, Time.deltaTime * 0.5f);
        cameraBobbingXLerp = bobbingParameter * Mathf.Lerp(cameraBobbingXLerp, angleX, Time.deltaTime * 0.5f);

        if (cameraBobbingTimeCounter >= 0.4f)
        {
            cameraBobbingZ = -cameraBobbingZ;
            cameraBobbingX = -cameraBobbingX;
            cameraBobbingTimeCounter = 0;
        }

        cameraBobbingTimeCounter += Time.deltaTime * bobbingParameter;
        cameraBobbingPreviousPosition = transform.position;
    }
}
