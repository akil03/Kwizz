using System.Collections.Generic;
using UnityEngine.UI;

public class TestHelper : Singleton<TestHelper>
{
    public List<string> codes;
    public InputField code;
    int index = 0;
    public void SetCode()
    {
        if (index <= 10)
        {
            code.text = codes[index];
            index++;
        }
        else
        {
            index = 0;
            code.text = codes[index];
            index++;
        }
    }
}
