using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamInfo", menuName = "Team Info Object")]
public class TeamInfo : ScriptableObject
{
    [SerializeField] private string displayName = "Team Something";
    public string DisplayName => displayName;

    [SerializeField] private Color color = Color.white;
    public Color Color => color;

    public List<GameObject> characters = new();

    public static TeamInfo CreateInvalid()
    {
        TeamInfo ti = CreateInstance<TeamInfo>();
        ti.displayName = "[invalid team info object]";
        return ti;
    }
}
