using System;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public InputField userName, pwd, phoneNumber, code;
    public GameObject loginPage, savePhoneNumberPage, appStartPage, enterCodePage, questionPage, profilePage;
    public QuestionsPage questionsPage;
    public static User instance;
    public bool dontCheckPhoneNumber;
    public InputField userNameText, phone;
    public Text amount;
    public AccountDetails accountDetails;


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
        accountDetails = JsonUtility.FromJson<AccountDetails>(obj.JSONString);
        if (string.IsNullOrEmpty(accountDetails.scriptData.result.phone))
        {
            if (!dontCheckPhoneNumber)
            {
                loginPage.SetActive(false);
                savePhoneNumberPage.SetActive(true);
                return;
            }
        }
        else
        {
            SetProfileUI();
            OpenGame();
        }
        dontCheckPhoneNumber = false;
    }

    private void SetProfileUI()
    {
        userNameText.text = accountDetails.displayName;
        phone.text = accountDetails.scriptData.result.phone;
        amount.text = accountDetails.scriptData.result.amount.ToString();
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
        try
        {
            long.Parse(phoneNumber.text);
            if (phoneNumber.text.Length < 10 || phoneNumber.text.Length > 12)
            {
                Popup.Instance.DisplayMessage("Please enter a valid phone number!");
                return;
            }
        }
        catch
        {
            Popup.Instance.DisplayMessage("Please enter a valid phone number!");
            return;
        }
        SavePhoneNumber(phoneNumber.text);
    }

    public void SavePhoneNumber(string number)
    {
        if (accountDetails.scriptData.result == null)
        {
            accountDetails.scriptData.result = new UserData();
        }
        accountDetails.scriptData.result.phone = phoneNumber.text;
        new GameSparkRequests("SaveUserData").Add("phone", number).Request(SavePhoneNumberCallback);
    }

    public void UpdateUserdata()
    {
        new GameSparkRequests("UpdateUserdata").Add("phone", phone.text).Add("amount", int.Parse(amount.text)).Request(UpdateUserDataCallback);
        new ChangeUserDetailsRequest().SetDisplayName(userNameText.text).Send(response => { });
    }

    public void CancelUserDataUpdate()
    {
        SetProfileUI();
    }

    private void UpdateUserDataCallback(string str)
    {
        profilePage.SetActive(false);
        appStartPage.SetActive(true);
        accountDetails.scriptData.result.amount = int.Parse(amount.text);
        accountDetails.displayName = userNameText.text;
        accountDetails.scriptData.result.phone = phone.text;
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
        if (str.Contains("winner"))
        {
            Popup.Instance.DisplayMessage("You won the game today.");
            return;
        }
        enterCodePage.SetActive(false);
        questionPage.SetActive(true);
        questionsPage.Setup(JsonUtility.FromJson<Question>(JsonUtility.FromJson<GSResult>(str).scriptData.result));
    }

    public void Logout()
    {
        loginPage.SetActive(true);
        questionPage.SetActive(false);
        accountDetails = new AccountDetails();
    }
}