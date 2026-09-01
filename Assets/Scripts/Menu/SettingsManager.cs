using UnityEngine;
using UnityEngine.Audio;

public static class SettingsManager
{
    public const string MasterKey = "Vol_Master";
    public const string MusicKey = "Vol_Music";
    public const string SfxKey = "Vol_Sfx";
    public const string ResWKey = "Res_Width";
    public const string ResHKey = "Res_Height";
    public const string ModeKey = "DisplayMode";

    public static AudioMixer Mixer;

    public static float LinearToDb(float linear)
    {
        if (linear <= 0.0001f) return -80f;
        return 20f * Mathf.Log10(linear);
    }

    public static void ApplyAudio(string exposedParam, float linear)
    {
        if (Mixer != null) Mixer.SetFloat(exposedParam, LinearToDb(linear));
    }

    public static void SaveAudio(string key, float linear)
    {
        PlayerPrefs.SetFloat(key, linear);
    }

    public static float LoadAudio(string key, float defaultVal)
    {
        return PlayerPrefs.GetFloat(key, defaultVal);
    }
}
