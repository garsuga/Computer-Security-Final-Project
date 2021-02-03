using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float waitTime;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 maxScaleSize;
    [SerializeField] private float scaleSpeed;

    [Header("Shake Settings")]
    [SerializeField] private float maxShakeRotation;
    [SerializeField] private float shakeSpeed;

    [Header("Color Settings")]
    [SerializeField] private Color takeDamageColor;
    [SerializeField] private Image sliderFill;


    private HealthEffects _takeDamageEffect;
    private WaitForSeconds _wait;

    void Awake()
    {
        _wait = new WaitForSeconds(waitTime);
        _takeDamageEffect = new HealthEffects(this);
        _takeDamageEffect
            .AddEffect(new FlashColorEffect(sliderFill.color, takeDamageColor, sliderFill, _wait))
            .AddEffect(new ScaleRectEffect(slider.GetComponent<RectTransform>(), maxScaleSize, scaleSpeed, _wait))
            .OnAllEffectsComplete += OnAllEffectsComplete;
    }

    // Update is called once per frame
    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += HandleHealthChanged;
        PlayerHealth.OnMaxHealthChanged += HandleMaxHealthChanged;
    }

    private void OnDestroy()
    {
        PlayerHealth.OnHealthChanged -= HandleHealthChanged;
        PlayerHealth.OnMaxHealthChanged -= HandleMaxHealthChanged;
    }


    private void HandleHealthChanged(float currentHealth)
    {
        slider.value = currentHealth;
        _takeDamageEffect.ExecuteEffects();
    }

    private void HandleMaxHealthChanged(float maxHealth)
    {
        slider.maxValue = maxHealth;
    }

    private void OnEffectComplete(UI_Effect effect)
    {
        print($"Completed {effect}!");
    }

    private void OnAllEffectsComplete()
    {
        print("Completed all effects!");
    }
}
