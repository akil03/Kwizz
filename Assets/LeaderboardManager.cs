using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public LeaderboardElement leaderboardElement;
    public List<LeaderboardElement> leaderboardElements;
    public Transform container;

    private void OnEnable()
    {
        GameSparksManager.Instance.gotLeaderboard += GotLeaderboard;
        GameSparksManager.Instance.gotPlayerInLeaderoard += GotPlayerPositionInLeaderboard;
        GameSparksManager.Instance.loggedOut += Reset;
    }

    private void GotPlayerPositionInLeaderboard(List<Datum> list)
    {
        foreach (var item in list)
        {
            var instance = Instantiate(leaderboardElement);
            instance.Setup(item);
            instance.transform.SetParent(container);
            instance.transform.SetAsFirstSibling();
            instance.transform.localScale = Vector3.one;
            leaderboardElements.Add(instance);
        }
    }

    private void GotLeaderboard(List<Datum> list)
    {
        foreach (var item in list)
        {
            var instance = Instantiate(leaderboardElement);
            instance.Setup(item);
            instance.transform.SetParent(container);
            instance.transform.localScale = Vector3.one;
            leaderboardElements.Add(instance);
        }
    }

    void Reset()
    {
        foreach (var item in leaderboardElements)
        {
            Destroy(item.gameObject);
        }
        leaderboardElements.Clear();
    }
}
