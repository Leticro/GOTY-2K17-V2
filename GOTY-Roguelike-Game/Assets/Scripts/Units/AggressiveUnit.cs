﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AggressiveUnit : LivingUnit {

	[TagSelector]
	public string targetTag = "Player";
	public float aggroRadius = 20f;

	public List<Attack> attacks = new List<Attack>();

	public float attackPower = 5f;

	public SoundData sfx;

	public bool HasWeapon { get; set; }

	protected override void Start() {
		base.Start();
		ApplyAttackBehavior();
	}

	protected virtual new void Update() {
		base.Update();
		if (target == null) { // Only search if no target already
			CheckAggro();
		} else {
			ThirdPersonCharacter tpc = target.GetComponent<ThirdPersonCharacter>();
			if (tpc != null) {
				if (tpc.IsInvisible) {
					CancelAggro();
				}
			}
		}

		UpdateAttacks();
	}

	public void CancelAggro() {
		target = null;
		atGoal = false;
		StopCoroutine("FollowPath");
		if (UnitAnimator != null) {
			UnitAnimator.SetBool("Move", false);
		}
	}

	private void CheckAggro() {
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, aggroRadius);
		foreach (Collider collider in hitColliders) {
			if (collider.tag.Equals(targetTag) && HasLineOfSight(collider.transform, aggroRadius)) {
				SetAggro(collider.transform);
				break;
			}
		}
	}

	public void SetAggro(Transform target, bool ignoreSound=false) {
		ThirdPersonCharacter tpc = target.GetComponent<ThirdPersonCharacter>();
		if (tpc != null && tpc.IsInvisible) {
			return;
		}
		this.target = target;
		BeginPathing();
		if (sfx != null && sfx.soundEffects.Count > 0 && !sfx.oneTime) {
			sfx.playSound();
			sfx.oneTime = true;
		}
	}

	public void AlertNeighbors(Transform target) {
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, aggroRadius);
		foreach (Collider collider in hitColliders) {
			RigCollider rigCollider = collider.GetComponent<RigCollider>();
			if (rigCollider != null
				&& rigCollider.RootUnit is AggressiveUnit
				&& !(rigCollider.RootUnit is BossUnit)) {
				AggressiveUnit monster = rigCollider.RootUnit as AggressiveUnit;
				monster.SetAggro(target, true);
			}
		}
	}

	private void UpdateAttacks() {
		if (!IsStunned()) {
			foreach (Attack attack in attacks) {
				if ((!atGoal && attack.Controller.RequireGoal) // Does the controller require a goal?
						|| (!Living && !(attack.Controller is DeathAttackController))) { // Is the monster dead?
					continue;
				}
				if (attack.Controller.Check()) {
					attack.Use(target);
				}
			}
		}
	}

	protected virtual new void OnDrawGizmosSelected() {
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, aggroRadius);
	}

	protected abstract void ApplyAttackBehavior();

	protected override void UpdateAnimator() {
		base.UpdateAnimator();
	}

	public float CalculateAttackPower() {
		return attackPower;
	}

	public override void OnRigTriggerEnter(Collider collider) {
		if (HasWeapon) { // Damage is done by weapon collider if it exists
			return;
		}
		AttackCollision(collider);
	}

	public void OnWeaponTriggerEnter(Collider collider) {
		AttackCollision(collider);
	}

	bool recentlyAttacked = false;

	private void AttackCollision(Collider collider) {
		if (recentlyAttacked) {
			return;
		}
		if (collider.gameObject.tag == "Player") {
			HealthManager healthManager = collider.gameObject.GetComponent<HealthManager>();
			if (healthManager == null) {
				return;
			}
			if (this.UnitAnimator.GetBool("Attack")) {
				healthManager.Damage(CalculateAttackPower());
				recentlyAttacked = true;
				StartCoroutine("ResetAttack");
			} else if (this.UnitAnimator.GetBool("SpecialAttack")) {
				healthManager.Damage(CalculateAttackPower()); // TODO Possibly add damage modifiers for special attacks
			}
		}
	}

	IEnumerator ResetAttack() {
		yield return new WaitForSeconds(1.5f);
		recentlyAttacked = false;
		StopCoroutine("ResetAttack");
		yield return null;
	}

	public override void Damage(float amount, Transform attacker) {
		base.Damage(amount, attacker);
		SetAggro(attacker);
		AlertNeighbors(attacker);
	}

}
