using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolvingAnim : MonoBehaviour {
    public Transform transformToRotate;
    public float rotationTime = 0.5f;
    private float angle = 0;

    public void Revolve()
    {
        StartCoroutine(RevolvingRoutine());
    }

    private IEnumerator RevolvingRoutine()
    {
        float timeCounter = 0;
        while (timeCounter < rotationTime)
        {
            float newAngle = 90 * timeCounter / rotationTime;

            transformToRotate.localRotation = Quaternion.Euler(0, 0, angle + newAngle);
            yield return new WaitForEndOfFrame();
            timeCounter += Time.deltaTime;
        }
        angle += 90;
        transformToRotate.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
