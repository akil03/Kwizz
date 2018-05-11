using System;
using System.Collections.Generic;
using System.Linq;
using GameSparks.Core;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsManager : MonoBehaviour
{
    public static QuestionsManager instance;
    public InputField date;
    public Dropdown questionType;
    public List<QuestionElement> questionElementInstances;
    public QuestionElement[] questionPrefabs;
    public Transform questionsContainer;
    public List<Question> questions;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        GS.GameSparksAuthenticated += OnGameSparksLogin;
        if (date != null)
        {
            date.onEndEdit.AddListener(DateChanged);
        }
        EventManager.Instance.OnQuestionClose += CloseQuestion;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnQuestionClose -= CloseQuestion;
    }

    private void OnGameSparksLogin(string obj)
    {
        if (date != null)
        {
            date.text = DateTime.Today.ToString("dd/MM/yyyy");
            DateChanged(date.text);
        }
    }

    private void DateChanged(string arg0)
    {
        GetQuestions();
    }

    private void GetQuestions()
    {
        new GameSparkRequests("GetAllQuestions").Add("date", date.text).Request(GotQuestionsCallback);
    }

    private void GotQuestionsCallback(string str)
    {
        ResetQuestions();
        GSListResult result = JsonUtility.FromJson<GSListResult>(str);
        foreach (var item in result.scriptData.result)
        {
            Question qn = JsonUtility.FromJson<Question>(item);
            QuestionElement instance = CreateQuestion((int)qn.type);
            instance.Setup(qn);
            questions.Add(qn);
        }
    }

    public void CreateQuestion()
    {
        CreateQuestion(questionType.value);
    }

    private QuestionElement CreateQuestion(int type)
    {
        QuestionElement instance = Instantiate(questionPrefabs[type]);
        instance.transform.SetParent(questionsContainer);
        questionElementInstances.Add(instance);
        instance.GenerateCode();
        return instance;
    }

    public void RemoveQuestion(QuestionElement obj)
    {
        questionElementInstances.Remove(obj);
        Destroy(obj.gameObject);
    }

    public void SaveQuestions()
    {
        questions = questionElementInstances.Select(a => a.Question).ToList();
        foreach (var item in questions)
        {
            string json = JsonUtility.ToJson(item);
            new GameSparkRequests("SaveQuestion").Add("code", item.code).Add("data", json).Add("answer", item.correctIndex).Add("date", date.text).Request(SaveQuestionsSuccess);
        }
    }

    private void SaveQuestionsSuccess(string str)
    {
        Popup.Instance.DisplayMessage("Questions saved.");
    }

    public void ResetQuestions()
    {
        foreach (var item in questionElementInstances.ToList())
        {
            Destroy(item.gameObject);
        }
        questionElementInstances.Clear();
        questions.Clear();
    }

    public void CloseQuestion(Question question)
    {
        new GameSparkRequests("CloseQuestion").Add("code", question.code).Request(CloseClosedSuccessfully);
    }

    private void CloseClosedSuccessfully(string str)
    {
        GSResult result = JsonUtility.FromJson<GSResult>(str);
        if (result.scriptData.result == "open")
        {
            Popup.Instance.DisplayMessage("Question opened.");
        }
        else if (result.scriptData.result == "closed")
        {
            Popup.Instance.DisplayMessage("Question closed.");
        }
    }
}
