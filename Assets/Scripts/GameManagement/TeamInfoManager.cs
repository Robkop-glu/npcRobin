using System.Collections.Generic;
using UnityEngine;

public class TeamInfoManager : MonoBehaviour
{
    private static TeamInfoManager instance;

    public static TeamInfoManager Instance
    {
        get 
        {
            if (instance == null) instance = FindFirstObjectByType<TeamInfoManager>();
            return instance; 
        }
    }

    [SerializeField] private List<TeamInfo> teamInfos = new();

    public TeamInfo GetTeamInfo(int id)
    {
        if (id < 0 || id >= teamInfos.Count)
        {
            return TeamInfo.CreateInvalid();
        }
        return teamInfos[id - 1];
    }
}
