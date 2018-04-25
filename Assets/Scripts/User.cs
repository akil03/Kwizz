using System;
using System.Linq;
using GameSparks.Core;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public InputField userName, pwd, phoneNumber, code;
    public GameObject loginPage, savePhoneNumberPage, appStartPage, enterCodePage, questionPage;
    string id;
    public Questions questions;
    public QuestionsPage questionsPage;
    public static User instance;
    public bool isLoser;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        GS.GameSparksAuthenticated += OnGameSparksLogin;
    }

    private void OnGameSparksLogin(string obj)
    {
        id = obj;
        loginPage.SetActive(false);
        CheckPhoneNumber();
        GetQuestions();
        CheckIsLoser();
    }

    private void CheckIsLoser()
    {
        GameSparkRequests request = new GameSparkRequests();
        request.Request("IsLoser", IsLoserCallback);
    }

    private void CheckPhoneNumber()
    {
        GameSparkRequests request = new GameSparkRequests();
        request.Request("GetUserData", GetUserDataCallback);
    }

    private void IsLoserCallback(string str)
    {
        if (str.Contains("true"))
        {
            isLoser = true;
        }
        else
        {
            print("proceed man!");
        }
    }

    public void OpenGame()
    {
        if (!isLoser)
        {
            appStartPage.SetActive(false);
            enterCodePage.SetActive(true);
        }
        else
        {
            print("you fucking lost already man.");
        }
    }

    private void GetUserDataCallback(string str)
    {
        if (!str.Contains("Error"))
        {
            appStartPage.SetActive(true);
        }
        else
        {
            savePhoneNumberPage.SetActive(true);
        }
    }

    public void UserNameLogin()
    {
        GameSparksManager.instance.Login(userName.text, pwd.text);
    }

    public void SavePhoneNumber()
    {
        UserData userData = new UserData();
        userData.phone = phoneNumber.text;
        GameSparkRequests request = new GameSparkRequests();
        request.Request("SaveUserData", SavePhoneNumberCallback);
    }

    private void SavePhoneNumberCallback(string str)
    {
        if (!str.Contains("Error"))
        {
            print("Save success!");
            appStartPage.SetActive(true);
        }
        else
        {
            print("Save failed!");
        }
    }

    private void GetQuestions()
    {
        GameSparkRequests requests = new GameSparkRequests();
        requests.Request("GetQuestion", "date", DateTime.Today.ToString("dd/MM/yyyy"), GotQuestionsCallback);
    }

    private void GotQuestionsCallback(string str)
    {
        string json = JsonUtility.FromJson<QuestionResult>(str).scriptData.question;
        if (json != null)
        {
            questions = JsonUtility.FromJson<Questions>(json);
        }
    }

    public void GetAQuestion()
    {
        try
        {
            var result = questions.questionList.First(a => a.code == code.text);
            enterCodePage.SetActive(false);
            questionPage.SetActive(true);
            questionsPage.Setup(result);
        }
        catch (Exception)
        {
            print("wrong fucking code moron!");

        }
    }

    public void Logout()
    {
        loginPage.SetActive(true);
        questionPage.SetActive(false);
    }
}