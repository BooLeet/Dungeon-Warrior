using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingKeyChangeWindow : MonoBehaviour
{
    public SettingKeyMapper settingsKeyMapper;
    private KeyCode keyCode;

    void Update()
    {
        keyCode = KeyCode.None;
        List<KeyCode> pressedKeys = Utility.GetPressedKeycodes();
        if (pressedKeys.Count == 0)
            return;

        keyCode = pressedKeys[0];
        GoBack();
    }

    public void GoBack()
    {
        settingsKeyMapper.ApplyNewKey(keyCode);
    }
}
