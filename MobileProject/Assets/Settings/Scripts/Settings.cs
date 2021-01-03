using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

[System.Serializable]
public class Settings
{
    [System.Serializable]
    public struct GraphicSettings
    {
        public bool bloom, filmGrain, AO, motionBlur,showFPS;
        public byte renderResolution;//1 = 1/8, 2 = 1/4, 3 = 1/2, 4 = 1
        public byte frameRate;//1 = 30,2 = 60,3 = uncapped

        public GraphicSettings(bool bloom, bool filmGrain, bool AO, bool motionBlur, byte renderResolution, byte frameRate,bool showFPS)
        {
            this.bloom = bloom;
            this.filmGrain = filmGrain;
            this.AO = AO;
            this.motionBlur = motionBlur;
            this.renderResolution = (byte)Mathf.Clamp(renderResolution, 1, 4);
            this.frameRate = (byte)Mathf.Clamp(frameRate, 1, 3);
            this.showFPS = showFPS;
        }

        public static GraphicSettings Default()
        {
            return new GraphicSettings(false, false, false, false, 4, 2, false);
        }

        public int GetTargetFrameRate()
        {
            switch (frameRate)
            {
                case 1: return 30;
                case 2: return 60;
                default: return 1000;
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

    [System.Serializable]
    public struct KeyMapSettings
    {
        [System.Serializable]
        public struct Key
        {
            public string name;
            public KeyCode keyCode;
            public string localizationKey;

            public Key(string name, string localizationKey, KeyCode keyCode)
            {
                this.name = name;
                this.localizationKey = localizationKey;
                this.keyCode = keyCode;
            }
        }

        public List<Key> keys;

        /// <summary>
        /// Returns a list of default keys
        /// </summary>
        /// <returns></returns>
        public static List<Key> GetDefaultKeys()
        {
            List<Key> keyInfo = new List<Key>();
            keyInfo.Add(new Key("forward", "keyForward", KeyCode.W));
            keyInfo.Add(new Key("backwards", "keyBackwards", KeyCode.S));
            keyInfo.Add(new Key("strafeRight", "keyStrafeRight", KeyCode.D));
            keyInfo.Add(new Key("strafeLeft", "keyStrafeLeft", KeyCode.A));

            keyInfo.Add(new Key("primaryAttack", "keyPrimaryAttack", KeyCode.Mouse0));
            keyInfo.Add(new Key("secondaryAttack", "keySecondaryAttack", KeyCode.Mouse1));
            keyInfo.Add(new Key("interact", "keyInteract", KeyCode.E));
            keyInfo.Add(new Key("jump", "keyJump", KeyCode.Space));
            keyInfo.Add(new Key("dash", "keyDash", KeyCode.LeftShift));
            keyInfo.Add(new Key("forcePush", "keyForcePush", KeyCode.V));
            keyInfo.Add(new Key("inspect", "keyInspect", KeyCode.F));
            keyInfo.Add(new Key("hideUI", "keyHideUI", KeyCode.U));

            keyInfo.Add(new Key("back", "keyBack", KeyCode.Tab));

            return keyInfo;
        }

        public KeyMapSettings(List<Key> keys)
        {
            this.keys = keys;
        }

        public static KeyMapSettings Default()
        {
            return new KeyMapSettings(GetDefaultKeys());
        }

        public KeyCode GetKeyCode(string keyName)
        {
            if (keys == null)
                return KeyCode.None;

            return keys.Find(key => key.name == keyName).keyCode;
        }

    }


    public GraphicSettings graphicSettings;
    public AudioSettings audioSettings;
    public GameSettings gameSettings;
    public KeyMapSettings keyMapSettings;

    public Settings()
    {
        graphicSettings = GraphicSettings.Default();
        audioSettings = AudioSettings.Default();
        gameSettings = GameSettings.Default();
        keyMapSettings = KeyMapSettings.Default();
    }

    public Settings(GraphicSettings graphicSettings, AudioSettings audioSettings, GameSettings gameSettings, KeyMapSettings keyMapSettings)
    {
        this.graphicSettings = graphicSettings;
        this.audioSettings = audioSettings;
        this.gameSettings = gameSettings;
        this.keyMapSettings = keyMapSettings;
    }

    public void ApplyAllSettings()
    {
        ApplyGraphicsSettings();
        ApplyAudioSettings();
        ApplyGameplaySettings();
        ApplyKeyMapSettings();
    }

    public void ApplyGraphicsSettings(PostProcessVolume[] volumes, RenderResolutionScaler resolutionScaler)
    {
        foreach(PostProcessVolume volume in volumes)
        {
            Bloom bloom = null;
            Grain filmGrain = null;
            AmbientOcclusion AO = null;
            MotionBlur motionBlur = null;
            
            volume.profile.TryGetSettings<Bloom>(out bloom);
            volume.profile.TryGetSettings<Grain>(out filmGrain);
            volume.profile.TryGetSettings<AmbientOcclusion>(out AO);
            volume.profile.TryGetSettings<MotionBlur>(out motionBlur);

            if (bloom)
                bloom.enabled.value = graphicSettings.bloom;
            if(filmGrain)
                filmGrain.enabled.value = graphicSettings.filmGrain;
            if(AO)
                AO.enabled.value = graphicSettings.AO;
            if (motionBlur)
                motionBlur.enabled.value = graphicSettings.motionBlur;
        }
        if(resolutionScaler.enabled)
            resolutionScaler.ApplyResolutionScale(graphicSettings.GetResolutionScale());

    }

    public void ApplyGraphicsSettings()
    {
        ApplyGraphicsSettings(GameManager.instance.postProcess, GameManager.instance.resolutionScaler);
        GameManager.instance.fpsCounter.enabled = graphicSettings.showFPS;
        int targetFrameRate = graphicSettings.GetTargetFrameRate();
        if (targetFrameRate > 60)
            QualitySettings.vSyncCount = 0;
        else
            QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = targetFrameRate;
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

    public void ApplyKeyMapSettings()
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


