using UnityEngine;
using UnityEngine.UI;

public class ImageColorChanger : MonoBehaviour
{
    public Image image;
    Color start;
    public Color target = Color.green;

    void Start()
    {
        start = image.color;
    }

    public void Change()
    {
        image.color = target;
    }

    public void Reset()
    {
        image.color = start;
    }
}