using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Hitmarker : MonoBehaviour {
    public RawImage image;
    public float lerpParameter = 10;
    public AudioSource audioSource;
    public AudioClip audioClip;
    private Color fullColor, noCollor;

	void Start () {
        fullColor = image.color;
        noCollor = new Color(fullColor.r, fullColor.g, fullColor.b, 0);
        image.color = noCollor;
	}


	void Update () {
        image.color = Color.Lerp(image.color, noCollor, Time.deltaTime * lerpParameter);
	}

    public void Hitmarker()
    {
        if (audioClip)
            audioSource.PlayOneShot(audioClip);
        image.color = fullColor;
    }
}
