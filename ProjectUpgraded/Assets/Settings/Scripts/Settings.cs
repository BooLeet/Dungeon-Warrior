using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Settings
{
    [System.Serializable]
    public struct GraphicSettings
    {
        public bool motionBlur, filmGrain, AO;
        public byte shadowQuality;

        public GraphicSettings(bool motionBlur, bool filmGrain, bool AO, byte shadowQuality)
        {
            this.motionBlur = motionBlur;
            this.filmGrain = filmGrain;
            this.AO = AO;
            this.shadowQuality = (byte)Mathf.Clamp(shadowQuality, 1, 4);
        }

        public static GraphicSettings Default()
        {
            return new GraphicSettings(true, true, true, 4);
        }
    }

    [System.Serializable]
    public struct AudioSettings
    {
        public float sfxVolume, musicVolume;

        public AudioSettings(float sfxVolume, float musicVolume)
        {
            this.sfxVolume = Mathf.Clamp(sfxVolume, 0.0001f, 1);
            this.musicVolume = Mathf.Clamp(musicVolume, 0.0001f, 1);
        }

        public static AudioSettings Default()
        {
            return new AudioSettings(1, 1);
        }
    }

    [System.Serializable]
    public struct GameSettings
    {
        public float mouseSensitivity;
        public bool showReticule;

        public GameSettings(float mouseSensitivity, bool showReticule)
        {
            this.mouseSensitivity = Mathf.Clamp(mouseSensitivity, 0.001f, 1);
            this.showReticule = showReticule;
        }

        public static GameSettings Default()
        {
            return new GameSettings(0.5f, true);
        }
    }

    public GraphicSettings graphicSettings;
    public AudioSettings audioSettings;
    public GameSettings gameSettings;

    public Settings()
    {
        graphicSettings = GraphicSettings.Default();
        audioSettings = AudioSettings.Default();
        gameSettings = GameSettings.Default();
    }

    public Settings(GraphicSettings graphicSettings, AudioSettings audioSettings, GameSettings gameSettings)
    {
        this.graphicSettings = graphicSettings;
        this.audioSettings = audioSettings;
        this.gameSettings = gameSettings;
    }

    public void ApplyAllSettings()
    {
        ApplyGraphicsSettings();
        ApplyAudioSettings();
        ApplyGameplaySettings();
    }

    public void ApplyGraphicsSettings(PostProcessVolume[] volumes)
    {
        foreach(PostProcessVolume volume in volumes)
        {
            MotionBlur motionBlur = null;
            Grain filmGrain = null;
            AmbientOcclusion AO = null;

            volume.profile.TryGetSettings<MotionBlur>(out motionBlur);
            volume.profile.TryGetSettings<Grain>(out filmGrain);
            volume.profile.TryGetSettings<AmbientOcclusion>(out AO);

            if(motionBlur)
                motionBlur.enabled.value = graphicSettings.motionBlur;
            if(filmGrain)
                filmGrain.enabled.value = graphicSettings.filmGrain;
            if(AO)
                AO.enabled.value = graphicSettings.AO;
        }
    }

    public void ApplyGraphicsSettings()
    {
        ApplyGraphicsSettings(GameManager.instance.postProcess);
    }

    public void ApplyAudioSettings(AudioMixer sfxMixer, AudioMixer musicMixer)
    {
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(audioSettings.sfxVolume, 0.0001f, 1)) * 20);
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(audioSettings.musicVolume, 0.0001f, 1)) * 20);
    }

    public void ApplyAudioSettings()
    {
        ApplyAudioSettings(GameManager.instance.sfxMixer, GameManager.instance.musicMixer);
    }

    public void ApplyGameplaySettings()
    {

    }
}

// Saves and loads setings
public static class SettingsLoader
{
    public static void SaveSettings(Settings settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Settings.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, settings);
        stream.Close();
    }

    public static Settings LoadSettings()
    {
        string path = Application.persistentDataPath + "/Settings.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Settings settings = formatter.Deserialize(stream) as Settings;
            stream.Close();
            return settings;
        }

        return new Settings();
    }
}


