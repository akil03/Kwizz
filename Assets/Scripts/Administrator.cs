using DoozyUI;
using GameSparks.Core;
using UnityEngine;
using UnityEngine.UI;

public class Administrator : MonoBehaviour
{
    public InputField userName, pwd;
    public UIElement loginPage, addQuestionsPage;
    public static Administrator instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (!Application.isEditor)
        {
            GS.Reset();
        }
        else
        {
            print("Not resetting login coz we are using editor.");
        }
    }

    private void Start()
    {
        GS.GameSparksAuthenticated += OnGameSparksLogin;
    }

    private void OnGameSparksLogin(string obj)
    {
        userName.text = string.Empty;
        pwd.text = string.Empty;
        loginPage.gameObject.SetActive(false);
        addQuestionsPage.gameObject.SetActive(true);
    }

    public void Login()
    {
        GameSparksManager.Instance.Login(userName.text, pwd.text);
    }
}
