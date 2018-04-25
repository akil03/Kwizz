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
        GameSparkRequests requests = new GameSparkRequests();
        requests.Request("GetAllQuestions", "date", date.text, GotQuestionsCallback);
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
            GameSparkRequests requests = new GameSparkRequests();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("code", item.code);
            dictionary.Add("data", json);
            dictionary.Add("answer", item.correctIndex);
            dictionary.Add("date", date.text);
            requests.Request("SaveQuestion", dictionary, SaveQuestionsSuccess);
        }
    }

    private void SaveQuestionsSuccess(string str)
    {
        print("Save Success!");
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
}
