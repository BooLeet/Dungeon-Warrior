using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
    public PlayerCharacter player;
    public Transform hudContainerTransform;
    [Space]
    public GameObject damageIndicatorPrefab;
    public HUD_Hitmarker hitmarker,deathmarker;
    public GameObject reticule;
    public GameObject scorePopupPrefab;
    public Canvas canvas;

    private void Start()
    {
        GameManager.instance.hud = this;
    }

    public void ShowHide(bool val)
    {
        canvas.enabled = val;
        //gameObject.SetActive(val);
    }

    private void Update()
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

    public void ScorePopup(int score, Vector3 position,HUD_ScorePopup.Scale scale)
    {
        Instantiate(scorePopupPrefab, transform).GetComponent<HUD_ScorePopup>().ShowPopup(score, position, scale);
    }
}
