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

        new KeyValue("menuSettingsGraphicsBloom", "Bloom"),
        new KeyValue("menuSettingsGraphicsGrain", "Film Grain"),
        new KeyValue("menuSettingsGraphicsAO", "Ambient Occlusion"),
        new KeyValue("menuSettingsGraphicsMotionBlur", "Motion Blur"),
        new KeyValue("menuSettingsGraphicsShadows", "Shadow Quality"),
        new KeyValue("menuSettingsGraphicsResolution", "Render Resolution"),
        new KeyValue("menuSettingsGraphicsFPS", "Framerate Limit"),
        new KeyValue("menuSettingsGraphicsShowFPS", "Show FPS"),

        new KeyValue("menuSettingsGameSensitivity", "Mouse Sensitivity"),
        new KeyValue("menuSettingsGameReticule", "Show Reticule"),
        new KeyValue("menuSettingsKeyChange", "Enter the desired key:"),

        new KeyValue("menuQuitMessage", "Do you really want to quit?"),

        new KeyValue("menuResume", "Resume"),
        new KeyValue("menuRetry", "Retry"),
        new KeyValue("menuQuitToMain", "Main Menu"),

        new KeyValue("Yes", "Yes"),
        new KeyValue("No", "No"),

        new KeyValue("confirm", "Confirm"),
        new KeyValue("cancel", "Cancel"),
        new KeyValue("back", "Back"),

        new KeyValue("dead", "DEAD!"),

        new KeyValue("doorOpen", "Open"),
        new KeyValue("doorClose", "Close"),
        new KeyValue("doorKeyRequired", "Key Required"),
        new KeyValue("doorLocked", "Locked"),
        new KeyValue("doorUnlock", "Unlock"),

        new KeyValue("lootPickUp", "Pick Up"),
        new KeyValue("lootAlreadyHave", "You already have this item"),
        new KeyValue("healthPotionUse", "Quaff"),

        new KeyValue("recordScore", "Record Score"),
        new KeyValue("currentScore", "Current Score"),

        new KeyValue("keyForward", "Move Forward"),
        new KeyValue("keyBackwards", "Move Backwards"),
        new KeyValue("keyStrafeRight", "Move Right"),
        new KeyValue("keyStrafeLeft", "Move Left"),

        new KeyValue("keyPrimaryAttack", "Primary Attack"),
        new KeyValue("keySecondaryAttack", "Secondary Attack"),
        new KeyValue("keyInteract", "Interact"),
        new KeyValue("keyJump", "Jump"),
        new KeyValue("keyDash", "Dash"),
        new KeyValue("keyForcePush", "Force Push"),
        new KeyValue("keyInspect", "Inspect Weapons"),
        new KeyValue("keyBack", "Back"),
        new KeyValue("keyHideUI", "Hide HUD"),
        new KeyValue("keyLMB", "LMB"),
        new KeyValue("keyRMB", "RMB"),
        new KeyValue("keyMMB", "MMB"),

        new KeyValue("pts", "PTS"),
        new KeyValue("buy", "Buy"),
        new KeyValue("tooExpensive", "Too Expensive"),

        new KeyValue("tutorial", "Tutorial"),
        new KeyValue("stingAttack", "Sting Attack"),
        new KeyValue("spinAttack", "Spin Attack"),
        new KeyValue("revolverAttack", "Revolver Attack"),
        new KeyValue("move", "Move"),
        
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
