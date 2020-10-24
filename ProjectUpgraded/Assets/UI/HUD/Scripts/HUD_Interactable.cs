using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Interactable : MonoBehaviour {
    public PlayerCharacter player;
    public Camera cam;
    public Canvas canvas;
    public RectTransform rectTransform;
    [Space]
    public GameObject[] gameObjects;
    public Text[] texts;
    public RawImage image;
    public Texture interactButton, pullButton;

	void LateUpdate () {
        if(!player.CurrentInteractable)
        {
            ShowHide(false);
            return;
        }

        SetText(player.CurrentInteractable.GetPrompt(player));
        ShowHide(true);
        Vector3 targetPosition = cam.WorldToScreenPoint(player.CurrentInteractable.ButtonPosition) - new Vector3(cam.pixelWidth, cam.pixelHeight, 0) / 2;
        rectTransform.localPosition = targetPosition / canvas.scaleFactor;
        
    }

    private void SetText(string val)
    {
        foreach (Text text in texts)
            text.text = val;
    }

    private void ShowHide(bool show)
    {
        foreach (GameObject obj in gameObjects)
            obj.SetActive(show);
    }
}
