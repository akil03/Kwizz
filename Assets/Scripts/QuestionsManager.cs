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
    public Questions questions;

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
        requests.Request("GetQuestion", "date", date.text, GotQuestionsCallback);
    }

    private void GotQuestionsCallback(string str)
    {
        ResetQuestions();
        string json = JsonUtility.FromJson<QuestionResult>(str).scriptData.question;
        if (json != null)
        {
            questions = JsonUtility.FromJson<Questions>(json);
            foreach (var item in questions.questionList)
            {
                QuestionElement instance = CreateQuestion((int)item.type);
                instance.Setup(item);
            }
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
        questions.questionList = questionElementInstances.Select(a => a.Question).ToList();
        string json = JsonUtility.ToJson(questions);
        GameSparkRequests requests = new GameSparkRequests();
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("date", date.text);
        dictionary.Add("data", json);
        requests.Request("SaveQuestion", dictionary, SaveQuestionsSuccess);
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
        questions = new Questions();
    }
}
