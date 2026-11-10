using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    public UnityEvent<GameObject> onCharacterSpawned;
    public UnityEvent<GameObject> onBeforeCharacterDestroyed;

    private static SpawnManager instance;

    public static SpawnManager Instance
    {
        get 
        {
            if (instance == null) instance = FindFirstObjectByType<SpawnManager>();
            return instance; 
        }
    }

    [SerializeField] private List<Transform> spawnpointsPerTeam = new();
    [SerializeField] private float spawnOffsetRandomRange = 5f;
    [SerializeField] private GameObject assaultPrefab;
    private List<GameObject> characters = new List<GameObject>();

    public IReadOnlyList<GameObject> Characters => characters.AsReadOnly();

    public void SpawnTeams()
    {
        if (TeamInfoManager.Instance == null)
        {
            Debug.LogError("Cannot spawn teams! Team info manager not available.");
            return;
        }

        TeamInfo team1 = TeamInfoManager.Instance.GetTeamInfo(1);
        if (team1 == null)
        {
            Debug.LogError("Team 1 is not available! Please set a team 1 in the inspector in the Team Info Manager");
            return;
        }

        TeamInfo team2 = TeamInfoManager.Instance.GetTeamInfo(2);
        if (team2 == null)
        {
            Debug.LogError("Team 2 is not available! Please set a team 1 in the inspector in the Team Info Manager");
            return;
        }

        SpawnTeam(team1, 1);
        SpawnTeam(team2, 2);
    }

    private void SpawnTeam(TeamInfo teamInfo, int teamNumber)
    {
        for (int characterNumber = 1; characterNumber <= teamInfo.characters.Count; characterNumber++)
        {
            GameObject prefab = teamInfo.characters[characterNumber - 1];
            if (prefab == null)
            {
                Debug.LogWarning("Character at index " + characterNumber + " is invalid for team info " + teamInfo.name);
                continue;
            }

            GameObject character = Instantiate(prefab, GetSpawnPoint(teamNumber), Quaternion.identity);
            character.name = prefab.name + " " + characterNumber.ToString() + " [Team " + teamNumber.ToString() + "]";

            if (character.TryGetComponent(out CharacterInfo characterInfo)) {
                characterInfo.teamIndex = teamNumber;
            }
            
            onCharacterSpawned.Invoke(character);
            characters.Add(character);
        }
    }

    public Vector3 GetSpawnPoint(int teamNumber)
    {
        teamNumber = Mathf.Clamp(teamNumber, 0, spawnpointsPerTeam.Count);
        Vector3 position = spawnpointsPerTeam[teamNumber - 1].position;
        Vector2 offset = Random.onUnitCircle * spawnOffsetRandomRange;
        return position + new Vector3(offset.x, 0f, offset.y);
    }

    public void DestroyAllCharacters()
    {
        while (characters.Count > 0)
        {
            onBeforeCharacterDestroyed.Invoke(characters[0]);
            Destroy(characters[0]);
            characters.RemoveAt(0);
        }
    }
}
