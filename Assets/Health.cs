using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum FACTION { player, enemy, other };
public class Health : MonoBehaviour
{
    [SerializeField] FACTION faction = FACTION.enemy;
    [SerializeField] Text healthText;
    [SerializeField] int maxHealth = 100;
    [SerializeField] UnityEvent zeroHealthActions;
    [SerializeField] OnHit onHit;
    int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    private void Start()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
        }
    }

    public void DealDamage(int damage)
    {
        onHit?.Hit();
        currentHealth -= damage;
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
        }
        if (currentHealth <= 0)
        {
            this.gameObject.SetActive(false);
            zeroHealthActions.Invoke();
        }
    }


    public FACTION GetFaction() => faction;
    public (int current, int max) GetHealth() => (currentHealth, maxHealth);
}
