#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

public class VerifyMixer
{
    [MenuItem("Tools/Verify Audio Mixer")]
    public static void Verify()
    {
        var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Audio/MainMixer.mixer");
        if (mixer == null) { Debug.LogError("[VerifyMixer] mixer not found"); return; }
        bool m = mixer.SetFloat("MasterVol", -6f);
        bool mu = mixer.SetFloat("MusicVol", -6f);
        bool s = mixer.SetFloat("SfxVol", -6f);
        Debug.Log("[VerifyMixer] SetFloat Master=" + m + " Music=" + mu + " Sfx=" + s);
    }
}
#endif
