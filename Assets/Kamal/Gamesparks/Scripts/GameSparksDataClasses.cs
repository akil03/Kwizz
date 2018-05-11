using System;
using System.Collections.Generic;

[Serializable]
public class GSFBLoginResult
{
    public string authToken;
    public string displayName;
    public bool newPlayer;
    public string requestId;
    public string userId;
}

[Serializable]
public class Error
{
    public string Status;
}

[Serializable]
public class GSError
{
    public Error error;
}

[Serializable]
public class Result
{
    public string result;
}

[Serializable]
public class GSResult
{
    public Result scriptData;
}

[Serializable]
public class UserData
{
    public string phone;
}

[Serializable]
public class ResultList
{
    public List<string> result;
}

[Serializable]
public class GSListResult
{
    public ResultList scriptData;
}
[Serializable]
public class ExternalIds
{

}
[Serializable]
public class AccountDetailsScriptData
{
    public string result;
}

[Serializable]
public class AccountDetails
{
    public string displayName;
    public ExternalIds externalIds;
    public AccountDetailsScriptData scriptData;
    public string userId;
}

[Serializable]
public class Datum
{
    public string userId;
    public int score;
    public string when;
    public string city;
    public string country;
    public string userName;
    public ExternalIds externalIds;
    public int rank;
}

[Serializable]
public class LeaderboardData
{
    public List<Datum> data;
    public string leaderboardShortCode;
    public string requestId;
}

[Serializable]
public class LeaderboardEntry
{
    public string requestId;
    public List<Datum> score;
}