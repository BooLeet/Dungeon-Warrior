using UnityEngine;
using UnityEngine.UI;

public class HUD_Magica : MonoBehaviour {
    public PlayerCharacter character;
    public Image fill;
    public Text text;

	void Update () {
        if(character)
        {
            fill.fillAmount = character.CurrentMagica / character.maxMagica;
            if(text)
                text.text = ((int)character.CurrentMagica).ToString();
        }
	}
}
