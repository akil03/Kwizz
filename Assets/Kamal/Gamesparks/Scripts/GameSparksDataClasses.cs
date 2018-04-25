using System;

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
public class ScriptData
{
    public string data;
}

[Serializable]
public class UserData
{
    public string phone;
}

[Serializable]
public class QuestionContainer
{
    public string date;
    public string data;
}

[Serializable]
public class QuestionScriptData
{
    public string question;
}

[Serializable]
public class QuestionResult
{
    public QuestionScriptData scriptData;
}