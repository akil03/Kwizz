using System;
using System.Collections.Generic;

[Serializable]
public class Question
{
    public QuestionType type;
    public string question;
    public string code;
    public int correctIndex;
    public List<string> options = new List<string>(4);

    public bool Answer(int index)
    {
        return index == correctIndex;
    }
}

public enum QuestionType
{
    text, image
}