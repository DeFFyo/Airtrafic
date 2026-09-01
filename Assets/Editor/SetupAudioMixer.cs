#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using System.Reflection;

public class SetupAudioMixer
{
    [MenuItem("Tools/Setup Audio Mixer")]
    public static void Setup()
    {
        string dir = "Assets/Audio";
        string path = dir + "/MainMixer.mixer";
        if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets", "Audio");

        var ctrlType = System.Type.GetType("UnityEditor.Audio.AudioMixerController, UnityEditor.CoreModule");
        if (ctrlType == null) { Debug.LogError("[SetupAudioMixer] AudioMixerController type not found"); return; }

        object controller = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        bool created = false;
        if (controller == null || controller.GetType().FullName != ctrlType.FullName)
        {
            var createM = ctrlType.GetMethod("CreateMixerControllerAtPath", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            controller = createM.Invoke(null, new object[] { path });
            created = true;
        }

        var masterProp = ctrlType.GetProperty("masterGroup", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        object master = masterProp.GetValue(controller, null);

        object music = FindOrCreateChild(ctrlType, controller, "Music", master);
        object sfx = FindOrCreateChild(ctrlType, controller, "SFX", master);

        Expose(ctrlType, controller, master, "MasterVol");
        Expose(ctrlType, controller, music, "MusicVol");
        Expose(ctrlType, controller, sfx, "SfxVol");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var setFloatM = ctrlType.GetMethod("SetFloat", new[] { typeof(string), typeof(float) });
        bool okM = (bool)setFloatM.Invoke(controller, new object[] { "MasterVol", -6f });
        bool okMu = (bool)setFloatM.Invoke(controller, new object[] { "MusicVol", -6f });
        bool okS = (bool)setFloatM.Invoke(controller, new object[] { "SfxVol", -6f });
        Debug.Log("[SetupAudioMixer] created=" + created + " SetFloat M=" + okM + " Mu=" + okMu + " S=" + okS);
        EditorUtility.DisplayDialog("Audio Mixer", "Mixer ready at " + path + "\nSetFloat M=" + okM + " Mu=" + okMu + " S=" + okS, "OK");
    }

    static object FindOrCreateChild(System.Type ctrlType, object controller, string name, object parent)
    {
        var findM = ctrlType.GetMethod("FindMatchingGroups", new[] { typeof(string) });
        var existing = (AudioMixerGroup[])findM.Invoke(controller, new object[] { name });
        if (existing != null && existing.Length > 0) return existing[0];

        var createM = ctrlType.GetMethod("CreateNewGroup", new[] { typeof(string), typeof(AudioMixerGroup) });
        int argc = 2;
        if (createM == null) { createM = ctrlType.GetMethod("CreateNewGroup", new[] { typeof(string) }); argc = 1; }
        object newGroup = (argc == 2)
            ? createM.Invoke(controller, new object[] { name, parent })
            : createM.Invoke(controller, new object[] { name });
        if (argc == 1)
        {
            var addChild = ctrlType.GetMethod("AddChildToParent", new[] { typeof(AudioMixerGroup), typeof(AudioMixerGroup) });
            if (addChild != null) addChild.Invoke(controller, new object[] { newGroup, parent });
        }
        return newGroup;
    }

    static void Expose(System.Type ctrlType, object controller, object group, string param)
    {
        var containsM = ctrlType.GetMethod("ContainsExposedParameter", new[] { typeof(string) });
        bool has = (bool)containsM.Invoke(controller, new object[] { param });
        if (has) return;
        var addM = ctrlType.GetMethod("AddExposedParameter", new[] { typeof(AudioMixerGroup), typeof(string) });
        int argc = 2;
        if (addM == null) { addM = ctrlType.GetMethod("AddExposedParameter", new[] { typeof(string) }); argc = 1; }
        if (argc == 2) addM.Invoke(controller, new object[] { group, param });
        else addM.Invoke(controller, new object[] { param });
    }
}
#endif
