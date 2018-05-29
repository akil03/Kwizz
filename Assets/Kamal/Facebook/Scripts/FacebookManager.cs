using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    public bool autoLogin, debugLog;
    public List<string> permissionsQuery = new List<string>() { "public_profile" };
    public string detailsQuery = "me?fields=id,name", friendsQuery = "me/friends";

    public static ParameterlessDelegate InitComplete;
    public static ParameterlessDelegate LoginSuccess;
    public static StringDelegate GotUserDetails;
    public static StringDelegate GotFriends;
    public static FacebookManager instance;
    public FacebookUserDetailsResult userDetails;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (autoLogin)
        {
            InitComplete += Login;
        }
        FB.Init(OnInitComplete);
    }

    private void OnInitComplete()
    {
        if (debugLog)
        {
            print("Facebook init complete!");
        }
        if (InitComplete != null)
        {
            InitComplete();
        }
    }

    public void Login()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Popup.Instance.DisplayMessage("Check your internet connection.");
            return;
        }
        Loading.Instance.StartLoading();
        FB.LogInWithReadPermissions(permissionsQuery, LoginCallback);
    }

    private void LoginCallback(ILoginResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
        {
            if (debugLog)
            {
                print("FB Login success event fired!");
            }
            FB.API(detailsQuery, HttpMethod.GET, GotUserDetailsCallback);
            FB.API(friendsQuery, HttpMethod.GET, GotFriendsCallback);
        }
        else
        {
            print("Login failed!");
            Loading.Instance.StopLoading();
            GameSparksManager.Instance.LoginFailed();
        }
    }

    private void GotUserDetailsCallback(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
        {
            if (debugLog)
            {
                print("Got user details event fired!");
            }
            userDetails = JsonUtility.FromJson<FacebookUserDetailsResult>(result.RawResult);
            if (GotUserDetails != null)
            {
                GotUserDetails(result.RawResult);
            }
            if (LoginSuccess != null)
            {
                LoginSuccess();
            }
        }
        else
        {
            print("Get user details failed!");
        }
    }

    private void GotFriendsCallback(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
        {
            if (debugLog)
            {
                print("Got friends event fired!");
            }
            if (GotFriends != null)
            {
                GotFriends(result.RawResult);
            }
        }
        else
        {
            print("Get friends failed!");
        }
    }
}
