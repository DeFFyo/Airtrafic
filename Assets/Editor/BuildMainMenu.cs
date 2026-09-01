#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections.Generic;

public class BuildMainMenu
{
    [MenuItem("Tools/Build Main Menu")]
    public static void Build()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        Scene scene = SceneManager.GetSceneByPath(scenePath);

        // Remove any previously built canvas / event system to avoid duplicates
        var allGOs = Object.FindObjectsOfType<GameObject>();
        var roots = new System.Collections.Generic.List<GameObject>();
        foreach (var g in allGOs) if (g.transform.parent == null) roots.Add(g);
        foreach (var go in roots)
        {
            if (go.name == "MenuCanvas" || go.name == "EventSystem")
                Object.DestroyImmediate(go);
        }

        // ---- Canvas + EventSystem (built directly, no menu) ----
        var canvasGO = new GameObject("MenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        System.Func<string, GameObject, GameObject> MakePanel = (name, parent) =>
        {
            var p = new GameObject(name, typeof(RectTransform), typeof(Image));
            p.transform.SetParent(parent.transform, false);
            var rt = p.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            p.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.16f, 1f);
            return p;
        };

        System.Func<GameObject, GameObject> MakeContainer = (panel) =>
        {
            var c = new GameObject("Container", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            c.transform.SetParent(panel.transform, false);
            var rt = c.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f); rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(600, 0);
            var vlg = c.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 24; vlg.childControlWidth = true; vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;
            var csf = c.GetComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return c;
        };

