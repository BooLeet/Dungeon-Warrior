using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingKeyButton : MonoBehaviour
{
    public Text keyNameText, keyCodeText;
    public string KeyName { get; private set; }

    public Settings.KeyMapSettings.Key Key { get; private set; }

    private SettingKeyMapper keyMapper;

    public void SetNewKeyCode(KeyCode keyCode)
    {
        Key = new Settings.KeyMapSettings.Key(Key.name, Key.localizationKey, keyCode);
        keyCodeText.text = keyCode.ToString();
    }

    public void SetValues(Settings.KeyMapSettings.Key key, SettingKeyMapper keyMapper)
    {
        Key = key;
        KeyName = key.name;
        keyNameText.text = GameManager.instance.languagePack.GetString(key.localizationKey);
        keyCodeText.text = key.keyCode.ToString();
        this.keyMapper = keyMapper;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        keyMapper.ShowKeyChangeWindow(this);
    }
}
