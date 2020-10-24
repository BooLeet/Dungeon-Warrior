using UnityEngine;
using UnityEngine.UI;

public class HUD_Mana : MonoBehaviour
{
    public PlayerCharacter player;
    public Image fill;
    public Text text;

    void Update()
    {
        if (player)
        {
            fill.fillAmount = player.CurrentMana > 0 ? player.CurrentMana / player.playerStats.maxMana : 0;
            if (text)
                text.text = ((int)player.CurrentMana).ToString();
        }
    }
}
