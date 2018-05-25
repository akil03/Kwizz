public class Loading : Singleton<Loading>
{
    public void StartLoading()
    {
        gameObject.SetActive(true);
    }

    public void StopLoading()
    {
        gameObject.SetActive(false);
    }
}
