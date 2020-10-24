using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_ScoreMultiplier : MonoBehaviour
{
    public Text[] scoreTexts;
    public Text[] multiplierTexts;
    public Text[] roomTexts;

    public Image fillImage;
    private GameModeDelve gameModeDelve;

    void Start()
    {
        SetTexts(multiplierTexts, "x1");
        SetTexts(scoreTexts, "0");
        SetTexts(roomTexts, "0");
    }

    
    void Update()
    {
        if (!gameModeDelve)
        {
            gameModeDelve = GameModeDelve.instance;
            return;
        }

        fillImage.fillAmount = gameModeDelve.ScoreMultiplierMeter;
        SetTexts(multiplierTexts, "x" + gameModeDelve.ScoreMultiplier.ToString());
        SetTexts(scoreTexts, gameModeDelve.Score.ToString());
        SetTexts(roomTexts, gameModeDelve.RoomNumber.ToString());
    }

    private void SetTexts(Text[] textArray, string str)
    {
        foreach (Text text in textArray)
            text.text = str;
    }
}