        System.Func<string, GameObject, GameObject> MakeButton = (name, parent) =>
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent.transform, false);
            var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(400, 70);
            var img = go.GetComponent<Image>(); img.color = new Color(0.25f, 0.35f, 0.55f, 1f);
            var btn = go.GetComponent<Button>(); btn.targetGraphic = img;
            var le = go.AddComponent<LayoutElement>(); le.preferredHeight = 70; le.preferredWidth = 400;
            var txt = new GameObject("Text", typeof(RectTransform), typeof(Text));
            txt.transform.SetParent(go.transform, false);
            var tRT = txt.GetComponent<RectTransform>(); tRT.anchorMin = Vector2.zero; tRT.anchorMax = Vector2.one; tRT.offsetMin = Vector2.zero; tRT.offsetMax = Vector2.zero;
            var t = txt.GetComponent<Text>(); t.text = name; t.alignment = TextAnchor.MiddleCenter; t.color = Color.white; t.fontSize = 36;
            return go;
        };

        System.Func<string, string, GameObject, int, GameObject> MakeText = (name, label, parent, fontSize) =>
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent.transform, false);
            var t = go.GetComponent<Text>(); t.text = label; t.fontSize = fontSize; t.alignment = TextAnchor.MiddleCenter; t.color = Color.white;
            return go;
        };

        System.Func<string, string, GameObject, Slider> MakeSliderRow = (labelName, label, parent) =>
        {
            var row = new GameObject(labelName + "Row", typeof(RectTransform), typeof(VerticalLayoutGroup));
            row.transform.SetParent(parent.transform, false);
            var rvg = row.GetComponent<VerticalLayoutGroup>();
            rvg.spacing = 6; rvg.childControlWidth = true; rvg.childControlHeight = true; rvg.childForceExpandWidth = true; rvg.childForceExpandHeight = false;
            row.AddComponent<LayoutElement>().preferredHeight = 50;
            var sliderLabel = MakeText(labelName + "Label", label, row, 30);
            sliderLabel.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            sliderLabel.AddComponent<LayoutElement>().preferredWidth = 220;

            var go = new GameObject(labelName + "Slider", typeof(RectTransform), typeof(Slider));
            go.transform.SetParent(row.transform, false);
            var srt = go.GetComponent<RectTransform>(); srt.sizeDelta = new Vector2(400, 20);
            var sle = go.AddComponent<LayoutElement>(); sle.preferredHeight = 30; sle.flexibleWidth = 1;
            var slider = go.GetComponent<Slider>();

            var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(go.transform, false);
            var bgRT = bg.GetComponent<RectTransform>(); bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one; bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;
            bg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(go.transform, false);
            var faRT = fillArea.GetComponent<RectTransform>(); faRT.anchorMin = new Vector2(0, 0.5f); faRT.anchorMax = new Vector2(1, 0.5f); faRT.offsetMin = new Vector2(5, 0); faRT.offsetMax = new Vector2(-5, 0);
            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            var fRT = fill.GetComponent<RectTransform>(); fRT.anchorMin = Vector2.zero; fRT.anchorMax = Vector2.one; fRT.offsetMin = Vector2.zero; fRT.offsetMax = Vector2.zero;
            fill.GetComponent<Image>().color = Color.white;

            var hsa = new GameObject("Handle Slide Area", typeof(RectTransform));
            hsa.transform.SetParent(go.transform, false);
            var hsaRT = hsa.GetComponent<RectTransform>(); hsaRT.anchorMin = Vector2.zero; hsaRT.anchorMax = Vector2.one; hsaRT.offsetMin = Vector2.zero; hsaRT.offsetMax = Vector2.zero;
            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(hsa.transform, false);
            var hRT = handle.GetComponent<RectTransform>(); hRT.sizeDelta = new Vector2(20, 20); hRT.anchorMin = new Vector2(0, 0.5f); hRT.anchorMax = new Vector2(0, 0.5f); hRT.anchoredPosition = Vector2.zero;
            handle.GetComponent<Image>().color = Color.white;

            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = bg.GetComponent<Image>();
            slider.value = 1f;
            return slider;
        };

        System.Func<string, string, GameObject, Dropdown> MakeDropdownRow = (name, label, parent) =>
        {
            var row = new GameObject(name + "Row", typeof(RectTransform), typeof(VerticalLayoutGroup));
            row.transform.SetParent(parent.transform, false);
            var rvg = row.GetComponent<VerticalLayoutGroup>();
            rvg.spacing = 6; rvg.childControlWidth = true; rvg.childControlHeight = true; rvg.childForceExpandWidth = true; rvg.childForceExpandHeight = false;
            row.AddComponent<LayoutElement>().preferredHeight = 50;
            var ddLabel = MakeText(name + "Label", label, row, 30);
            ddLabel.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            ddLabel.AddComponent<LayoutElement>().preferredWidth = 220;

            var go = new GameObject(name + "Dropdown", typeof(RectTransform), typeof(Image), typeof(Dropdown));
            go.transform.SetParent(row.transform, false);
            var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(400, 30);
            var dle = go.AddComponent<LayoutElement>(); dle.preferredHeight = 30; dle.flexibleWidth = 1;
            go.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f, 1f);

            var captionGO = new GameObject("Label", typeof(RectTransform), typeof(Text));
            captionGO.transform.SetParent(go.transform, false);
            var lRT = captionGO.GetComponent<RectTransform>(); lRT.anchorMin = Vector2.zero; lRT.anchorMax = Vector2.one; lRT.offsetMin = new Vector2(10, 0); lRT.offsetMax = new Vector2(-10, 0);
            var lt = captionGO.GetComponent<Text>(); lt.text = "Option"; lt.alignment = TextAnchor.MiddleLeft; lt.color = Color.black;

            var arrow = new GameObject("Arrow", typeof(RectTransform), typeof(Image));
            arrow.transform.SetParent(go.transform, false);
            var aRT = arrow.GetComponent<RectTransform>(); aRT.anchorMin = new Vector2(1, 0.5f); aRT.anchorMax = new Vector2(1, 0.5f); aRT.sizeDelta = new Vector2(20, 20); aRT.anchoredPosition = Vector2.zero;
            arrow.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);

            var template = new GameObject("Template", typeof(RectTransform), typeof(Image));
            template.transform.SetParent(go.transform, false);
            var tRT = template.GetComponent<RectTransform>(); tRT.anchorMin = new Vector2(0, 0); tRT.anchorMax = new Vector2(1, 0); tRT.pivot = new Vector2(0.5f, 1); tRT.sizeDelta = new Vector2(0, -150);
            template.SetActive(false);
            template.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewport.transform.SetParent(template.transform, false);
            var vpRT = viewport.GetComponent<RectTransform>(); vpRT.anchorMin = Vector2.zero; vpRT.anchorMax = Vector2.one; vpRT.offsetMin = Vector2.zero; vpRT.offsetMax = Vector2.zero;
            var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup));
            content.transform.SetParent(viewport.transform, false);
            var cRT = content.GetComponent<RectTransform>(); cRT.anchorMin = Vector2.zero; cRT.anchorMax = Vector2.one; cRT.offsetMin = Vector2.zero; cRT.offsetMax = Vector2.zero;
            var item = new GameObject("Item", typeof(RectTransform), typeof(Image), typeof(Toggle));
            item.transform.SetParent(content.transform, false);
            var iRT = item.GetComponent<RectTransform>(); iRT.sizeDelta = new Vector2(0, 20);
            var itemLabel = new GameObject("Item Label", typeof(RectTransform), typeof(Text));
            itemLabel.transform.SetParent(item.transform, false);
            var ilRT = itemLabel.GetComponent<RectTransform>(); ilRT.anchorMin = Vector2.zero; ilRT.anchorMax = Vector2.one; ilRT.offsetMin = new Vector2(10, 0); ilRT.offsetMax = new Vector2(-10, 0);
            var ilt = itemLabel.GetComponent<Text>(); ilt.text = "Option"; ilt.color = Color.black;

            var dropdown = go.GetComponent<Dropdown>();
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.captionText = captionGO.GetComponent<Text>();
            dropdown.itemText = ilt;
            dropdown.itemImage = null;
            dropdown.targetGraphic = go.GetComponent<Image>();
            return dropdown;
        };

        // ---------- Main Panel ----------
        GameObject mainPanel = MakePanel("MainPanel", canvasGO);
        GameObject mainContainer = MakeContainer(mainPanel);
        var title = MakeText("Title", "МОЯ ИГРА", mainContainer, 72);
        var startBtn = MakeButton("StartButton", mainContainer);
        var settingsBtn = MakeButton("SettingsButton", mainContainer);
        var exitBtn = MakeButton("ExitButton", mainContainer);

        // ---------- Level Select Panel ----------
        GameObject levelPanel = MakePanel("LevelSelectPanel", canvasGO);
        GameObject levelContainer = MakeContainer(levelPanel);
        var levelHeader = MakeText("Header", "Выбор уровня", levelContainer, 56);
        var levelContent = new GameObject("LevelContent", typeof(RectTransform), typeof(VerticalLayoutGroup));
        levelContent.transform.SetParent(levelContainer.transform, false);
        var lvg = levelContent.GetComponent<VerticalLayoutGroup>();
        lvg.spacing = 16; lvg.childControlWidth = true; lvg.childControlHeight = true; lvg.childForceExpandWidth = true; lvg.childForceExpandHeight = false;
        var levelBack = MakeButton("BackButton", levelContainer);

        // ---------- Settings Panel ----------
        GameObject settingsPanel = MakePanel("SettingsPanel", canvasGO);
        GameObject settingsContainer = MakeContainer(settingsPanel);
        var setHeader = MakeText("Header", "Настройки", settingsContainer, 56);
        var masterSlider = MakeSliderRow("Master", "Общий звук", settingsContainer);
        var musicSlider = MakeSliderRow("Music", "Музыка", settingsContainer);
        var sfxSlider = MakeSliderRow("Sfx", "Звуки", settingsContainer);
        var resDropdown = MakeDropdownRow("Resolution", "Разрешение", settingsContainer);
        var modeDropdown = MakeDropdownRow("DisplayMode", "Режим отображения", settingsContainer);
        var applyBtn = MakeButton("ApplyButton", settingsContainer);
        var settingsBack = MakeButton("BackButton", settingsContainer);

        // ---------- Modal Dialog ----------
        GameObject modal = MakePanel("ModalDialog", canvasGO);
        modal.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
        GameObject modalPanel = new GameObject("DialogPanel", typeof(RectTransform), typeof(Image));
        modalPanel.transform.SetParent(modal.transform, false);
        var mrt = modalPanel.GetComponent<RectTransform>();
        mrt.anchorMin = new Vector2(0.5f, 0.5f); mrt.anchorMax = new Vector2(0.5f, 0.5f);
        mrt.sizeDelta = new Vector2(600, 300); mrt.anchoredPosition = Vector2.zero;
        modalPanel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 1f);
        GameObject modalContainer = MakeContainer(modalPanel);
        var dlgTitle = MakeText("DialogTitle", "Заголовок", modalContainer, 40);
        var dlgMsg = MakeText("DialogMessage", "Сообщение", modalContainer, 28);
        var dlgYes = MakeButton("DialogYes", modalContainer);
        var dlgNo = MakeButton("DialogNo", modalContainer);

        // ---------- Add logic components & wire ----------
        MenuManager mm = canvasGO.AddComponent<MenuManager>();
        MainMenuUI mmui = canvasGO.AddComponent<MainMenuUI>();
        LevelSelectPanel lsp = levelPanel.AddComponent<LevelSelectPanel>();
        SettingsPanel sp = settingsPanel.AddComponent<SettingsPanel>();
        ModalDialog md = modal.AddComponent<ModalDialog>();

        mm.mainPanel = mainPanel;
        mm.settingsPanel = settingsPanel;
        mm.levelSelectPanel = levelPanel;
        mm.modalDialog = md;

        mmui.startButton = startBtn.GetComponent<Button>();
        mmui.settingsButton = settingsBtn.GetComponent<Button>();
        mmui.exitButton = exitBtn.GetComponent<Button>();
        mmui.titleText = title.GetComponent<Text>();
        mmui.menuManager = mm;
        mmui.modalDialog = md;

        lsp.levelListContent = levelContent.transform;
        lsp.backButton = levelBack.GetComponent<Button>();
        lsp.headerText = levelHeader.GetComponent<Text>();
        lsp.menuManager = mm;

        sp.masterSlider = masterSlider;
        sp.musicSlider = musicSlider;
        sp.sfxSlider = sfxSlider;
        sp.resolutionDropdown = resDropdown;
        sp.displayModeDropdown = modeDropdown;
        sp.applyButton = applyBtn.GetComponent<Button>();
        sp.backButton = settingsBack.GetComponent<Button>();
        sp.headerText = setHeader.GetComponent<Text>();
        sp.masterLabel = masterSlider.transform.parent.Find("MasterLabel").GetComponent<Text>();
        sp.musicLabel = musicSlider.transform.parent.Find("MusicLabel").GetComponent<Text>();
        sp.sfxLabel = sfxSlider.transform.parent.Find("SfxLabel").GetComponent<Text>();
        sp.resolutionLabel = resDropdown.transform.parent.Find("ResolutionLabel").GetComponent<Text>();
        sp.displayModeLabel = modeDropdown.transform.parent.Find("DisplayModeLabel").GetComponent<Text>();
        sp.menuManager = mm;
        var mixerObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Audio/MainMixer.mixer");
        if (mixerObj != null) { var so = new SerializedObject(sp); so.FindProperty("mixer").objectReferenceValue = mixerObj; so.ApplyModifiedProperties(); }

        md.titleText = dlgTitle.GetComponent<Text>();
        md.messageText = dlgMsg.GetComponent<Text>();
        md.yesButton = dlgYes.GetComponent<Button>();
        md.noButton = dlgNo.GetComponent<Button>();

        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        levelPanel.SetActive(false);
        modal.SetActive(false);

        // ---------- Data assets ----------
        if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");

        MenuStrings str = AssetDatabase.LoadAssetAtPath<MenuStrings>("Assets/Resources/MenuStrings.asset");
        if (str == null) { str = ScriptableObject.CreateInstance<MenuStrings>(); AssetDatabase.CreateAsset(str, "Assets/Resources/MenuStrings.asset"); }
        mm.strings = str; mmui.strings = str; lsp.strings = str; sp.strings = str; md.strings = str;

        LevelList ll = AssetDatabase.LoadAssetAtPath<LevelList>("Assets/Resources/LevelList.asset");
        if (ll == null) { ll = ScriptableObject.CreateInstance<LevelList>(); AssetDatabase.CreateAsset(ll, "Assets/Resources/LevelList.asset"); }
        lsp.levelList = ll;
        if (ll.levels == null) ll.levels = new List<LevelDefinition>();
        if (ll.levels.Count == 0)
        {
            var lvl1 = ScriptableObject.CreateInstance<LevelDefinition>();
            lvl1.levelName = "Уровень 1";
            lvl1.sceneName = "SampleScene";
            AssetDatabase.CreateAsset(lvl1, "Assets/Resources/Level1.asset");
            ll.levels.Add(lvl1);
            EditorUtility.SetDirty(ll);
        }

        FixFonts(canvasGO);
        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveScene(scene);
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[BuildMainMenu] Done. Canvas=" + canvasGO.name);
    }

    static void FixFonts(GameObject root)
    {
        var arial = (Font)Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf");
        if (arial == null) return;
        foreach (var tx in root.GetComponentsInChildren<Text>(true))
            if (tx.font == null) tx.font = arial;
    }
}
#endif
