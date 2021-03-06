﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDamageAttack : Attack {

	public BasicDamageAttack(AttackController controller) : base(controller) {
	}

	public override void Use(Transform target) {
		HealthManager healthManager = Attacker.target.GetComponent<HealthManager>();
		if (healthManager != null) {
			//healthManager.Damage(Attacker.CalculateAttackPower());
		} else {
			Debug.LogError("Can't damage target without HealthManager");
		}
		Attacker.UnitAnimator.SetBool("Attack", true);
	}

}
