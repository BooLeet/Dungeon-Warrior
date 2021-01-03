using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverText : MonoBehaviour
{
    public Text summaryText;

    public void UpdateText()
    {
        summaryText.text = GameMode.gameModeInstance.GetGameOverText();
    }
}
