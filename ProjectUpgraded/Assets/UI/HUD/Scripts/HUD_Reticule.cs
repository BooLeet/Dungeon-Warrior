using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Reticule : MonoBehaviour {
    public PlayerCharacter player;
    public Canvas canvas;
    public RectTransform rectTransform;

	void LateUpdate () {
        if (player.AimAssistedEntity)
        {
            Vector3 targetPosition = player.playerCamera.cam.WorldToScreenPoint(player.AimAssistedEntity.Position) - new Vector3(player.playerCamera.cam.pixelWidth, player.playerCamera.cam.pixelHeight, 0) / 2;
            rectTransform.localPosition = targetPosition / canvas.scaleFactor;
        }
        else
            rectTransform.localPosition = Vector3.zero;
	}
}
