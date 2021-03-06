﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDragon : BossUnit {

	public Transform flameThrowerOriginPoint;
	public GameObject explosionPillar;

	protected override void Start(){
		base.Start ();
		sfx = GetComponent<SoundData> ();
	}

	protected override void ApplyAttackBehavior() {
		attacks.Add(new BasicDamageAttack(
			new IntervalAttackController(this, 2, 2)
		));
		attacks.Add(new FlamethrowerAttack(
			new IntervalAttackController(this, 5, 5)
				.AddConditional(CheckFlamethrower),
			flameThrowerOriginPoint
		));
		attacks.Add(new SpawnObjectAttack(
			new IntervalAttackController(this, 8, 8),
			explosionPillar,
			true
		));
	}

	private bool CheckFlamethrower() {
		if (target == null) {
			return false;
		}
		float dist = Vector3.Distance(this.transform.position, target.position);
		if (dist < 10f && dist > destinationRadius + 1) {
			return true;
		}
		return false;
	}

	protected void FlamethrowerStart() {
		flameThrowerOriginPoint.gameObject.SetActive(true);
	}

	protected override void AnimationComplete(AnimationEvent animationEvent) {
		base.AnimationComplete(animationEvent);
		if (animationEvent.stringParameter.Equals("reset_SpecialAttack")) {
			flameThrowerOriginPoint.gameObject.SetActive(false);
			this.speed = DefaultSpeed;
		}
	}

}
