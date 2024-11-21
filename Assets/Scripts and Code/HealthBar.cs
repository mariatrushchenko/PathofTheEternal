using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;

    /// <summary>
    /// function is called on start AND when max values (aka stats.maxHealth or mana is increased)
    /// </summary>
    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        fill.color = gradient.Evaluate(1f);
    }

    /// <summary>
    /// this function is called after damage has been applied or current health has been increased
    /// </summary>
    public void SetCurrentHealth(int currentHealth)
    {
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
