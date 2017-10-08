﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : AggressiveUnit {

	protected new void Start() {
		base.Start();
		attacks.Add(new TestAttack(
			new IntervalAttackController(2, 3)
				.AddConditional(() => GetHealthPercentage() > 0.5f)
		));
		attacks.Add(new TestAttack(
			new HealthAttackController(this, 0.4f)
		));
	}

}