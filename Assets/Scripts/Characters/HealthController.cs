using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    public UnityEvent<HealthController, HealthChangeInfo> onTakeDamage;
    public UnityEvent<HealthController> onDie;

    public UnityEvent<HealthController, bool> onAliveStateChanged;
    public UnityEvent<HealthController, float> onCurrentHealthChanged;

    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth;

    [SerializeField] [Range(0f, 1f)] private float initialRatio = 1f;
    [SerializeField] private float currentHealth = 100f;

    private bool alive = true;
    public bool Alive => alive;

    private void Awake()
    {
        Respawn();
    }

    public void Respawn()
    {
        ChangeHealth(maxHealth * initialRatio);
        alive = true;
        onAliveStateChanged?.Invoke(this, true);
    }

    public void ApplyHealthChangeInfo(HealthChangeInfo info)
    {
        if (info.rawAmount < 0.0) TakeDamage(info);
    }

    public void TakeDamage(HealthChangeInfo info)
    {
        float damage = info.rawAmount;
        if (Mathf.Approximately(damage, 0f)) return;
        if (!alive) return;

        // Correct the damage so we never do more damage than health we have.
        damage = Mathf.Min(Mathf.Abs(damage), currentHealth);

        // Never go below zero health.
        ChangeHealth(currentHealth - damage);

        onTakeDamage.Invoke(this, info);

        // To prevent float precision mistakes, we use Approximately.
        if (currentHealth <= 0f || Mathf.Approximately(currentHealth, 0f))
        {
            Die();
        }
    }

    public void Die()
    {
        ChangeHealth(0f);
        alive = false;
        onDie.Invoke(this);
        onAliveStateChanged?.Invoke(this, false);
    }

    private void ChangeHealth(float newHealth)
    {
        float oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(newHealth, 0f, maxHealth);
        onCurrentHealthChanged?.Invoke(this, currentHealth);
    }
}
