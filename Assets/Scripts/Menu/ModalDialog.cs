using UnityEngine;
using UnityEngine.UI;
using System;

public class ModalDialog : MonoBehaviour
{
    public Text titleText;
    public Text messageText;
    public Button yesButton;
    public Button noButton;
    public MenuStrings strings;

    private Action onYes;

    void Awake()
    {
        if (strings == null) strings = Resources.Load<MenuStrings>("MenuStrings");
        gameObject.SetActive(false);
        noButton.onClick.AddListener(Hide);
    }

    public void Show(string title, string message, Action yesCallback)
    {
        titleText.text = title;
        messageText.text = message;
        yesButton.GetComponentInChildren<Text>().text = strings.yes;
        noButton.GetComponentInChildren<Text>().text = strings.no;

        onYes = yesCallback;
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() =>
        {
            onYes?.Invoke();
            Hide();
        });

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
