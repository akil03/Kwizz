using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    public bool debugLog;
    public event QuestionDelegate OnQuestionClose;

    public void CloseQuestion(Question question)
    {
        OnQuestionClose(question);
    }
}

public delegate void QuestionDelegate(Question question);
public delegate void StringDelegate(string str);
public delegate void ParameterlessDelegate();
public delegate void LeaderboardDataDelegate(List<Datum> list);
