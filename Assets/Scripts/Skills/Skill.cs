using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
/*
 * Skill types: 
 * 1) Passive
 * 2) Active 
 * Offensive, Defensive, Utility
 * */

[RequireComponent(typeof(PlayerController))]
public class Skill : NetworkBehaviour {

    public bool HasAnimation = true;
    public bool hasParticleEffect = true;

    public float coolDownInSeconds = 2.0f;

    protected bool isCasting;

    public Animator animator;

    public int manaCost = 30;

    public enum SkillKeyCode { Q, W, E, R };
    public SkillKeyCode chosenKeyCode = SkillKeyCode.Q;
    public KeyCode currentKeyCode;

    Image skillFillImg;
    Text SkillCooldownText;

    private void Start()
    {
        if (HasAnimation) animator = GetComponent<Animator>();


        if(!isLocalPlayer)
        {
            enabled = false;
            return;
        }


        string heroName = GetComponent<PlayerController>().HeroName;
        Image skillSprite;


        switch (chosenKeyCode)
        {
            case SkillKeyCode.Q:
                currentKeyCode = KeyCode.Q;
                skillSprite = GameObject.Find("Skill_Q").GetComponent<Image>();
                skillFillImg = GameObject.Find("Skill_Q_Fill").GetComponent<Image>();
                SkillCooldownText = GameObject.Find("Skill_Q_Text").GetComponent<Text>();
                skillSprite.sprite = Resources.Load<Sprite>("AbilityIcons/" + heroName + "Q");
                break;
            case SkillKeyCode.W:
                currentKeyCode = KeyCode.W;
                skillSprite = GameObject.Find("Skill_W").GetComponent<Image>();
                skillFillImg = GameObject.Find("Skill_W_Fill").GetComponent<Image>();
                SkillCooldownText = GameObject.Find("Skill_W_Text").GetComponent<Text>();
                skillSprite.sprite = Resources.Load<Sprite>("AbilityIcons/" + heroName + "W");
                break;
            case SkillKeyCode.E:
                currentKeyCode = KeyCode.E;
                skillSprite = GameObject.Find("Skill_E").GetComponent<Image>();
                skillFillImg = GameObject.Find("Skill_E_Fill").GetComponent<Image>();
                SkillCooldownText = GameObject.Find("Skill_E_Text").GetComponent<Text>();
                skillSprite.sprite = Resources.Load<Sprite>("AbilityIcons/" + heroName + "E");
                break;
            case SkillKeyCode.R:
                currentKeyCode = KeyCode.R;
                skillSprite = GameObject.Find("Skill_R").GetComponent<Image>();
                skillFillImg = GameObject.Find("Skill_R_Fill").GetComponent<Image>();
                SkillCooldownText = GameObject.Find("Skill_R_Text").GetComponent<Text>();
                skillSprite.sprite = Resources.Load<Sprite>("AbilityIcons/" + heroName + "R");
                break;
            default:
                currentKeyCode = KeyCode.Q;
                skillFillImg = GameObject.Find("Skill_Q_Fill").GetComponent<Image>();
                SkillCooldownText = GameObject.Find("Skill_Q_Text").GetComponent<Text>();
                skillSprite = GameObject.Find("Skill_Q").GetComponent<Image>();
                skillSprite.sprite = Resources.Load<Sprite>("AbilityIcons/" + heroName + "Q");
                break;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(currentKeyCode)) StartCoroutine(CastSkill());
    }



    protected virtual void SkillAnimation()
    {
        if (!HasAnimation) return;
    }

    protected virtual void SkillParticle()
    {
        if (!hasParticleEffect) return;
    }


    protected virtual bool SkillEffect()
    {
        return true;
    }

    protected IEnumerator CastSkill()
    {
        if (!isCasting)
        {
            isCasting = true;

            if (GetComponent<Stats>().UtilizeMana(manaCost) && SkillEffect())
            {
                SkillAnimation();
                SkillParticle();

                CoolDownStartTime = Time.time;
                isOnCoolDown = true;
                StartCoroutine(VisualizeCoolDown());
                yield return new WaitForSeconds(coolDownInSeconds);
                isOnCoolDown = false;
            }
            isCasting = false;
        }

    }

    float CoolDownStartTime;
    bool isOnCoolDown;

    protected IEnumerator VisualizeCoolDown()
    {
        skillFillImg.fillAmount = 1f;
        while (isOnCoolDown)
        {
            skillFillImg.fillAmount = 1f - ((Time.time - CoolDownStartTime) / coolDownInSeconds);
            SkillCooldownText.text = "" + (int)(coolDownInSeconds - (Time.time - CoolDownStartTime));
            yield return new WaitForSeconds(0.1f);
        }
        SkillCooldownText.text = "";
        skillFillImg.fillAmount = 0f;
    }


    [Command]
    protected void CmdTriggerAttackAnimation(string animationParameterName)
    {
        RpcTriggerAttackAnimation(animationParameterName);
    }

    [ClientRpc]
    protected void RpcTriggerAttackAnimation(string animationParameterName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationParameterName)) animator.SetTrigger(animationParameterName);
    }


    private void OnDestroy()
    {
        SkillCooldownText.text = "";
        skillFillImg.fillAmount = 0f;
    }
}
