using UnityEngine;
public class CodeGenerator
{
    string[] letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "a", "b" };

    public string GenerateCode()
    {
        string result = Random.Range(111111, 999999).ToString();
        char[] characters = result.ToCharArray();
        result = string.Empty;
        foreach (var character in characters)
        {
            int Char = Random.Range(0, 2);
            int Case = Random.Range(0, 2);
            int multiplier = Random.Range(1, 4);
            if (Char == 1)
            {
                result += Case == 1 ? letters[int.Parse(character.ToString()) * multiplier].ToUpper() : letters[int.Parse(character.ToString()) * multiplier];
            }
            else
            {
                result += character;
            }
        }
        return result;
    }
}