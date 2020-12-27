using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_PausePrompt : MonoBehaviour
{
    public Text text;

    void Update()
    {
        text.text = "[" + GameManager.instance.settings.keyMapSettings.GetKeyCode("back").ToString() + "]";
    }
}
