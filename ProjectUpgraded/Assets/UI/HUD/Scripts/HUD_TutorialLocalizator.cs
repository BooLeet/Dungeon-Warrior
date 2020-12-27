using UnityEngine;
using UnityEngine.UI;


public class HUD_TutorialLocalizator : MonoBehaviour
{
    public Text text;
    private Settings.KeyMapSettings keyMapSettings;
    private LanguagePack languagePack;

    void Update()
    {
        languagePack = GameManager.instance.languagePack;
        keyMapSettings = GameManager.instance.settings.keyMapSettings;

        string tutorialMessage = languagePack.GetString("tutorial") + "\n\n\n";
        tutorialMessage += "- " + languagePack.GetString("keyPrimaryAttack") + " - " + SquareBracketsKey("primaryAttack") + "\n\n";
        tutorialMessage += "- " + languagePack.GetString("stingAttack") + " - " + SquareBracketsKey("forward") + " + " + SquareBracketsKey("secondaryAttack") + "\n\n";
        tutorialMessage += "- " + languagePack.GetString("spinAttack") + " - " + SquareBracketsKey("strafeLeft") + "/" + SquareBracketsKey("strafeRight") + " + " + SquareBracketsKey("secondaryAttack") + "\n\n";
        tutorialMessage += "- " + languagePack.GetString("revolverAttack") + " - " + SquareBracketsKey("backwards") + " + " + SquareBracketsKey("secondaryAttack") + "\n\n";

        tutorialMessage += "- " + languagePack.GetString("keyInteract") + " - " + SquareBracketsKey("interact") + "\n\n";
        tutorialMessage += "- " + languagePack.GetString("keyJump") + " - " + SquareBracketsKey("jump") + "\n\n";
        tutorialMessage += "- " + languagePack.GetString("keyDash") + " - " + SquareBracketsKey("dash") + "\n\n";
        tutorialMessage += "- " + languagePack.GetString("keyForcePush") + " - " + SquareBracketsKey("forcePush") + "\n\n";

        text.text = tutorialMessage;
    }

    private string SquareBracketsKey(string code)
    {
        KeyCode keyCode = keyMapSettings.GetKeyCode(code);
        string key;
        if (keyCode == KeyCode.Mouse0)
            key = languagePack.GetString("keyLMB");
        else if (keyCode == KeyCode.Mouse1)
            key = languagePack.GetString("keyRMB");
        else if (keyCode == KeyCode.Mouse2)
            key = languagePack.GetString("keyMMB");
        else
            key = keyCode.ToString();
        return "[" + key + "]";
    }

}
