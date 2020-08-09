using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;


public class GameManager : MonoBehaviour {
    public static GameManager instance;



    public LanguagePack languagePack;
    [HideInInspector]public Settings settings;
    public PostProcessVolume[] postProcess;
    public AudioMixer sfxMixer;
    public AudioMixer musicMixer;
    public bool IsPaused { get; set; }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

	void Start () {
        settings = SettingsLoader.LoadSettings();
        settings.ApplyAllSettings();
    }

    public void PauseGame()
    {
        Utility.EnableCursor();
        IsPaused = true;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        IsPaused = false;
        Time.timeScale = 1;
    }

    public void ResumeGame()
    {
        Utility.DisbleCursor();
        UnpauseGame();
    }

    public AudioMixerGroup GetSfxMixerGroup()
    {
        return sfxMixer.FindMatchingGroups("Master")[0];
    }

    public AudioMixerGroup GetMusicMixerGroup()
    {
        return musicMixer.FindMatchingGroups("Master")[0];
    }
}
