using UnityEngine;
using UnityEngine.UI;

public class HUD_Health : MonoBehaviour {
    public Character character;
    public Image fill;
    public Text text;

	void Update () {
        if(character)
        {
            fill.fillAmount = character.CurrentHealth / character.maxHealth;
            text.text = ((int)character.CurrentHealth).ToString();
        }
	}
}
