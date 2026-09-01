using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool IsPaused = false;

    public GameObject gameOverPanel;
    public Text reasonText;
    public Button btnRestart;
    public Button btnMenu;
    public Button btnQuit;

    void Awake()
    {
        Instance = this;
        IsPaused = false;
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(false);
            if (btnRestart) btnRestart.onClick.AddListener(Restart);
            if (btnMenu) btnMenu.onClick.AddListener(MainMenu);
            if (btnQuit) btnQuit.onClick.AddListener(Quit);
        }
    }

    public void GameOver(string reason)
    {
        if (IsPaused) return;
        IsPaused = true;
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            if (reasonText) reasonText.text = "GAME OVER\n" + reason;
        }
    }

    public void Restart() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void MainMenu() { SceneManager.LoadScene("MainMenu"); }
    public void Quit() { Application.Quit(); }
}
