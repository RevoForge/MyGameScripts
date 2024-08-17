using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealth : MonoBehaviour
{
    private int MaxHealth = 10;
    private float CurrentHealth;
    public Slider HealthBar;
    public TextMeshProUGUI DamageText;
    private bool healing;
    private bool textFade;
    private float textTimer;
    private float textTimerRef = 3f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (textFade)
        {
            textTimer -= Time.deltaTime;
            if (textTimer <= 0)
            {
                textFade = false;
                textTimer = textTimerRef;
                DamageText.text = "";
            }
        }
        HealthBar.value = CurrentHealth;

        if (CurrentHealth <= 0)
        {
            healing = true;
        }
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
            healing = false;
        }
        if (healing)
        {
            CurrentHealth += 0.01f;
        }
    }
    public (bool, bool) TakeDamage(int damage)
    {
        bool isDead = false;
        bool isNegativeHealthPrevented = false;

        if (CurrentHealth <= 0)
        {
            isNegativeHealthPrevented = true;
        }
        else
        {
            CurrentHealth -= damage;
            DamageText.text = "-" + damage;
            textFade = true;
            if (CurrentHealth <= 0)
            {
                isDead = true;
            }
        }

        return (isDead, isNegativeHealthPrevented);
    }

}
