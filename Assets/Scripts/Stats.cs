using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public int ManaRegeneration = 2;
    public int Mana = 400;
    [SyncVar(hook = "OnChangeMana")]
    public int CurrentMana = 400;


    //Other
    public float Experience = 0f;
    [SyncVar(hook = "OnChangeGold")]
    public int Gold = 0;
    public int GoldGenerationPerSecond = 1;
    public float MovementSpeed = 10f;
    public float Range = 10f;


    public Image manaFillImg;
    public Text manaText;

    Image localManaFill;
    Text localManaText;


    private void Start()
    {
        healthReference = GetComponent<Health>();
        if (manaText != null) manaText.text = CurrentMana + "/" + Mana;
        if (isLocalPlayer)
        {
            localManaFill = GameObject.Find("LocalMpFillImg").GetComponent<Image>();
            localManaText = GameObject.Find("LocalMpText").GetComponent<Text>();
            localManaText.text = CurrentMana + "/" + Mana; 
        }

        StartCoroutine(StatRegenerationPerSecond(1f));
    }

    IEnumerator StatRegenerationPerSecond(float yieldTime)
    {
        while (true)
        {

            if (CurrentMana < Mana)
            {
                CurrentMana += ManaRegeneration;
                if (CurrentMana > Mana) CurrentMana = Mana;
            }
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

    void OnChangeMana(int newMana)
    {
        CurrentMana = newMana;
        manaFillImg.fillAmount = (float)CurrentMana / Mana;
        manaText.text = CurrentMana + "/" + Mana;

        if (isLocalPlayer)
        {
            if (localManaFill == null) return;
            localManaFill.fillAmount = (float)CurrentMana / Mana;
            localManaText.text = CurrentMana + "/" + Mana;
        }
    }

    public bool UtilizeMana(int amount)
    {
        if (CurrentMana >= amount)
        {
            //CurrentMana -= amount;
            CmdUtilizeMana(amount);
            return true;
        }
        return false;
    }


    [Command]
    void CmdUtilizeMana(int amount)
    {
        CurrentMana -= amount;
    }

}
