using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * 
 * Skill types: 
 * 1) Passive
 * 2) Active 
 * 
 * Offensive, Defensive, Utility
 * 
 * 
 * 
 * */

public class Skill : NetworkBehaviour {

    public KeyCode skillKeycode = KeyCode.Q;
    public bool HasAnimation = true;
    public bool hasParticleEffect = true;

    public float coolDownInSeconds = 2.0f;

    protected bool isCasting;

    protected Animator animator;
    protected int damage;

    public int manaCost = 30;

    private void Start()
    {
        if (HasAnimation) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(skillKeycode))
        {
            StartCoroutine(CastSkill());
        }
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

            damage = (int)GetComponent<Stats>().AttackDamage;

            print("skill dmg: " + damage);

            if (SkillEffect()
                && GetComponent<Stats>().UtilizeMana(manaCost))
            {
                SkillAnimation();
                SkillParticle();
                yield return new WaitForSeconds(coolDownInSeconds);
            }
            isCasting = false;
        }

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


}
