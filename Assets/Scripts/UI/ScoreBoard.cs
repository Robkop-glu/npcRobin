using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private RectTransform teamVisualsParent;
    [SerializeField] private Image team1Visual;
    [SerializeField] private Image team2Visual;
    [SerializeField] private TextMeshProUGUI team1ScoreText;
    [SerializeField] private TextMeshProUGUI team2ScoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    public void OnRoundStarted()
    {
        gameObject.SetActive(true);
    }

    public void OnRoundEnded()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        var teamInfo = TeamInfoManager.Instance.GetTeamInfo(1);
        team1Visual.color = teamInfo.Color;
        int team1Score = ObjectiveManager.Instance.GetScore(1);
        team1ScoreText.text = teamInfo.DisplayName + "\n" + team1Score;

        int team2Score = ObjectiveManager.Instance.GetScore(2);
        teamInfo = TeamInfoManager.Instance.GetTeamInfo(2);
        team2Visual.color = teamInfo.Color;
        team2ScoreText.text = teamInfo.DisplayName + "\n" + team2Score;

        float totalScore = (float)(team1Score + team2Score);
        float team1VisualRatio = 0.5f;
        float team2VisualRatio = 0.5f;

        if (!Mathf.Approximately(totalScore, 0))
        {
            team1VisualRatio = team1Score / totalScore;
            team2VisualRatio = team2Score / totalScore;
        }

        team1Visual.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, teamVisualsParent.rect.width * team1VisualRatio);
        team2Visual.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, teamVisualsParent.rect.width * team2VisualRatio);

        float timeLeft = RoundManager.Instance.TimeLeftInRound;
        int minutes = Mathf.FloorToInt(timeLeft / 60.0f);
        int secondsLeft = Mathf.CeilToInt(timeLeft - (minutes * 60.0f));
        string minutesText = minutes.ToString();
        string secondsText = secondsLeft.ToString();
        if (secondsLeft < 10) secondsText = "0" + secondsText;
        timerText.text = minutesText + ":" + secondsText;
    }
}
