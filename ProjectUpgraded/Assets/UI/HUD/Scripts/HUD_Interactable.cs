using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Interactable : MonoBehaviour {
    public PlayerCharacter player;
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

        float currentDistance = Vector3.Distance(player.head.position, player.CurrentInteractable.ButtonPosition);
        bool isPullable = player.CurrentInteractable as Pullable;

        if (currentDistance < player.interactionDistance && ((isPullable && (player.CurrentInteractable as Pullable).canInteractWith) || !isPullable))
        {
            image.texture = interactButton;
            SetText(player.CurrentInteractable.GetPrompt(player));
        }
        else if (isPullable && currentDistance >= player.interactionDistance)
        {
            image.texture = pullButton;
            SetText("");
        }
        else
        {
            ShowHide(false);
            return;
        }

        ShowHide(true);
        Vector3 targetPosition = player.playerCamera.cam.WorldToScreenPoint(player.CurrentInteractable.ButtonPosition) - new Vector3(player.playerCamera.cam.pixelWidth, player.playerCamera.cam.pixelHeight, 0) / 2;
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
