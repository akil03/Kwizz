using UnityEngine;
using UnityEngine.EventSystems;
public class CloseQuestionButton : MonoBehaviour, IPointerClickHandler
{
    public QuestionElement element;
    public void OnPointerClick(PointerEventData eventData)
    {
        EventManager.Instance.CloseQuestion(element.Question);
    }
}
