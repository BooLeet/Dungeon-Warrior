using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LanguagePack")]
public class LanguagePack : ScriptableObject {
    [Header("Interactions")]
    public string doorOpen = "Open";
    public string doorClose = "Close";
    public string doorKeyRequired = "Key Required";
    public string doorLocked = "Locked";
    public string doorUnlock = "Unlock";
    [Space]
    public string lootPickUp = "Pick Up";
    public string lootAlreadyHave = "You already have this item";
}
