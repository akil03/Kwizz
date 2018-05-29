
public class GoogleManager : Singleton<GoogleManager>
{
    public ParameterlessDelegate loginSuccess;

    private void Start()
    {
        LCGoogleLoginBridge.InitWithClientID("683220570490-nvghovlfjkascnib08t5rrb2dqtiop1f.apps.googleusercontent.com");
    }

    public void Login()
    {
        if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
        {
            Popup.Instance.DisplayMessage("Check your internet connection.");
            return;
        }
        Loading.Instance.StartLoading();
        LCGoogleLoginBridge.LoginUser(Callback, false);
    }

    private void Callback(bool obj)
    {
        if (obj)
        {
            loginSuccess();
        }
        else
        {
            Loading.Instance.StopLoading();
            GameSparksManager.Instance.LoginFailed();
        }
    }
}
