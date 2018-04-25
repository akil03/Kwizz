using UnityEngine;
using UnityEngine.UI;

public class QuestionElement : MonoBehaviour
{
    public Question Question;
    public InputField questionInput;
    public InputField[] inputFields;
    public Toggle[] toggleGroup;
    public CodeGenerator generator = new CodeGenerator();


    // Use this for initialization
    void Start()
    {
        Question.code = generator.GenerateCode();
        for (int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].onEndEdit.AddListener(OptionAdded);
        }
        foreach (var item in toggleGroup)
        {
            item.onValueChanged.AddListener(Toggled);
        }
        questionInput.onEndEdit.AddListener(QuestionAdded);
    }

    private void QuestionAdded(string arg0)
    {
        Question.question = arg0;
    }

    private void Toggled(bool arg0)
    {
        for (int i = 0; i < toggleGroup.Length; i++)
        {
            if (toggleGroup[i].isOn)
            {
                Question.correctIndex = i;
            }
        }
    }

    private void OptionAdded(string arg0)
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            Question.options[i] = inputFields[i].text;
        }
    }

    public void Setup(Question question)
    {
        this.Question = question;
        questionInput.text = question.question;
        for (int i = 0; i < inputFields.Length; i++)
        {
            inputFields[i].text = question.options[i];
            if (question.correctIndex == i)
            {
                toggleGroup[i].isOn = true;
            }
        }
    }
}
