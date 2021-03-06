﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardKickAbility : Ability {

    public float damage;
	public float damageRadius;
    public Vector2 effectPosition;
    public float bonusDamage = 10f;
    public float bonusStunDuration = 3f;

    ParticleSystem particleEffect;

    protected override void Start(){
        base.Start();
        applyOnFrame = true;
		sfx = GetComponent<SoundData> ();

        particleEffect = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        // change particleEffect size for to match damage radius
        ParticleSystem[] pSystems = particleEffect.GetComponentsInChildren<ParticleSystem>();

        // took the time to adjust the values in particle effect to scale with damage
        ParticleSystem.MainModule mainEffect = pSystems[0].main;
        mainEffect.startSize = damageRadius * mainEffect.startSize.constant;
        mainEffect = pSystems[1].main;
        mainEffect.startSize = damageRadius * mainEffect.startSize.constant;
        mainEffect = pSystems[3].main;
        mainEffect.startSize = damageRadius * mainEffect.startSize.constant;
    }

    public override void applyEffect(GameObject player)
    {
        base.applyEffect(player);

        this.transform.position = player.transform.position + player.transform.forward * effectPosition.x + player.transform.up*effectPosition.y;

        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach(Collider collider in colliders)
        {
			RigCollider rigCollider = collider.gameObject.GetComponent<RigCollider> ();
			Debug.Log (collider);
			if (rigCollider != null && rigCollider.RootUnit is AggressiveUnit) {
				AggressiveUnit monster = ((AggressiveUnit)rigCollider.RootUnit);
				float damage = this.damage;
                if(bonusEffect)
                {
                    monster.ApplyStatus(new StatusStun(monster, bonusStunDuration));
                    damage += bonusDamage;
                }
                monster.ApplyKnockback(10f);
				monster.Damage(damage, player.transform);
			}
        }
		//sfx.playSound ();
        particleEffect.Play();
    }
}
