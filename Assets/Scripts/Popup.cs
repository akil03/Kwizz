using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Popup : Singleton<Popup>
{
    public GameObject popup;
    public Text message;
    public Button ok, dismiss, cancel;

    public void DisplayMessage(string message)
    {
        popup.SetActive(true);
        dismiss.gameObject.SetActive(true);
        ok.gameObject.SetActive(false);
        cancel.gameObject.SetActive(false);
        this.message.text = message;
        Loading.Instance.StopLoading();
    }

    public void DisplayMessage(string message, ParameterlessDelegate callback)
    {
        popup.SetActive(true);
        dismiss.gameObject.SetActive(true);
        ok.gameObject.SetActive(false);
        cancel.gameObject.SetActive(false);
        this.message.text = message;
        Loading.Instance.StopLoading();
        UnityAction action = new UnityAction(() =>
        {
            callback();
            dismiss.onClick.RemoveAllListeners();
        });
        dismiss.onClick.AddListener(action);
    }



    public void Close()
    {
        popup.SetActive(false);
    }
}
