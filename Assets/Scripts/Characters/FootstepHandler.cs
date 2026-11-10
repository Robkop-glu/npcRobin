using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
   

    [SerializeField] private AudioSource footstepSource;


    public void OnFootLanded(bool left)
    {
        footstepSource.Play();
    }
}
