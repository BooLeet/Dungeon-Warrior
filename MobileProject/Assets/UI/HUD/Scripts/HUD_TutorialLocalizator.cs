using UnityEngine;
using UnityEngine.UI;


public class HUD_TutorialLocalizator : MonoBehaviour
{
    public Text tutorial, primaryAttack, secondaryAttack, move, interact, dash, forcePush;
    private Settings.KeyMapSettings keyMapSettings;
    private LanguagePack languagePack;

    void Update()
    {
        languagePack = GameManager.instance.languagePack;
        keyMapSettings = GameManager.instance.settings.keyMapSettings;

        tutorial.text = languagePack.GetString("tutorial");
        primaryAttack.text = languagePack.GetString("keyPrimaryAttack");
        secondaryAttack.text = languagePack.GetString("keySecondaryAttack");

        move.text = languagePack.GetString("move");
        interact.text = languagePack.GetString("keyInteract");
        dash.text = languagePack.GetString("keyDash");
        forcePush.text = languagePack.GetString("keyForcePush");
    }

}
