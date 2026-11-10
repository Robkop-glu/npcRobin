using UnityEngine;

public abstract class Sensor : MonoBehaviour
{
    public abstract bool IsTargetDetectable(GameObject target);

    public abstract void Clear();
}
