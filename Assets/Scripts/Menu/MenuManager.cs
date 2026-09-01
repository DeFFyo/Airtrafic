using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject levelSelectPanel;
    public ModalDialog modalDialog;
    public MenuStrings strings;

    void Awake()
    {
        if (strings == null)
            strings = Resources.Load<MenuStrings>("MenuStrings");
        ShowMain();
    }

    public void ShowMain()
    {
        SetActivePanels(mainPanel);
    }

    public void ShowSettings()
    {
        SetActivePanels(settingsPanel);
    }

    public void ShowLevelSelect()
    {
        SetActivePanels(levelSelectPanel);
    }

    private void SetActivePanels(GameObject active)
    {
        if (mainPanel) mainPanel.SetActive(active == mainPanel);
        if (settingsPanel) settingsPanel.SetActive(active == settingsPanel);
        if (levelSelectPanel) levelSelectPanel.SetActive(active == levelSelectPanel);
    }
}
