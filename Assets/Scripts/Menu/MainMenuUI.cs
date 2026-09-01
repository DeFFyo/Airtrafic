using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Button exitButton;
    public Text titleText;
    public MenuManager menuManager;
    public ModalDialog modalDialog;
    public MenuStrings strings;

    void Awake()
    {
        if (strings == null) strings = Resources.Load<MenuStrings>("MenuStrings");

        titleText.text = strings.mainTitle;
        startButton.GetComponentInChildren<Text>().text = strings.start;
        settingsButton.GetComponentInChildren<Text>().text = strings.settings;
        exitButton.GetComponentInChildren<Text>().text = strings.exit;

        startButton.onClick.AddListener(menuManager.ShowLevelSelect);
        settingsButton.onClick.AddListener(menuManager.ShowSettings);
        exitButton.onClick.AddListener(() =>
        {
            modalDialog.Show(strings.dialogExitTitle, strings.dialogExitMessage, () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        });
    }
}
