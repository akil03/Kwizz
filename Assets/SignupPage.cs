using UnityEngine;
using UnityEngine.UI;

public class SignupPage : MonoBehaviour
{
    public InputField name, phoneNumber, email, password, confirmPassowrd;
    string number, uname, pwd;
    bool saveNumber;

    private void OnEnable()
    {
        GameSparksManager.Instance.RegistrationSuccess += OnRegistrationSuccess;
        GameSparksManager.Instance.authenticated += OnLogin;
    }

    private void OnLogin()
    {
        if (saveNumber)
        {
            saveNumber = false;
            User.instance.SavePhoneNumber(number);
            number = string.Empty;
        }
    }

    private void OnDisable()
    {
        GameSparksManager.Instance.RegistrationSuccess -= OnRegistrationSuccess;
    }

    private void OnRegistrationSuccess(string str)
    {
        gameObject.SetActive(false);
    }

    public void Register()
    {
        try
        {
            long.Parse(phoneNumber.text);
            if (phoneNumber.text.Length < 10 || phoneNumber.text.Length > 12)
            {
                Popup.Instance.DisplayMessage("Please enter a valid phone number!");
                return;
            }
        }
        catch
        {
            Popup.Instance.DisplayMessage("Please enter a valid phone number!");
            return;
        }
        if (!email.text.Contains("@") || !email.text.Contains(".com"))
        {
            Popup.Instance.DisplayMessage("Please enter a valid email address!");
            return;
        }
        if (password.text != confirmPassowrd.text)
        {
            Popup.Instance.DisplayMessage("Passowrds do not match!");
            return;
        }
        number = phoneNumber.text;
        uname = email.text;
        pwd = password.text;
        GameSparksManager.Instance.Register(name.text, email.text, password.text, phoneNumber.text);
        name.text = ""; phoneNumber.text = ""; email.text = ""; password.text = ""; confirmPassowrd.text = "";
    }
}
