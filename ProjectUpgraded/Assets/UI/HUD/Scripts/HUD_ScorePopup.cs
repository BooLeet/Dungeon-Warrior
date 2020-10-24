using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_ScorePopup : MonoBehaviour
{
    public Text[] texts;
    [Space]
    public float bigScale = 1;
    public float midScale = 0.75f;
    public float smallScale = 0.5f;

    public enum Scale { Big,Mid,Small}
    [Space]
    public float popupTime = 0.75f;
    public float fadeTime = 0.75f;
    public float verticalAscendSpeed = 0.5f;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void ShowPopup(int score, Vector3 position, Scale scale)
    {
        StartCoroutine(MainCoroutine(score, position, scale));
    }

    private IEnumerator MainCoroutine(int score, Vector3 position, Scale scale)
    {
        foreach (Text text in texts)
            text.text = score.ToString();

        transform.localScale = Vector3.zero;

        float timeCounter = 0;
        float fullScale = smallScale;
        switch (scale)
        {
            case Scale.Big:
                fullScale = bigScale; break;
            case Scale.Mid:
                fullScale = midScale; break;
        }

        float allTime = fadeTime + popupTime;
        Vector3 targetPosition = position;
        while (timeCounter < allTime)
        {
            PlayerCamera playerCamera = PlayerCamera.instance;
            transform.position = playerCamera.UICam.WorldToScreenPoint(targetPosition);
            targetPosition += Vector3.up * Time.deltaTime * verticalAscendSpeed;

            bool show = Utility.WithinAngle(playerCamera.transform.position, playerCamera.transform.forward, position, 180);

            foreach (Text text in texts)
                text.enabled = show;

            float newScale = fullScale * Mathf.Clamp(Mathf.Min(timeCounter / popupTime, (allTime - timeCounter) / (allTime - popupTime)), 0, 1);
            transform.localScale = Vector3.one * newScale;
            timeCounter += Time.deltaTime;

            
            yield return null;
        }

        Destroy(gameObject);
    }
}
