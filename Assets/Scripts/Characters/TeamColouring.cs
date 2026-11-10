using System.Collections.Generic;
using UnityEngine;

public class TeamColouring : MonoBehaviour
{
    [SerializeField] private CharacterInfo characterInfo;

    private void Start()
    {
        Color color = Color.royalBlue;
        if (TeamInfoManager.Instance != null)
        {
            color = TeamInfoManager.Instance.GetTeamInfo(characterInfo.teamIndex).Color;
        }

        if (TryGetComponent(out MeshRenderer meshRenderer))
        {
            meshRenderer.material = Instantiate(meshRenderer.material);
            meshRenderer.material.color = color;
        }
        else if (TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
        {
            skinnedMeshRenderer.material = Instantiate(skinnedMeshRenderer.material);
            skinnedMeshRenderer.material.color = color;
        }
        else
        {
            Debug.Log("Could not find either mesh renderer or skinned mesh renderer on " + gameObject.name);
        }
        
    }
}
