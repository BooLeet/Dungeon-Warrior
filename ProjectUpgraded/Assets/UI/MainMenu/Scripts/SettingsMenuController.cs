using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    [Header("Graphics")]
    public Toggle bloomToggle;
    public Toggle filmGrainToggle;
    public Toggle AO_Toggle;
    public Slider resolutionSlider;
    public Slider frameRateSlider;

    [Header("Audio")]
    public Slider sfxVolumeSlider; 
    public Slider musicVolumeSlider;

    [Header("Gameplay")]
    public Slider sensitivytySlider;
    public Toggle reticuleToggle;

    [Header("Key bindings")]
    public SettingKeyMapper keyMapper;


    void Start()
    {
        keyMapper.FillKeyButtons();
    }

    #region Graphics
    public void LoadGraphicsValues()
    {
        Settings.GraphicSettings graphicSettings = GameManager.instance.settings.graphicSettings;
        bloomToggle.isOn = graphicSettings.bloom;
        filmGrainToggle.isOn = graphicSettings.filmGrain;
        AO_Toggle.isOn = graphicSettings.AO;
        resolutionSlider.value = graphicSettings.renderResolution;
        frameRateSlider.value = graphicSettings.frameRate;
    }

    public void ApplyGraphicsSettings()
    {
        GameManager.instance.settings.graphicSettings = new Settings.GraphicSettings(bloomToggle.isOn, filmGrainToggle.isOn, AO_Toggle.isOn, (byte)resolutionSlider.value, (byte)frameRateSlider.value);
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

    #region Key

    public void ApplyKeySettings()
    {
        GameManager.instance.settings.keyMapSettings = new Settings.KeyMapSettings(keyMapper.GetMappedKeys());
        GameManager.instance.settings.ApplyKeyMapSettings();
        SaveSettings();
        keyMapper.FillKeyButtons();
    }

    #endregion

    public void GoBack()
    {
        mainMenuController.GoBack();
    }

    public void SaveSettings()
    {
        SettingsLoader.SaveSettings(GameManager.instance.settings);
        mainMenuController.GoBack();
    }
}
