using TMPro;
using UnityEngine;

public class PostRoundScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;

    public void OnRoundStarted()
    {
        gameObject.SetActive(false);
    }

    public void OnRoundEnded()
    {
        gameObject.SetActive(true);

        float scoreTeam1 = ObjectiveManager.Instance.GetScore(1);
        float scoreTeam2 = ObjectiveManager.Instance.GetScore(2);

        if (scoreTeam1 > scoreTeam2)
        {
            TeamInfo team = TeamInfoManager.Instance.GetTeamInfo(1);
            titleText.text = "Round over! Team 1 " + team.DisplayName + " won!";
        }
        else if(scoreTeam2 > scoreTeam1)
        {
            TeamInfo team = TeamInfoManager.Instance.GetTeamInfo(2);
            titleText.text = "Round over! Team 2 " + team.DisplayName + "won!";
        }
        else
        {
            titleText.text = "Round over! Draw.";
        }

    }
}
