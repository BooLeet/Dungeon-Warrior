using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {
    public string defaultSceneName = "MainMenuScene";
    public SceneLoader sceneLoader;

	void Start () {
        sceneLoader.LoadScene(SceneNameContainer.instance.sceneName);
	}
}
