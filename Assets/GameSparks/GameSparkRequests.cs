using System.Collections.Generic;
using GameSparks.Api.Requests;
using UnityEngine;

public class GameSparkRequests
{
    public void Request(string eventKey, string attributeKey, string attributeValue, StringDelegate SuccessCallback = null, StringDelegate FailedCallback = null)
    {
        new LogEventRequest()
            .SetEventKey(eventKey)
            .SetEventAttribute(attributeKey, attributeValue)
        .Send((response) =>
        {
            if (response.HasErrors)
            {
                Debug.Log(eventKey + " request failed!");
                if (FailedCallback != null)
                {
                    FailedCallback(response.JSONString);
                }
            }
            else
            {

            }
            if (SuccessCallback != null)
            {
                SuccessCallback(response.JSONString);
            }
        });
    }

    public void Request(string eventKey, Dictionary<string, object> dictionary, StringDelegate SuccessCallback, StringDelegate FailedCallback = null)
    {
        var req = new LogEventRequest();
        req.SetEventKey(eventKey);
        foreach (var item in dictionary)
        {
            if (item.Value != null)
            {
                req.SetEventAttribute(item.Key, item.Value.ToString());
            }
            else
            {
                Debug.Log(item.Key + " has null value!");
            }
        }
        req.Send((response) =>
        {
            if (response.HasErrors)
            {
                if (FailedCallback != null)
                {
                    FailedCallback(response.JSONString);
                }
            }
            else
            {
                if (SuccessCallback != null)
                {
                    SuccessCallback(response.JSONString);
                }
            }
        });
    }

    public void Request(string eventKey, StringDelegate SuccessCallback = null, StringDelegate FailedCallback = null)
    {
        new LogEventRequest()
            .SetEventKey(eventKey)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.Log(eventKey + " Request failed!");
                    if (FailedCallback != null)
                    {
                        FailedCallback(response.JSONString);
                    }
                }
                else
                {
                    Debug.Log("Request success!");
                }
                if (SuccessCallback != null)
                {
                    SuccessCallback(response.JSONString);
                }
            });
    }
}
