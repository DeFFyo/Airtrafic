using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SettingsPanel : MonoBehaviour
{
    public Slider masterSlider, musicSlider, sfxSlider;
    public Dropdown resolutionDropdown;
    public Dropdown displayModeDropdown;
    public Button applyButton, backButton;
    public Text headerText, masterLabel, musicLabel, sfxLabel, resolutionLabel, displayModeLabel;
    public MenuManager menuManager;
    public AudioMixer mixer;
    public MenuStrings strings;

    private Resolution[] resolutions;
    private int pendingWidth, pendingHeight;
    private int pendingModeIndex;

    private static readonly FullScreenMode[] ModeMap = new FullScreenMode[]
    {
        FullScreenMode.Windowed,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.ExclusiveFullScreen
    };

    void Awake()
    {
        if (strings == null) strings = Resources.Load<MenuStrings>("MenuStrings");
        SettingsManager.Mixer = mixer;
        SetLabels();
        LoadCurrent();
        Wire();
    }

    void OnEnable()
    {
        if (resolutionDropdown != null && resolutionDropdown.options.Count == 0)
            LoadCurrent();
    }

    void SetLabels()
    {
        headerText.text = strings.settingsTitle;
        masterLabel.text = strings.master;
        musicLabel.text = strings.music;
        sfxLabel.text = strings.sfx;
        resolutionLabel.text = strings.resolution;
        displayModeLabel.text = strings.displayMode;
        applyButton.GetComponentInChildren<Text>().text = strings.apply;
        backButton.GetComponentInChildren<Text>().text = strings.back;
    }

    int ModeToIndex(FullScreenMode m)
    {
        for (int i = 0; i < ModeMap.Length; i++)
            if (ModeMap[i] == m) return i;
        return 1;
    }

    void LoadCurrent()
    {
        masterSlider.value = SettingsManager.LoadAudio(SettingsManager.MasterKey, 1f);
        musicSlider.value = SettingsManager.LoadAudio(SettingsManager.MusicKey, 1f);
        sfxSlider.value = SettingsManager.LoadAudio(SettingsManager.SfxKey, 1f);
        ApplyAudioLive();

        resolutions = Screen.resolutions;
        var opts = new List<string>();
        int cur = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            opts.Add(r.width + " x " + r.height);
            if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height)
                cur = i;
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(opts);
        resolutionDropdown.value = cur;
        resolutionDropdown.RefreshShownValue();

        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(new List<string>(new[]
        {
            strings.displayWindowed,
            strings.displayFullscreenWindow,
            strings.displayExclusive
        }));
        int modeIdx = ModeToIndex(Screen.fullScreenMode);
        displayModeDropdown.value = modeIdx;
        displayModeDropdown.RefreshShownValue();

        pendingWidth = resolutions[cur].width;
        pendingHeight = resolutions[cur].height;
        pendingModeIndex = modeIdx;
    }

    void ApplyAudioLive()
    {
        SettingsManager.ApplyAudio("MasterVol", masterSlider.value);
        SettingsManager.ApplyAudio("MusicVol", musicSlider.value);
        SettingsManager.ApplyAudio("SfxVol", sfxSlider.value);
    }

    void Wire()
    {
        masterSlider.onValueChanged.AddListener(v =>
        {
            ApplyAudioLive();
            SettingsManager.SaveAudio(SettingsManager.MasterKey, v);
        });
        musicSlider.onValueChanged.AddListener(v =>
        {
            ApplyAudioLive();
            SettingsManager.SaveAudio(SettingsManager.MusicKey, v);
        });
        sfxSlider.onValueChanged.AddListener(v =>
        {
            ApplyAudioLive();
            SettingsManager.SaveAudio(SettingsManager.SfxKey, v);
        });

        resolutionDropdown.onValueChanged.AddListener(i =>
        {
            pendingWidth = resolutions[i].width;
            pendingHeight = resolutions[i].height;
        });
        displayModeDropdown.onValueChanged.AddListener(i => { pendingModeIndex = i; });

        applyButton.onClick.AddListener(ApplyVideo);
        backButton.onClick.AddListener(menuManager.ShowMain);
    }

    void ApplyVideo()
    {
        Screen.SetResolution(pendingWidth, pendingHeight, ModeMap[pendingModeIndex]);
        PlayerPrefs.SetInt(SettingsManager.ResWKey, pendingWidth);
        PlayerPrefs.SetInt(SettingsManager.ResHKey, pendingHeight);
        PlayerPrefs.SetInt(SettingsManager.ModeKey, pendingModeIndex);
        PlayerPrefs.Save();
    }
}
