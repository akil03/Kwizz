using System.Linq;
using Facebook.Unity;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Core;
using UnityEngine;

[RequireComponent(typeof(GameSparksUnity))]
public class GameSparksManager : Singleton<GameSparksManager>
{
    public bool debugLog, autoLogin;
    public string Name, Pwd;
    public StringDelegate FBConnectSuccess;
    public StringDelegate NewUser;
    public StringDelegate RegistrationSuccess;
    public ParameterlessDelegate authenticated;
    public ParameterlessDelegate loggedOut;
    public LeaderboardData leaderboardData;
    public LeaderboardEntry leaderboardEntry;
    public LeaderboardDataDelegate gotLeaderboard;
    public LeaderboardDataDelegate gotPlayerInLeaderoard;
    bool isloggedIn;

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
        if (autoLogin)
        {
            Login(Name, Pwd);
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

    public void Register(string displayName, string userName, string password)
    {
        new RegistrationRequest().SetDisplayName(displayName).SetUserName(userName).SetPassword(password).Send(response =>
        {
            if (!response.HasErrors)
            {
                isloggedIn = false;
                if (RegistrationSuccess != null)
                {
                    RegistrationSuccess(response.JSONString);
                }
            }
            else
            {
                print(response.Errors.JSON);
            }
        });
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
        new LeaderboardsEntriesRequest().SetLeaderboards(new System.Collections.Generic.List<string> { "score" }).Send(reposnse =>
         {
             if (!reposnse.HasErrors)
             {
                 leaderboardEntry = JsonUtility.FromJson<LeaderboardEntry>(reposnse.JSONString);
                 gotPlayerInLeaderoard(leaderboardEntry.score);
                 new LeaderboardDataRequest().SetLeaderboardShortCode("score").SetEntryCount(10).Send(response =>
                 {
                     if (!response.HasErrors)
                     {
                         leaderboardData = JsonUtility.FromJson<LeaderboardData>(response.JSONString);
                         try
                         {
                             var player = leaderboardData.data.First(a => a.userId == id);
                             leaderboardData.data.Remove(player);
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