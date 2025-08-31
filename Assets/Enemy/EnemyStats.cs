using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public WinGameManager winGameManager; // 🟢 اربطه من الـInspector

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (winGameManager != null)
        {
            winGameManager.ShowWinScreen();
        }
        else
        {
            Debug.LogWarning("⚠️ WinGameManager غير مربوط في EnemyStats");
        }

        Destroy(gameObject);
    }
}
