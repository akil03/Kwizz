using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    public bool autoLogin, debugLog;
    public List<string> permissionsQuery = new List<string>() { "publish_actions" };
    public string detailsQuery = "me?fields=id,name", friendsQuery = "me/friends";

    public static ParameterlessDelegate InitComplete;
    public static ParameterlessDelegate LoginSuccess;
    public static StringDelegate GotUserDetails;
    public static StringDelegate GotFriends;
    public static FacebookManager instance;

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
        if (!FB.IsLoggedIn)
        {
            FB.LogInWithPublishPermissions(permissionsQuery, LoginCallback);
        }
        else
        {
            LoginSuccess();
        }
    }

    private void LoginCallback(ILoginResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
        {
            if (debugLog)
            {
                print("FB Login success event fired!");
            }
            if (LoginSuccess != null)
            {
                LoginSuccess();
            }
            FB.API(detailsQuery, HttpMethod.GET, GotUserDetailsCallback);
            FB.API(friendsQuery, HttpMethod.GET, GotFriendsCallback);
        }
        else
        {
            print("Login failed!");
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
            if (GotUserDetails != null)
            {
                GotUserDetails(result.RawResult);
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
