using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Dash : MonoBehaviour {
    public PlayerCharacter player;
    public Image fillImage;

	
	void Update () {
        if(player)
            fillImage.fillAmount = player.DashMeter / player.dashCapacity;
	}
}
