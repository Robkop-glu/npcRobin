using UnityEngine;

public class HealthChangeInfo
{
    /// <summary>
    /// The agent that is trying to heal or inflict damage, probably a character.
    /// </summary>
    public GameObject originAgent;

    /// <summary>
    /// The 'device' that the agent tries to do the healing or damage with. This is most
    /// likely a weapon.
    /// </summary>
    public GameObject originDevice;

    /// <summary>
    /// The original target that the heal of damage was intended to reach.
    /// </summary>
    public GameObject originalTarget;

    /// <summary>
    /// The position where the health change is supposed to take place.
    /// </summary>
    public Vector3 targetPosition;

    /// <summary>
    /// The raw amount of healing or damage the object is trying to do.
    /// </summary>
    public float rawAmount;


}
