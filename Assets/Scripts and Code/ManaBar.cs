using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;

    public void SetMaxMana(int maxMana)
    {
        slider.maxValue = maxMana;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetCurrentMana(int currentMana)
    {
        slider.value = currentMana;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
