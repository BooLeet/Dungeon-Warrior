using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_ZoomAbility : MonoBehaviour {
    public PlayerCharacter player;
    public Image fill;
    //public Text text;

    void Update()
    {
        if (player)
        {
            fill.fillAmount = player.ZoomAbilityMeter / player.zoomAbilityDuration;
            //text.text = ((int)player.ZoomAbilityMeter).ToString();
        }
    }
}
