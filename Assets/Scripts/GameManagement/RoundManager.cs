using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private float timePerRound = 300f;
     
    public UnityEvent OnRoundStarted;
    public UnityEvent OnRoundEnded;

    private bool roundInProgress = false;
    public bool RoundInProgress => roundInProgress;

    private float currentRoundTimer = 0f;

    public float CurrentRoundTimer => currentRoundTimer;
    public float TimeLeftInRound => Mathf.Clamp(timePerRound - currentRoundTimer, 0f, timePerRound);

    private static RoundManager instance;

    public static RoundManager Instance
    {
        get
        {
            if (instance == null) instance = FindFirstObjectByType<RoundManager>();
            return instance;
        }
    }

    private void Start()
    {
       StartRound();
    }

    public void StartRound()
    {
        SpawnManager.Instance.SpawnTeams();
        currentRoundTimer = 0f;
        roundInProgress = true;
        OnRoundStarted?.Invoke();
    }

    private void Update()
    {
        if (roundInProgress) {
            currentRoundTimer += Time.deltaTime;

            if (currentRoundTimer >= timePerRound)
            {
                EndCurrentRound();
            }
        }
    }

    private void EndCurrentRound()
    {
        currentRoundTimer = timePerRound;
        roundInProgress = false;
        SpawnManager.Instance.DestroyAllCharacters();
        Debug.Log("Round over!");
        OnRoundEnded?.Invoke();
    }
}
