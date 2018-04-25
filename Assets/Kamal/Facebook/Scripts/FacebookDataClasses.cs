using System;
using System.Collections.Generic;

[Serializable]
public class FacebookLoginResult
{
    public string permissions;
    public string expiration_timestamp;
    public string access_token;
    public string user_id;
    public string last_refresh;
    public List<string> granted_permissions;
    public List<object> declined_permissions;
    public string callback_id;
}

[Serializable]
public class FacebookUserDetailsResult
{
    public string id;
    public string name;
}

[Serializable]
public class Summary
{
    public int total_count;
}

[Serializable]
public class FacebookFriendsResult
{
    public List<object> data;
    public Summary summary;
}