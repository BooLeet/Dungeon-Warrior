using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera cam;
    public Camera UICam;
    public float defaultFov = 75;
    private float targetFov;

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
    }


    public void UpdateCamera()
    {
        ShakeUpdate();
        RecoilEffectUpdate();
        ApplyRotation();

        FOVUpdate();
        //Follow();
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

    private void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(0, 0, currentShakeAngle);
    }

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
}
