using UnityEngine;
using UnityEngine.UI;

public class QuestionsPage : MonoBehaviour
{
    Question current;
    public Text question;
    public Text[] options;
    public Toggle[] toggleGroup;
    int questionsAnswered;

    public void Setup(Question question)
    {
        current = question;
        this.question.text = question.question;
        for (int i = 0; i < options.Length; i++)
        {
            options[i].text = question.options[i];
        }
    }

    public void AnswerQuestion()
    {
        for (int i = 0; i < toggleGroup.Length; i++)
        {
            if (toggleGroup[i].isOn)
            {
                if (current.Answer(i))
                {
                    questionsAnswered++;
                    if (questionsAnswered == 10)
                    {
                        GameSparkRequests request = new GameSparkRequests();
                        request.Request("SaveWinner", SaveWinnerCallback);
                    }
                    else
                    {
                        User.instance.OpenGame();
                    }
                }
                else
                {
                    GameSparkRequests request = new GameSparkRequests();
                    request.Request("SaveLoser", SaveLoserCallback);
                }
            }
        }
    }

    private void SaveWinnerCallback(string str)
    {
        print("you won motherfucker!");
    }

    private void SaveLoserCallback(string str)
    {
        GameSparksManager.instance.LogOut();
        User.instance.Logout();
    }
}
