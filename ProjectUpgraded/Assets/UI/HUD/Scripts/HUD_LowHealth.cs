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
    public float flashingSpeed = 1;
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
        timeCounter += Time.deltaTime * flashingSpeed;
        timeCounter %= Mathf.PI;

        if (player.CurrentHealth / player.maxHealth <= healthPercentThreshold)
            targetColor.a = Mathf.Sin(timeCounter);
        else
            targetColor.a = 0;

        image.color = targetColor;
    }
}
