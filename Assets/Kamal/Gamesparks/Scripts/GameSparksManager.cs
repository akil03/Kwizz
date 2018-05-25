using System.Linq;
using Facebook.Unity;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Core;
using UnityEngine;

[RequireComponent(typeof(GameSparksUnity))]
public class GameSparksManager : Singleton<GameSparksManager>
{
    public bool debugLog;
    public StringDelegate FBConnectSuccess;
    public StringDelegate NewUser;
    public StringDelegate RegistrationSuccess;
    public ParameterlessDelegate authenticated;
    public ParameterlessDelegate loggedOut;
    public LeaderboardData leaderboardData;
    public LeaderboardEntry leaderboardEntry;
    public LeaderboardDataDelegate gotLeaderboard;
    public LeaderboardDataDelegate gotPlayerInLeaderoard;
    public bool isloggedIn;
    public GameObject loginPage, loading;

    private void Start()
    {
        FacebookManager.LoginSuccess += FBLogin;
        GameSparks.Api.Messages.NewHighScoreMessage.Listener += newHg;
        GoogleManager.Instance.loginSuccess += GoogleLogin;
        GS.GameSparksAuthenticated += delegate (string str)
        {
            if (!isloggedIn)
            {
                isloggedIn = true;
                if (authenticated != null)
                {
                    authenticated();
                    GetLeaderboard(str);
                }
            }
        };
        StartCoroutine(WaitForLogin());
    }

    System.Collections.IEnumerator WaitForLogin()
    {
        int i = 0;
        while (!GS.Authenticated && i < 3)
        {
            yield return new WaitForSeconds(1);
            i++;
        }
        if (!GS.Authenticated)
        {
            GS.Reset();
            loginPage.SetActive(true);
            loading.SetActive(false);
        }
    }

    private void newHg(NewHighScoreMessage obj)
    {
        print("New score");
    }


    void GoogleLogin()
    {
        new AuthenticationRequest().SetUserName(LCGoogleLoginBridge.GSIEmail()).SetPassword(LCGoogleLoginBridge.GSIEmail()).Send(callback =>
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
                new RegistrationRequest().SetDisplayName(LCGoogleLoginBridge.GSIUserName()).SetUserName(LCGoogleLoginBridge.GSIEmail()).SetPassword(LCGoogleLoginBridge.GSIEmail()).Send(response =>
                {
                    if (!response.HasErrors)
                    {
                        isloggedIn = false;
                        Login(LCGoogleLoginBridge.GSIEmail(), LCGoogleLoginBridge.GSIEmail());
                    }
                    else
                    {
                        print(response.Errors.JSON);
                    }
                });
            }
        });
    }

    public void Register(string displayName, string userName, string password, string phone)
    {
        new RegistrationRequest().SetDisplayName(displayName).SetUserName(userName).SetPassword(password).Send(response =>
        {
            if (!response.HasErrors)
            {
                isloggedIn = false;
                User.instance.dontCheckPhoneNumber = true;
                User.instance.phoneNo = phone;
                Login(userName, password);
                if (RegistrationSuccess != null)
                {
                    RegistrationSuccess("");
                }
            }
            else
            {
                print(response.Errors.JSON);
                if (response.Errors.JSON.Contains("TAKEN"))
                {
                    Popup.Instance.DisplayMessage("Email ID already registred!");
                }
            }
        });
    }

    public void Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Popup.Instance.DisplayMessage("Email and password cannot be empty.");
            return;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Popup.Instance.DisplayMessage("Check your internet connection.");
            return;
        }
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
                if (callback.Errors.JSON.Contains("UNRECOGNISED"))
                {
                    Popup.Instance.DisplayMessage("The login does not exist.");
                    return;
                }
                if (callback.Errors.JSON.Contains("LOCKED"))
                {
                    Popup.Instance.DisplayMessage("Account locked. Try again aftre sometime.");
                    return;
                }
                var error = JsonUtility.FromJson<GameSparksLoginFailed>(callback.Errors.JSON);
                Popup.Instance.DisplayMessage(error.DETAILS);
            }
        });
    }

    private void FBLogin()
    {
        new AuthenticationRequest().SetUserName(AccessToken.CurrentAccessToken.UserId).SetPassword(AccessToken.CurrentAccessToken.UserId).Send(callback =>
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
                new RegistrationRequest().SetDisplayName(FacebookManager.instance.userDetails.name).SetUserName(AccessToken.CurrentAccessToken.UserId).SetPassword(AccessToken.CurrentAccessToken.UserId).Send(response =>
                {
                    if (!response.HasErrors)
                    {
                        isloggedIn = false;
                        Login(AccessToken.CurrentAccessToken.UserId, AccessToken.CurrentAccessToken.UserId);
                    }
                    else
                    {
                        print(response.Errors.JSON);
                    }
                });
            }
        });
    }

    public void GetLeaderboard(string id)
    {
        new LeaderboardsEntriesRequest().SetLeaderboards(new System.Collections.Generic.List<string> { "money" }).Send(reposnse =>
         {
             if (!reposnse.HasErrors)
             {
                 leaderboardEntry = JsonUtility.FromJson<LeaderboardEntry>(reposnse.JSONString);
                 gotPlayerInLeaderoard(leaderboardEntry.score);
                 new LeaderboardDataRequest().SetLeaderboardShortCode("money").SetEntryCount(10).Send(response =>
                 {
                     if (!response.HasErrors)
                     {
                         leaderboardData = JsonUtility.FromJson<LeaderboardData>(response.JSONString);
                         try
                         {
                             var player = leaderboardData.data.First(a => a.userId == id);
                             //leaderboardData.data.Remove(player);
                         }
                         catch (System.Exception)
                         {

                             print("Player not in top 10.");
                         }
                         gotLeaderboard(leaderboardData.data);
                     }
                 });
             }
         });
    }

    [ContextMenu("Logout")]
    public void LogOut()
    {
        isloggedIn = false;
        GS.Reset();
        loggedOut();
    }
}