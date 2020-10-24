using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderResolutionScaler : MonoBehaviour
{
    public RectTransform rectTransform;
    public Camera mainCamera;
    [Range(0.125f,1f)]
    public float defaultScale = 0.5f;
    public RawImage image;

    private RenderTexture texture;

    private void Awake()
    {
        if (GameManager.instance) 
            GameManager.instance.resolutionScaler = this;
    }

    private void Start()
    {
        image.color = Color.white;
        if (!texture)
            ApplyResolutionScale(GameManager.instance.settings.graphicSettings.GetResolutionScale());
    }

    public void ApplyResolutionScale(float scale)
    {
        if (texture)
        {
            texture.DiscardContents();
            texture.Release();
        }

        texture = new RenderTexture((int)(scale * Screen.width), (int)(scale * Screen.height), 24, RenderTextureFormat.ARGB32);
        image.texture = texture;
        texture.filterMode = FilterMode.Point;
        mainCamera.targetTexture = texture;
    }
}
