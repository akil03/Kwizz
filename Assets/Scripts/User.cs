using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public InputField userName, pwd, phoneNumber, code;
    public GameObject loginPage, savePhoneNumberPage, appStartPage, enterCodePage, questionPage;
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
        GameSparksManager.authenticated += OnGameSparksLogin;
    }

    private void OnGameSparksLogin()
    {
        loginPage.SetActive(false);
        CheckPhoneNumber();
    }

    private void CheckPhoneNumber()
    {
        GameSparkRequests request = new GameSparkRequests();
        request.Request("GetUserData", GetUserDataCallback);
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
        print(str);
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

    public void GetQuestions()
    {
        GameSparkRequests requests = new GameSparkRequests();
        requests.Request("GetQuestion", "code", code.text, GetQuestionsSuccessCallback, GetQuestionsFailedCallback);
    }

    private void GetQuestionsFailedCallback(string str)
    {
        GSError result = JsonUtility.FromJson<GSError>(str);
        if (result.error.Status == ErrorCodes.noteligible)
        {
            print("Not eligible to get this question!");
        }
        if (result.error.Status == ErrorCodes.close)
        {
            print("question is already closed!");
        }
    }

    private void GetQuestionsSuccessCallback(string str)
    {
        print(str);
    }

    public void Logout()
    {
        loginPage.SetActive(true);
        questionPage.SetActive(false);
    }
}