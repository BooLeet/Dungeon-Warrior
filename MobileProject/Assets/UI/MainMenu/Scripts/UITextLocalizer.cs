using UnityEngine;
using UnityEngine.UI;

public class UITextLocalizer : MonoBehaviour
{
    public Text[] texts;
    public string localizationKey;

    void Start()
    {
        string str = GameManager.instance.languagePack.GetString(localizationKey);
        foreach (Text text in texts)
            text.text = str;
    }
}
