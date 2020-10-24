using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_ResourceIndicator : MonoBehaviour
{
    public PlayerCharacter player;
    public LootScriptable lootScriptable;
    public RawImage[] images;
    public Text[] texts;

    private void Start()
    {
        foreach(RawImage image in images)
            image.texture = lootScriptable.icon;
    }

    void Update()
    {
        int lootCount = player.inventory.GetResourceCount(lootScriptable);
        foreach (Text text in texts)
            text.text = lootCount.ToString();
    }
}
