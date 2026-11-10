using UnityEngine;

public class PlayerOverlay : MonoBehaviour
{
    public void OnCameraBecameActive(bool active)
    {
        gameObject.SetActive(active);
    }

}
