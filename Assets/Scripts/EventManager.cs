using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    public bool debugLog;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}

public delegate void StringDelegate(string str);
public delegate void ParameterlessDelegate();
