using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("PlayerPrefs key. The tutorial only shows while this key is not set to 1.")]
    public string tutorialKey = "TutorialShown";

    [Tooltip("Guide texture shown to the player. Falls back to Resources/Guide if left empty.")]
    public Texture2D guideTexture;

    [Tooltip("Label of the skip button (top-right).")]
    public string skipButtonText = "Пропустить";

    private GameObject overlay;

    void Awake()
    {
        if (PlayerPrefs.GetInt(tutorialKey, 0) == 1)
        {
            Destroy(gameObject);
            return;
        }

        BuildOverlay();
    }

    void BuildOverlay()
    {
        Texture2D tex = guideTexture;
        if (tex == null) tex = Resources.Load<Texture2D>("Guide");

        if (tex == null)
        {
            Debug.LogWarning("TutorialManager: Guide texture not found. Skipping tutorial.");
            Destroy(gameObject);
            return;
        }

        overlay = new GameObject("TutorialOverlay");

        Canvas canvas = overlay.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = overlay.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        overlay.AddComponent<GraphicRaycaster>();

        // Dim background so the guide stands out and blocks clicks to the game.
        GameObject dim = new GameObject("Dim");
        dim.transform.SetParent(overlay.transform, false);
        Image dimImg = dim.AddComponent<Image>();
        dimImg.color = new Color(0f, 0f, 0f, 0.85f);
        dimImg.raycastTarget = true;
        RectTransform dimRT = dim.GetComponent<RectTransform>();
        dimRT.anchorMin = Vector2.zero;
        dimRT.anchorMax = Vector2.one;
        dimRT.offsetMin = Vector2.zero;
        dimRT.offsetMax = Vector2.zero;

        // Guide image, kept at its native aspect ratio.
        GameObject guide = new GameObject("Guide");
        guide.transform.SetParent(overlay.transform, false);
        RawImage raw = guide.AddComponent<RawImage>();
        raw.texture = tex;
        raw.raycastTarget = false;
        AspectRatioFitter fit = guide.AddComponent<AspectRatioFitter>();
        fit.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fit.aspectRatio = (float)tex.width / tex.height;
        RectTransform guideRT = guide.GetComponent<RectTransform>();
        guideRT.anchorMin = Vector2.zero;
        guideRT.anchorMax = Vector2.one;
        guideRT.offsetMin = Vector2.zero;
        guideRT.offsetMax = Vector2.zero;

        // Skip button, anchored to the top-right corner.
        GameObject skip = new GameObject("SkipButton");
        skip.transform.SetParent(overlay.transform, false);
        Image btnImg = skip.AddComponent<Image>();
        btnImg.color = new Color(0.85f, 0.3f, 0.3f, 1f);
        Button btn = skip.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        RectTransform skipRT = skip.GetComponent<RectTransform>();
        skipRT.anchorMin = new Vector2(1f, 1f);
        skipRT.anchorMax = new Vector2(1f, 1f);
        skipRT.pivot = new Vector2(1f, 1f);
        skipRT.anchoredPosition = new Vector2(-30f, -30f);
        skipRT.sizeDelta = new Vector2(180f, 56f);

        GameObject label = new GameObject("Text");
        label.transform.SetParent(skip.transform, false);
        Text t = label.AddComponent<Text>();
        t.text = skipButtonText;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.fontSize = 24;
        t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf");
        RectTransform labelRT = label.GetComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero;
        labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = Vector2.zero;
        labelRT.offsetMax = Vector2.zero;

        btn.onClick.AddListener(HideTutorial);
    }

    void HideTutorial()
    {
        PlayerPrefs.SetInt(tutorialKey, 1);
        PlayerPrefs.Save();

        if (overlay != null) Destroy(overlay);
        Destroy(gameObject);
    }
}
