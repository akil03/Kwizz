using UnityEngine;
using UnityEngine.UI;

public class SignupPage : MonoBehaviour
{
    public InputField name, phoneNumber, email, password, confirmPassowrd;
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
            User.instance.SavePhoneNumber(phoneNumber.text);
        }
    }

    private void OnDisable()
    {
        GameSparksManager.Instance.RegistrationSuccess -= OnRegistrationSuccess;
    }

    private void OnRegistrationSuccess(string str)
    {
        saveNumber = true;
        GameSparksManager.Instance.Login(email.text, password.text);
    }

    public void Register()
    {
        if (password.text != confirmPassowrd.text)
        {
            Popup.Instance.DisplayMessage("Passowrds do not match!");
            return;
        }
        GameSparksManager.Instance.Register(name.text, email.text, password.text);
    }
}
