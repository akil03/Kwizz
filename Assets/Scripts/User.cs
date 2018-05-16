using System;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public InputField userName, pwd, phoneNumber, code;
    public GameObject loginPage, savePhoneNumberPage, appStartPage, enterCodePage, questionPage;
    public QuestionsPage questionsPage;
    public static User instance;
    public bool dontCheckPhoneNumber;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnLogout()
    {
        loginPage.SetActive(true);
        enterCodePage.SetActive(false);
    }

    private void Start()
    {
        GameSparksManager.Instance.authenticated += OnGameSparksLogin;
        GameSparksManager.Instance.loggedOut += OnLogout;
    }

    private void OnDisable()
    {
        GameSparksManager.Instance.authenticated -= OnGameSparksLogin;
        GameSparksManager.Instance.loggedOut -= OnLogout;
    }

    private void OnGameSparksLogin()
    {
        new AccountDetailsRequest().Send(AccountDetailsCallback);
    }

    private void AccountDetailsCallback(AccountDetailsResponse obj)
    {
        AccountDetails details = JsonUtility.FromJson<AccountDetails>(obj.JSONString);
        if (details.scriptData.result == "add number" && !dontCheckPhoneNumber)
        {
            loginPage.SetActive(false);
            savePhoneNumberPage.SetActive(true);
        }
        else
        {
            OpenGame();
        }
        dontCheckPhoneNumber = false;
    }

    public void OpenGame()
    {
        loginPage.SetActive(false);
        appStartPage.SetActive(true);
        questionPage.SetActive(false);
        enterCodePage.SetActive(false);
        code.text = string.Empty;
        if (Application.isEditor)
        {
            TestHelper.Instance.SetCode();
        }
    }

    public void UserNameLogin()
    {
        GameSparksManager.Instance.Login(userName.text, pwd.text);
    }

    public void SavePhoneNumber()
    {
        SavePhoneNumber(phoneNumber.text);
    }

    public void SavePhoneNumber(string number)
    {
        UserData userData = new UserData();
        userData.phone = phoneNumber.text;
        new GameSparkRequests("SaveUserData").Add("phone", number).Request(SavePhoneNumberCallback);
    }

    private void SavePhoneNumberCallback(string str)
    {
        if (!str.Contains("Error"))
        {
            savePhoneNumberPage.SetActive(false);
            appStartPage.SetActive(true);
            print("phone number saved!");
        }
        else
        {
            print("Save failed!");
        }
    }

    public void GetQuestions()
    {
        new GameSparkRequests("GetQuestion").Add("code", code.text).Add("date", DateTime.Today.AddDays(ErrorCodes.daysOffset).ToString("dd/MM/yyyy")).Request(GetQuestionsSuccessCallback, GetQuestionsFailedCallback);
    }

    private void GetQuestionsFailedCallback(string str)
    {
        GSError result = JsonUtility.FromJson<GSError>(str);
        if (result.error.Status == "already answered")
        {
            Popup.Instance.DisplayMessage("Already answered. Wait for next question.");
        }
        if (result.error.Status == ErrorCodes.noteligible)
        {
            Popup.Instance.DisplayMessage("Contest already halfway. Please try again tomorrow.");
        }
        if (result.error.Status == "already lost")
        {
            Popup.Instance.DisplayMessage("Already lost. Please try again tomorrow.");
        }
        if (result.error.Status == ErrorCodes.close)
        {
            Popup.Instance.DisplayMessage("Question is already closed!");
        }
        if (result.error.Status == ErrorCodes.failed)
        {
            Popup.Instance.DisplayMessage("Incorrect code. Please check again.");
        }
    }

    private void GetQuestionsSuccessCallback(string str)
    {
        enterCodePage.SetActive(false);
        questionPage.SetActive(true);
        questionsPage.Setup(JsonUtility.FromJson<Question>(JsonUtility.FromJson<GSResult>(str).scriptData.result));
    }

    public void Logout()
    {
        loginPage.SetActive(true);
        questionPage.SetActive(false);
    }
}