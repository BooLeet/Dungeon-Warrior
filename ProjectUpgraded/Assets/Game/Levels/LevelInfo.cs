using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelInfo")]
public class LevelInfo : ScriptableObject
{
    public string nameLocalizationKey;
    public string sceneName;
}
