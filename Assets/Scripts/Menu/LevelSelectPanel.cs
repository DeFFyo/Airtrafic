using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : MonoBehaviour
{
    public Transform levelListContent;
    public Button backButton;
    public Text headerText;
    public MenuManager menuManager;
    public LevelList levelList;
    public MenuStrings strings;

    void Awake()
    {
        if (strings == null) strings = Resources.Load<MenuStrings>("MenuStrings");
        if (levelList == null) levelList = Resources.Load<LevelList>("LevelList");

        headerText.text = strings.levelSelectTitle;
        backButton.GetComponentInChildren<Text>().text = strings.back;
        backButton.onClick.AddListener(menuManager.ShowMain);

        BuildLevelList();
    }

    void OnEnable()
    {
        if (levelList == null) levelList = Resources.Load<LevelList>("LevelList");
        BuildLevelList();
    }

    void BuildLevelList()
    {
        if (levelList == null) levelList = Resources.Load<LevelList>("LevelList");
        if (levelList == null) return;

        var toDestroy = new System.Collections.Generic.List<Transform>();
        foreach (Transform child in levelListContent)
            toDestroy.Add(child);
        foreach (var child in toDestroy)
            Object.Destroy(child.gameObject);

        foreach (var level in levelList.levels)
        {
            if (level == null) continue;

            GameObject btn = new GameObject(level.levelName, typeof(RectTransform), typeof(Button), typeof(Image));
            btn.transform.SetParent(levelListContent, false);
            var brt = btn.GetComponent<RectTransform>(); brt.sizeDelta = new Vector2(600, 80);
            btn.AddComponent<LayoutElement>().preferredHeight = 80;
            btn.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f);

            if (level.icon != null)
                btn.GetComponent<Image>().sprite = level.icon;

            GameObject label = new GameObject("Label", typeof(RectTransform), typeof(Text));
            label.transform.SetParent(btn.transform, false);
            Text t = label.GetComponent<Text>();
            t.text = level.levelName;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.fontSize = 28;
            if (t.font == null) t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf");
            RectTransform rt = label.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            string scene = level.sceneName;
            btn.GetComponent<Button>().onClick.AddListener(
                () => UnityEngine.SceneManagement.SceneManager.LoadScene(scene));
        }
    }
}
