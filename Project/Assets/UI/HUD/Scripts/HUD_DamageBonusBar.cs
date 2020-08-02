using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_DamageBonusBar : MonoBehaviour {
    public PlayerCharacter player;
    public Image fillImage;
    public Text[] texts;
    public float fillLerpParameter = 10;

	void Start () {
        fillImage.fillAmount = 0;
    }
	
	
	void Update () {
        float damageMultiplier = player.DamageBonusCurrentMultiplier;
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, damageMultiplier % 1, Time.deltaTime * fillLerpParameter);
        foreach(Text text in texts)
            text.text = "x" + (damageMultiplier - damageMultiplier % 1).ToString();

    }
}
