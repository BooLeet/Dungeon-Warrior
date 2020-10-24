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
        public bool bloom, filmGrain, AO;
        public byte renderResolution;//1 = 1/8, 2 = 1/4, 3 = 1/2, 4 = 1
        public byte frameRate;//1 = 30,2 = 60,3 = uncapped

        public GraphicSettings(bool bloom, bool filmGrain, bool AO, byte renderResolution, byte frameRate)
        {
            this.bloom = bloom;
            this.filmGrain = filmGrain;
            this.AO = AO;
            this.renderResolution = (byte)Mathf.Clamp(renderResolution, 1, 4);
            this.frameRate = (byte)Mathf.Clamp(frameRate, 1, 3);
        }

        public static GraphicSettings Default()
        {
            return new GraphicSettings(false, false, false, 1,2);
        }

        public int GetTargetFrameRate()
        {
            switch (frameRate)
            {
                case 1: return 30;
                case 2: return 60;
                default: return 120;
            }
        }

        public float GetResolutionScale()
        {
            switch(renderResolution)
            {
                case 1: return 1 / 8f;
                case 2: return 1 / 4f;
                case 3: return 1 / 2f;
                default: return 1;
            }
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
            this.mouseSensitivity = mouseSensitivity;
            this.showReticule = showReticule;
        }

        public static GameSettings Default()
        {
            return new GameSettings(2.5f, true);
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

    public void ApplyGraphicsSettings(PostProcessVolume[] volumes, RenderResolutionScaler resolutionScaler)
    {
        foreach(PostProcessVolume volume in volumes)
        {
            Bloom bloom = null;
            Grain filmGrain = null;
            AmbientOcclusion AO = null;

            
            volume.profile.TryGetSettings<Bloom>(out bloom);
            volume.profile.TryGetSettings<Grain>(out filmGrain);
            volume.profile.TryGetSettings<AmbientOcclusion>(out AO);

            if(bloom)
                bloom.enabled.value = graphicSettings.bloom;
            if(filmGrain)
                filmGrain.enabled.value = graphicSettings.filmGrain;
            if(AO)
                AO.enabled.value = graphicSettings.AO;
        }

        resolutionScaler.ApplyResolutionScale(graphicSettings.GetResolutionScale());
    }

    public void ApplyGraphicsSettings()
    {
        ApplyGraphicsSettings(GameManager.instance.postProcess, GameManager.instance.resolutionScaler);
        Application.targetFrameRate = graphicSettings.GetTargetFrameRate();
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


