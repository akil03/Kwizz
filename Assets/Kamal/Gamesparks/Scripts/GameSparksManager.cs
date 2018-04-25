using Facebook.Unity;
using GameSparks.Api.Requests;
using GameSparks.Core;
using UnityEngine;

[RequireComponent(typeof(GameSparksUnity))]
public class GameSparksManager : MonoBehaviour
{
    public bool debugLog, autoLogin;
    public string Name, Pwd;
    public static StringDelegate FBConnectSuccess;
    public static StringDelegate NewUser;
    public static ParameterlessDelegate authenticated;
    public static GameSparksManager instance;
    bool isloggedIn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        FacebookManager.LoginSuccess += FBLogin;
        GS.GameSparksAuthenticated += delegate (string str)
        {
            if (!isloggedIn)
            {
                isloggedIn = true;
                if (authenticated != null)
                {
                    authenticated();
                }
            }
        };
        if (autoLogin)
        {
            Login(Name, Pwd);
        }
    }

    public void Login(string username, string password)
    {
        new AuthenticationRequest().SetUserName(username).SetPassword(password).Send(callback =>
        {
            if (!callback.HasErrors)
            {
                if (debugLog)
                {
                    print("Login Success!");
                }
            }
            else
            {
                print(callback.Errors.JSON);
            }
        });
    }

    private void FBLogin()
    {
        new FacebookConnectRequest().SetAccessToken(AccessToken.CurrentAccessToken.TokenString).SetErrorOnSwitch(true).SetDoNotLinkToCurrentPlayer(true).Send(callback =>
          {
              if (!callback.HasErrors)
              {
                  if (debugLog)
                  {
                      print("FB login success.");
                      print(callback.JSONString);
                  }
                  GSFBLoginResult fbLoginResult = JsonUtility.FromJson<GSFBLoginResult>(callback.JSONString);
                  if (fbLoginResult.newPlayer)
                  {
                      if (debugLog)
                      {
                          print("New user event is fired!");
                      }
                      if (NewUser != null)
                      {
                          foreach (var item in callback.ScriptData.BaseData)
                          {
                              NewUser(((GSData)item.Value).JSON);
                          }
                      }
                  }
                  if (FBConnectSuccess != null)
                  {
                      FBConnectSuccess(callback.JSONString);
                  }
              }
              else
              {
                  print(callback.Errors.JSON);
              }
          });
    }

    public void LogOut()
    {
        isloggedIn = false;
        GS.Reset();
    }
}