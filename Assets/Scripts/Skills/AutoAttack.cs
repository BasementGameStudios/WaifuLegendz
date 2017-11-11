using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(Animator))]
public class AutoAttack : NetworkBehaviour {

    protected Stats currentStatReference;

    public float attackSpeed = 1.0f;
    protected Animator animator;
    protected float nextTimeToFire = 0f;
    protected bool isAttacking;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentStatReference = GetComponent<Stats>();
    }


    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(0)){
            BasicAttack();
        }

        if (Input.GetMouseButton(1))
        {
            isAttacking = false;
        }
    }

    // Simple damage function inheriting scripts can use. 
    [Command]
    protected void CmdDamage(GameObject other, int damage)
    {
        other.GetComponent<Health>().RpcTakeTrueDamage(damage);
    }

    protected virtual IEnumerator ApplyBasicAttack(GameObject other, int damage)
    {

        while (true && isAttacking)
        {
            if (Time.time >= nextTimeToFire)
            {
                if ( other == null || Vector3.Distance(transform.position, other.transform.position) > currentStatReference.Range)
                {
                    break;
                }
                nextTimeToFire = Time.time + 1f / attackSpeed;
                transform.LookAt(other.transform);
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
                {
                    CmdTriggerAttackAnimation();//animator.SetTrigger("attack");
                }
                CmdDamage(other, damage);
            }
            yield return null;
        }
    }

    protected virtual void BasicAttack()
    {
        Ray intersectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit interactionHitInfo; //raycast hit object
        if (Physics.Raycast(intersectionRay, out interactionHitInfo, Mathf.Infinity))
        {
            if ((interactionHitInfo.collider.gameObject.layer != LayerMask.NameToLayer("LocalPlayer") && interactionHitInfo.collider.tag == "Player")
                ||interactionHitInfo.collider.tag == "Dragon"
                || interactionHitInfo.collider.tag == "Minion"
                || interactionHitInfo.collider.tag == "Tower"
                || interactionHitInfo.collider.tag == "Nexus"
                )
            {
                if (Vector3.Distance(transform.position, interactionHitInfo.transform.position) <= currentStatReference.Range)
                {
                    isAttacking = true;
                    StartCoroutine(ApplyBasicAttack(interactionHitInfo.collider.gameObject, (int)currentStatReference.AttackDamage));
                }

            }
            else
            {
                isAttacking = false;
            }
        }
    }


    [Command]
    protected void CmdTriggerAttackAnimation()
    {
        RpcTriggerAttackAnimation();
    }

    [ClientRpc]
    protected void RpcTriggerAttackAnimation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack")) animator.SetTrigger("attack");
    }

}
