﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
    public PlayerCharacter player;
    public Transform hudContainerTransform;
    [Space]
    public GameObject damageIndicatorPrefab;
    public HUD_Hitmarker hitmarker,deathmarker;
    public GameObject reticule;

    private void Start()
    {
        ShowHideReticule(GameManager.instance.settings.gameSettings.showReticule);
    }

    public void DamageIndicator(Vector3 damageSource, PlayerCharacter player)
    {
        Instantiate(damageIndicatorPrefab, new Vector2(Screen.width, Screen.height) / 2, Quaternion.identity, hudContainerTransform).GetComponent<HUD_DamageIndicator>().StartEffect(damageSource, player);
    }

    public void ShowHideReticule(bool val)
    {
        reticule.SetActive(val);
    }
}