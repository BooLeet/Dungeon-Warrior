using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_LowHealth : MonoBehaviour
{
    public PlayerCharacter player;
    public RawImage image;
    [Range(0,1)]
    public float healthPercentThreshold = 0.2f;
    public float minFlashingSpeed = 10;
    public float maxFlashingSpeed = 30;
    private Color targetColor;
    private float timeCounter = 0;

    void Start()
    {
        targetColor = image.color;
        targetColor.a = 0;
        image.color = targetColor;
    }

    void Update()
    {
        float currentHealthPercent = player.CurrentHealth / player.GetMaxHealth();
        float currentFlashingSpeed = Mathf.Lerp(maxFlashingSpeed, minFlashingSpeed, currentHealthPercent / healthPercentThreshold);

        timeCounter += Time.deltaTime * currentFlashingSpeed;
        timeCounter %= Mathf.PI;

        if (currentHealthPercent <= healthPercentThreshold)
            targetColor.a = Mathf.Sin(timeCounter);
        else
            targetColor.a = 0;

        image.color = targetColor;
    }
}
