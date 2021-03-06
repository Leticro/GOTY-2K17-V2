﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerData : WeaponData
{
    public float daggerRange = 1f;
    public float daggerRadius = 2f;
    public float hitsPerSecond = 10f;

    float timeSinceLastAttack = 0f;
    Vector3 leftDaggerRotation = new Vector3(-35, 90, -100);
    Vector3 leftDaggerPosition = new Vector3(-0.25f, -0.04f, 0.1f);
    Transform leftHand;
    GameObject secondDagger;

    protected void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
    }

    public override void Attack()
    {
        if (timeSinceLastAttack >= 1f / hitsPerSecond)
        {
            //PlayAttackAudio(0);
            timeSinceLastAttack = 0f;
            SlashAttack();
        }
    }
    void SlashAttack() {
        ThirdPersonCharacter player = this.GetComponentInParent<ThirdPersonCharacter>();
		Collider[] colliders = Physics.OverlapCapsule(player.transform.position + player.transform.forward * daggerRange,
			player.transform.position + player.transform.up*2f + player.transform.forward * daggerRange, daggerRadius);
        
        foreach (Collider collider in colliders)
        {
            RigCollider rigCollider = collider.gameObject.GetComponent<RigCollider>();
            if (rigCollider != null && !(rigCollider is AttackCollider) && rigCollider.RootUnit is AggressiveUnit)
            {
                AggressiveUnit monster = ((AggressiveUnit)rigCollider.RootUnit);
                float damage = this.damage;
                damage *= damageMultiplier;
                damage = WeaponEmotionActionHandler.GetOnDamageAction(emotion)(this, monster, damage);
                damage = WeaponModifierActionHandler.GetOnDamageAction(modifier)(this, monster, damage);
                monster.Damage(damage, Player.transform);
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (secondDagger != null)
        {
            Destroy(secondDagger);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(WaitForDrop());

        if (this.tag == "Equipped")
        {
            switch (emotion)
            {
                case WeaponEmotion.Inspiration:
                    hitsPerSecond *= 1.3f; // 30% hit rate increase
                    break;
                default: break;
            }

            // spawn second dagger in hand
            this.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            leftHand = this.GetComponentInParent<ThirdPersonCharacter>().leftHand.transform;
            secondDagger = Instantiate(this.transform.GetChild(0).gameObject, leftHand, false);
            secondDagger.transform.localPosition = leftDaggerPosition;
            secondDagger.transform.localEulerAngles = leftDaggerRotation;
            // if dagger has particle effect on second child
            if (transform.childCount > 1)
            {
                GameObject secondParticle = Instantiate(this.transform.GetChild(1).gameObject, secondDagger.transform, false);
                secondParticle.transform.localPosition = new Vector3(0, .5f, 0);
            }
        }
    }

    protected override IEnumerator WaitForDrop()
    {
        yield return new WaitWhile(() => this.tag == "Equipped");
        OnDisable();
    }

    private void OnDestroy()
    {
        Destroy(secondDagger);
    }
}
