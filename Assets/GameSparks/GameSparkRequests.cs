using System.Collections.Generic;
using GameSparks.Api.Requests;
using UnityEngine;

public class GameSparkRequests
{
    Dictionary<string, object> dictionary = new Dictionary<string, object>();
    string eventKey;

    public GameSparkRequests(string eventKey)
    {
        this.eventKey = eventKey;
    }

    public GameSparkRequests Add(string key, object value)
    {
        dictionary.Add(key, value);
        return this;
    }

    public void Request(StringDelegate SuccessCallback = null, StringDelegate FailedCallback = null)
    {
        var req = new LogEventRequest();
        foreach (var item in dictionary)
        {
            req.SetEventAttribute(item.Key, item.Value.ToString());
        }
        req.SetEventKey(eventKey).Send((response) =>
        {
            if (response.HasErrors)
            {
                Debug.Log(eventKey + " request failed!\n" + response.Errors.JSON);
                if (FailedCallback != null)
                {
                    FailedCallback(response.JSONString);
                }
            }
            else
            {
                Debug.Log(eventKey + " request success!\n" + response.JSONString);
                if (SuccessCallback != null)
                {
                    SuccessCallback(response.JSONString);
                }
            }
        });
    }
}
