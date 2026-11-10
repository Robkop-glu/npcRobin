using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    private static ObjectiveManager instance;
    public static ObjectiveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ObjectiveManager>();
            }
            return instance;
        }
    }

    [SerializeField] private Transform capturePointTransform;
    [SerializeField] private SimpleAreaSensor outerSensor;
    [SerializeField] private SimpleAreaSensor innerSensor;

    [SerializeField] [Range(1, 10)] private int scoreTickOuter = 2;
    [SerializeField] [Range(1, 25)] private int scoreTickInner = 5;
    [SerializeField] private float scoreTickInterval = 1f;
    private float scoreTickTimer = 0f;

    [SerializeField] private List<int> scorePerTeam = new() { 0, 0, 0, 0 };

    public Vector3 CapturePointPosition => capturePointTransform.position;

    private void Awake()
    {
        scorePerTeam = new() { 0, 0, 0, 0 };
    }

    private void Update()
    {
        if (!RoundManager.Instance.RoundInProgress) return;

        scoreTickTimer += Time.deltaTime;
        if (scoreTickTimer >= scoreTickInterval)
        {
            ScoreTick();
            scoreTickTimer = 0f;
        }
    }

    private void ScoreTick()
    {
        foreach(GameObject objectInInner in innerSensor.AllDetectedObjects)
        {
            TryAddScoreForObject(objectInInner, scoreTickInner);
        }

        foreach (GameObject objectInOuter in outerSensor.AllDetectedObjects)
        {
            if (innerSensor.AllDetectedObjects.Contains(objectInOuter)) continue;
            TryAddScoreForObject(objectInOuter, scoreTickOuter);
        }
    }

    private void TryAddScoreForObject(GameObject go, int score)
    {
        if (!go.TryGetComponent(out CharacterInfo characterInfo)) return;
        if (go.TryGetComponent(out HealthController hc) && !hc.Alive) return;

        scorePerTeam[characterInfo.teamIndex - 1] += score;
    }

    public int GetScore(int teamIndex)
    {
        return scorePerTeam[teamIndex - 1];
    }
}
