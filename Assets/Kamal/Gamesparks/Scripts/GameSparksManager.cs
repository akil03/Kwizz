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

    public void Register(string displayName, string userName, string password)
    {
        new RegistrationRequest().SetDisplayName(displayName).SetUserName(userName).SetPassword(password).Send(response =>
        {
            if (!response.HasErrors)
            {
                isloggedIn = false;
                RegistrationSuccess(response.JSONString);
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
        new DeviceAuthenticationRequest().Send(response =>
        {
            if (!response.HasErrors)
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