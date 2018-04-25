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