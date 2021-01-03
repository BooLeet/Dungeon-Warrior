using UnityEngine;
using UnityEngine.UI;

public class HUD_DelveRecord : MonoBehaviour
{
    public Text text;

    void Start()
    {
        text.text = GameManager.instance.languagePack.GetString("recordScore") + ":\n " + GameModeDelve.LoadScore().score.ToString();
    }

}
