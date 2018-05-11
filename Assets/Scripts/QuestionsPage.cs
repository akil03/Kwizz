using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsPage : MonoBehaviour
{
    Question current;
    public Text question;
    public Text[] options;
    public Toggle[] toggleGroup;
    public ToggleGroup toggleGroupElement;
    public Text timer;
    bool timeout;

    public void Setup(Question question)
    {
        current = question;
        timer.text = "10";
        this.question.text = question.question;
        for (int i = 0; i < options.Length; i++)
        {
            options[i].text = question.options[i];
        }
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        int i = 10;
        while (i > 0)
        {
            yield return new WaitForSeconds(1);
            i--;
            timer.text = i.ToString();
        }
        timeout = true;
        AnswerQuestion();
    }

    public void AnswerQuestion()
    {
        if (!timeout)
        {
            for (int i = 0; i < toggleGroup.Length; i++)
            {
                if (toggleGroup[i].isOn)
                {
                    new GameSparkRequests("AnswerQuesion").Add("code", current.code).Add("date", DateTime.Today.AddDays(ErrorCodes.daysOffset).ToString("dd/MM/yyyy")).Add("answer", i).Request(SaveWinnerCallback, SaveLoserCallback);
                }
            }
        }
        else
        {
            new GameSparkRequests("AnswerQuesion").Add("code", current.code).Add("date", DateTime.Today.AddDays(ErrorCodes.daysOffset).ToString("dd/MM/yyyy")).Add("answer", 9).Request(FailedCallback: TimeoutCallback);
        }
        StopCoroutine(StartTimer());
        timeout = false;
    }

    private void SaveWinnerCallback(string str)
    {
        GSResult result = JsonUtility.FromJson<GSResult>(str);
        if (result.scriptData.result == "correct answer")
        {
            toggleGroupElement.SetAllTogglesOff();
            Popup.Instance.DisplayMessage("Correct answer. Wait for next question.");
            User.instance.OpenGame();
        }
        if (result.scriptData.result == "winner")
        {
            toggleGroupElement.SetAllTogglesOff();
            Popup.Instance.DisplayMessage("You won the game today.");
            User.instance.OpenGame();
        }
    }

    private void SaveLoserCallback(string str)
    {
        WrongAnswer(str, "Wrong answer. Please try again tomorrow.");
    }

    private void TimeoutCallback(string str)
    {
        WrongAnswer(str, "Timeout. Please try again tomorrow.");
    }

    private void WrongAnswer(string str, string message)
    {
        toggleGroupElement.SetAllTogglesOff();
        GSError error = JsonUtility.FromJson<GSError>(str);
        if (error.error.Status == ErrorCodes.wrongAnswer)
        {
            User.instance.OpenGame();
            Popup.Instance.DisplayMessage(message);
        }
    }
}
