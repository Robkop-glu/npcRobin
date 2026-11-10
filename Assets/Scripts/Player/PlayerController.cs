using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterMotor motor;
    private CharacterCombatController combatController;
    [SerializeField] private CharacterAnimationController animationController;
    private Targeter targeter;
    private HealthController healthController;

    [SerializeField] private Transform aimTarget;
    [SerializeField] private CameraInfo firstPersonCameraInfo;

    private bool mouseIsLocked = false;

    private void Awake()
    {
        motor = GetComponent<CharacterMotor>();
        combatController = GetComponent<CharacterCombatController>();
        targeter = GetComponent<Targeter>();
        healthController = GetComponent<HealthController>();
    }

    private void Start()
    {
        animationController.SetAimTarget(aimTarget);
    }

    private void Update()
    {

    }

    public void OnHealthControllerDie(HealthController _hc)
    {
        Debug.Log("Player died.");

        motor.Freeze();
        combatController.enabled = false;
        animationController.Die();

        if (targeter != null)
        {
            targeter.ClearAll();
        }

        Invoke(nameof(Respawn), 3f);
    }

    public void Respawn()
    {
        healthController.Respawn();
        animationController.Respawn(); // Set the right animationa

        // Teleport the character to the right position
        int teamIndex = GetComponent<CharacterInfo>().teamIndex;
        motor.Teleport(SpawnManager.Instance.GetSpawnPoint(teamIndex));

        // Unfreeze the character so it can move again.
        motor.Unfreeze();
        combatController.enabled = true;

        animationController.SetAimTarget(aimTarget);
    }

    public void OnFirstPersonCameraActiveChanged(bool active)
    {
        SetMouseLock(active);
    }

    public void OnShootInput()
    {
        if (!healthController.Alive) return;

        Vector3 direction = (aimTarget.transform.position - combatController.Weapon.firePoint.transform.position).normalized;
        combatController.TryShoot(direction);
    }

    public void SetMouseLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        mouseIsLocked = locked;
    }

    public void ToggleMouseLock()
    {
        if (!firstPersonCameraInfo.CurrentlyActive) return;

        SetMouseLock(!mouseIsLocked);
    }
}
