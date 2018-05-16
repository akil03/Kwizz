public class GoogleManager : Singleton<GoogleManager>
{
    public ParameterlessDelegate loginSuccess;

    private void Start()
    {
        LCGoogleLoginBridge.InitWithClientID("683220570490-nvghovlfjkascnib08t5rrb2dqtiop1f.apps.googleusercontent.com");
    }

    public void Login()
    {
        LCGoogleLoginBridge.LoginUser(Callback, false);
    }

    private void Callback(bool obj)
    {
        if (obj)
        {
            loginSuccess();
        }
    }
}
