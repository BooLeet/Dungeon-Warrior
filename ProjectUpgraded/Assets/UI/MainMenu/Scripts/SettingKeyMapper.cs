using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingKeyMapper : MonoBehaviour
{
    public SettingsMenuController settingsMenuController;

    public GameObject keyButtonPrefab;
    public Transform contentTransform;
    private List<SettingKeyButton> buttons;

    public SettingKeyButton KeyButtonToChange { get; private set; }

    public void FillKeyButtons()
    {
        int childCount = contentTransform.childCount;
        for (int i = 0; i < childCount; ++i)
            DestroyImmediate(contentTransform.GetChild(0).gameObject);

        List<Settings.KeyMapSettings.Key> keys = GameManager.instance.settings.keyMapSettings.keys;
        buttons = new List<SettingKeyButton>();

        foreach (Settings.KeyMapSettings.Key key in keys)
        {
            SettingKeyButton button = Instantiate(keyButtonPrefab, contentTransform).GetComponent<SettingKeyButton>();
            button.SetValues(key, this);
            
            buttons.Add(button);
        }
    }

    public void ShowKeyChangeWindow(SettingKeyButton keyButtonToChange)
    {
        KeyButtonToChange = keyButtonToChange;
        settingsMenuController.mainMenuController.ShowKeyChangeWindow();
    }

    public void ApplyNewKey(KeyCode keyCode)
    {
        settingsMenuController.GoBack();
        if (KeyButtonToChange && keyCode != KeyCode.None && keyCode != GameManager.instance.settings.keyMapSettings.GetKeyCode("back"))
        {
            KeyButtonToChange.SetNewKeyCode(keyCode);
            KeyButtonToChange = null;
        }
    }

    public List<Settings.KeyMapSettings.Key> GetMappedKeys()
    {
        List<Settings.KeyMapSettings.Key> result = new List<Settings.KeyMapSettings.Key>();

        foreach (SettingKeyButton button in buttons)
            result.Add(button.Key);

        return result;
    }

    void OnEnable()
    {
        if (!KeyButtonToChange) 
            FillKeyButtons();
    }
}
