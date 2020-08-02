using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Game/LanguagePack")]
public class LanguagePack : ScriptableObject {
    [System.Serializable]
    public struct KeyValue
    {
        public string key;
        public string value;
        public KeyValue(string key,string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    private KeyValue[] keyValues = {
        new KeyValue("menuPlay", "Play"),
        new KeyValue("menuSettings", "Settings"),
        new KeyValue("menuQuit", "Quit"),

        new KeyValue("menuPlay", "Campaign"),
        new KeyValue("menuPlay", "Survival"),

        new KeyValue("menuSettingsGraphics", "Graphics"),
        new KeyValue("menuSettingsAudio", "Audio"),
        new KeyValue("menuSettingsGameplay", "Gameplay"),
        new KeyValue("menuSettingsKeys", "Key Bindings"),

        new KeyValue("menuSettingsAudioSFX", "SFX Volume"),
        new KeyValue("menuSettingsAudioMusic", "Music Volume"),

        new KeyValue("menuSettingsGraphicsMotionBlur", "Motion Blur"),
        new KeyValue("menuSettingsGraphicsGrain", "Film Grain"),
        new KeyValue("menuSettingsGraphicsAO", "Ambient Occlusion"),
        new KeyValue("menuSettingsGraphicsShadows", "Shadow Quality"),

        new KeyValue("menuSettingsGameSensitivity", "Mouse Sensitivity"),
        new KeyValue("menuSettingsGameReticule", "Show Reticule"),

        new KeyValue("menuQuitMessage", "Do you really want to quit?"),

        new KeyValue("menuResume", "Resume"),
        new KeyValue("menuQuitToMain", "Main Menu"),

        new KeyValue("Yes", "Yes"),
        new KeyValue("No", "No"),

        new KeyValue("confirm", "Confirm"),
        new KeyValue("cancel", "Cancel"),
        new KeyValue("back", "Back"),

        new KeyValue("doorOpen", "Open"),
        new KeyValue("doorClose", "Close"),
        new KeyValue("doorKeyRequired", "Key Required"),
        new KeyValue("doorLocked", "Locked"),
        new KeyValue("doorUnlock", "Unlock"),

        new KeyValue("lootPickUp", "Pick Up"),
        new KeyValue("lootAlreadyHave", "You already have this item"),
    };
    //public System.Tuple<string, string>[] keyValues = {new System.Tuple<string, string>("doorOpen", "Open") };
    //[Header("Interactions")]
    //public string doorOpen = "Open";
    //public string doorClose = "Close";
    //public string doorKeyRequired = "Key Required";
    //public string doorLocked = "Locked";
    //public string doorUnlock = "Unlock";
    //[Space]
    //public string lootPickUp = "Pick Up";
    //public string lootAlreadyHave = "You already have this item";

    public string GetString(string key)
    {
        var queryResults = from keyVal in keyValues
                           where keyVal.key == key
                           select keyVal.value;
        if (queryResults.Count() > 0)
            return queryResults.First();
        return key;
    }
}
