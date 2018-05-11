using UnityEngine;
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
    }

    public void Close()
    {
        popup.SetActive(false);
    }
}
