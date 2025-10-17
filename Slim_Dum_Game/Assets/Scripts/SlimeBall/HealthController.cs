using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    Image healthBar;

    [SerializeField]
    Gradient _healthGradient;

    [SerializeField]
    public float maxHealth = 100.0F;

    [SerializeField]
    public float minHealth = 0.001F;

    [SerializeField]
    public float _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void UpdateHealth(float amount)
    {
        _currentHealth += amount;
        if (_currentHealth > maxHealth){
            _currentHealth = maxHealth;
        }
        _currentHealth = Mathf.Clamp(_currentHealth, 0.0F, maxHealth);
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {

        float fillAmount = _currentHealth / maxHealth;
        Color fillColor = _healthGradient.Evaluate(fillAmount);

        healthBar.fillAmount = fillAmount;
        healthBar.color = fillColor;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= minHealth)
        {
            _currentHealth = 0.0f;
        }
    }
}
