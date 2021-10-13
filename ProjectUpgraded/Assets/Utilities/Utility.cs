using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class Utility{
    public delegate void VoidFunction();
    /// <summary>
    /// Checks if the object position withing an angle
    /// </summary>
    /// <param name="pivotPosition"></param>
    /// <param name="pivotDirection"></param>
    /// <param name="objectPosition"></param>
    /// <param name="angle"> angle in degrees</param>
    /// <returns></returns>
    public static bool WithinAngle(Vector3 pivotPosition,Vector3 pivotDirection,Vector3 objectPosition,float angle)
    {
        angle /= 2;
        angle = Mathf.Abs(angle % 180);
        angle *= Mathf.Deg2Rad;
        pivotDirection.Normalize();

        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        float directionalDistance = Vector2.Distance(direction, new Vector2(1, 0));
        return Vector3.Distance(pivotDirection, (objectPosition - pivotPosition).normalized) < directionalDistance;
    }

    public static bool IsVisible(Vector3 from, GameObject obj, float maxDistance, float verticalOffset, int layerMask = int.MaxValue)
    {
        Ray ray = new Ray(from, obj.transform.position + Vector3.up * verticalOffset - from);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, maxDistance,layerMask))
        {
            if (obj == hit.collider.gameObject)
                return true;
        }

        return false;
    }

    public static void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void DisbleCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void PlayAudioClipAtPoint(AudioClip clip, Vector3 position, Transform parent,float spatialBlend,AudioMixerGroup mixerGroup)
    {
        GameObject obj = new GameObject("Audio Clip");
        obj.transform.position = position;
        obj.transform.parent = parent;
        AudioSource source = obj.AddComponent<AudioSource>();
        source.spatialBlend = spatialBlend;
        source.clip = clip;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();
        obj.AddComponent<DestroyOnTime>().delay = clip.length;
    }

    public static float AngleBetweenTwoVectors(Vector3 lhs,Vector3 rhs)
    {
        float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(lhs, rhs) / (lhs.magnitude * rhs.magnitude), -1, 1));
        return angle;
    }

    public static List<KeyCode> GetPressedKeycodes()
    {
        List<KeyCode> result = new List<KeyCode>();
        foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            if (Input.GetKeyDown(key))
                result.Add(key);
        return result;
    }
}
