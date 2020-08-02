using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_SpecialAttackMeter : MonoBehaviour {
    public GameObject[] images;
    public Image fillImage;
    public PlayerCharacter player;
	void Start () {
        foreach (GameObject obj in images)
            obj.SetActive(false);
	}
	

	void Update () {
        foreach (GameObject obj in images)
            obj.SetActive(player.SpecialAttackMeter > 0);
        fillImage.fillAmount = player.SpecialAttackMeter;
    }
}
