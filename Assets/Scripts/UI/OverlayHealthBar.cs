using TMPro;
using UnityEngine;

public class OverlayHealthBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private RectTransform middleBar;
    [SerializeField] private RectTransform rightBar;

    [SerializeField] private float middleBarLeftOffsetMin;
    [SerializeField] private float rightBarWidth;

    public void OnHealthChanged(HealthController healthController, float newHealth)
    {
        healthText.text = Mathf.CeilToInt(newHealth).ToString() + "/" + Mathf.CeilToInt(healthController.MaxHealth).ToString();
        float ratio = 1.0f - newHealth / healthController.MaxHealth;
        middleBar.offsetMin = new Vector2(middleBarLeftOffsetMin * ratio, middleBar.offsetMin.y);
        middleBar.offsetMax = new Vector2(middleBarLeftOffsetMin * ratio, middleBar.offsetMax.y);

        rightBar.anchoredPosition = new Vector2(middleBarLeftOffsetMin * ratio + rightBarWidth, rightBar.anchoredPosition.y);
    }
}
