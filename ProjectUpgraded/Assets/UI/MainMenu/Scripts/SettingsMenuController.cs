using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Graphics")]
    public Toggle motionBlurToggle;
    public Toggle filmGrainToggle;
    public Toggle AO_Toggle;

    [Header("Audio")]
    public Slider sfxVolumeSlider; 
    public Slider musicVolumeSlider;

    [Header("Gameplay")]
    public Slider sensitivytySlider;
    public Toggle reticuleToggle;

    void Start()
    {
        
    }

    #region Graphics
    public void LoadGraphicsValues()
    {
        Settings.GraphicSettings graphicSettings = GameManager.instance.settings.graphicSettings;
        motionBlurToggle.isOn = graphicSettings.motionBlur;
        filmGrainToggle.isOn = graphicSettings.filmGrain;
        AO_Toggle.isOn = graphicSettings.AO;
    }

    public void ApplyGraphicsSettings()
    {
        GameManager.instance.settings.graphicSettings = new Settings.GraphicSettings(motionBlurToggle.isOn, filmGrainToggle.isOn, AO_Toggle.isOn, 4);
        GameManager.instance.settings.ApplyGraphicsSettings();
        SaveSettings();
    }
    #endregion

    #region Audio
    public void LoadAudioValues()
    {
        Settings.AudioSettings audioSettings = GameManager.instance.settings.audioSettings;
        sfxVolumeSlider.value = audioSettings.sfxVolume;
        musicVolumeSlider.value = audioSettings.musicVolume;
    }

    public void ApplyAudioSettings()
    {
        GameManager.instance.settings.audioSettings = new Settings.AudioSettings(sfxVolumeSlider.value, musicVolumeSlider.value);
        GameManager.instance.settings.ApplyAudioSettings();
        SaveSettings();
    }
    #endregion

    #region Gameplay
    public void LoadGameValues()
    {
        Settings.GameSettings gameSettings = GameManager.instance.settings.gameSettings;
        sensitivytySlider.value = gameSettings.mouseSensitivity;
        reticuleToggle.isOn = gameSettings.showReticule;
    }

    public void ApplyGameSettings()
    {
        GameManager.instance.settings.gameSettings = new Settings.GameSettings(sensitivytySlider.value, reticuleToggle.isOn);
        SaveSettings();
    }
    #endregion

    public void SaveSettings()
    {
        SettingsLoader.SaveSettings(GameManager.instance.settings);
    }
}
