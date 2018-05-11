using UnityEngine;
using UnityEngine.UI;

public class LeaderboardElement : MonoBehaviour
{
    public Text name, rank, score;
    public void Setup(Datum datum)
    {
        name.text = datum.userName;
        rank.text = datum.rank.ToString();
        score.text = datum.score.ToString();
    }
}