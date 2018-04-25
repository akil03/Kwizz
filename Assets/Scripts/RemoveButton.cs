using UnityEngine;

public class RemoveButton : MonoBehaviour
{
    public QuestionElement element;
    public void Remove()
    {
        QuestionsManager.instance.RemoveQuestion(element);
    }
}
