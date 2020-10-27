using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycodeTest : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        List<KeyCode> keyCodes = Utility.GetPressedKeycodes();
        if (keyCodes.Count == 0)
            return;

        string message = "";
        foreach (KeyCode key in keyCodes)
            message += key.ToString() + " ";
        Debug.Log(message);
    }
}
