using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float deduction;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float Deduction => deduction;

    private Text _currentHealthText;
    public static event Action<float> OnHealthChanged;
    public static event Action<float> OnMaxHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        SetHealth(currentHealth);
        SetMaxHealth(maxHealth);
    }

    public void AdjustHealth()
    {
        currentHealth = Mathf.Clamp(currentHealth - deduction, 0, maxHealth);
        _currentHealthText = GameObject.Find("CurrentHealth").GetComponent<Text>();
        OnHealthChanged?.Invoke(currentHealth);
        _currentHealthText.text = currentHealth.ToString();
        if (currentHealth / maxHealth <= 0.3) _currentHealthText.color = Color.red;
    }

    public void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = Mathf.Clamp(value, 0, float.MaxValue);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnMaxHealthChanged?.Invoke(maxHealth);
    }

    public bool HealthEmpty() { return currentHealth <= 0; }

}
