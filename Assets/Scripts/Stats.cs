using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
public class Stats : NetworkBehaviour {

    //Offensive
    [SyncVar(hook = "OnChangeAbilityPower")]
    public float AbilityPower = 0;
    [SyncVar(hook = "OnChangeAttackDamage")]
    public float AttackDamage = 0;
    [SyncVar(hook = "OnChangeAttackSpeed")]
    public float AttackSpeed = 0f;
    [SyncVar(hook = "OnChangeCriticalStrikeChance")]
    public float CriticalStrikeChance = 0f;
    [SyncVar(hook = "OnChangeCriticalStrikeMultiplier")]
    public float CriticalStrikeMultiplier = 2.0f;
    [SyncVar(hook = "OnChangeArmorPenetration")]
    public float ArmorPenetration = 0;
    [SyncVar(hook = "OnChangeMagicPenetration")]
    public float MagicPenetration = 0f;
    [SyncVar(hook = "OnChangeLifeSteal")]
    public float LifeSteal = 0f;

    //Defensive
    public float Armor;
    public float MagicResistance;
    [SyncVar(hook = "OnChangeHealthRegen")]
    public float HealthRegenerationPerSecond;
    Health healthReference;

    //Utility
    public float CooldownReduction = 0f;
    public float ManaRegeneration = 1f;
    public float Mana = 400f;
    public float CurrentMana = 400f;


    //Other
    public float Experience = 0f;
    [SyncVar(hook = "OnChangeGold")]
    public int Gold = 0;
    public int GoldGenerationPerSecond = 1;
    public float MovementSpeed = 10f;
    public float Range = 10f;



    private void Start()
    {
        healthReference = GetComponent<Health>();
        StartCoroutine(StatRegenerationPerSecond(1f));
    }

    IEnumerator StatRegenerationPerSecond(float yieldTime)
    {
        while (true)
        {

            if(CurrentMana < Mana) CurrentMana += ManaRegeneration;
            if (healthReference.currentHealth <= healthReference.maxHealth)
            {
                healthReference.currentHealth += (int)HealthRegenerationPerSecond;
                if (healthReference.currentHealth > healthReference.maxHealth) healthReference.currentHealth = healthReference.maxHealth;
            }
            yield return new WaitForSeconds(yieldTime);
            
        }
    }



    void OnChangeAbilityPower(float newAbilityPower)
    {
        AbilityPower = newAbilityPower;
    }

    void OnChangeAttackDamage(float newAttackDamage)
    {
        AttackDamage = newAttackDamage;
    }

    void OnChangeAttackSpeed(float newAttackSpeed)
    {
        AttackSpeed = newAttackSpeed;
    }

    void OnChangeCriticalStrikeChance(float newCriticalStrikeChance)
    {
        CriticalStrikeChance = newCriticalStrikeChance;
    }

    void OnChangeCriticalStrikeMultiplier(float newCritMultiplier)
    {
        CriticalStrikeMultiplier = newCritMultiplier;
    }

    void OnChangeArmorPenetration(float newArmorPen)
    {
        ArmorPenetration = newArmorPen;
    }

    void OnChangeMagicPenetration(float newMpen)
    {
        MagicPenetration = newMpen;
    }

    void OnChangeLifeSteal(float newLifeSteal)
    {
        LifeSteal = newLifeSteal;
    }

    void OnChangeHealthRegen(float newHpRegen)
    {
        HealthRegenerationPerSecond = newHpRegen;
    }

    void OnChangeGold(int newGold)
    {
        Gold = newGold;
    }




}
