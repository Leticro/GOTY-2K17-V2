﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunData : WeaponData {

	public Transform shootPoint;

	public float range = 20f;
	public float bulletsPerSecond = 10f;

	private float timeSinceLastShot = 0f;

	private LineRenderer line;

	protected override void Start() {
		base.Start();
		this.line = GetComponent<LineRenderer>();
	}

	protected void Update() {
		timeSinceLastShot += Time.deltaTime;
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
                    bulletsPerSecond *= 1.3f; // 30% hit rate increase
                    break;
                default: break;
            }
        }
    }

    public override void Attack() {
		if (timeSinceLastShot >= 1f / bulletsPerSecond) {
			PlayAttackAudio(0);
			timeSinceLastShot = 0f;
			RaycastShoot();
		}
	}

	private void RaycastShoot() {
		StartCoroutine(ShotEffect());

		RaycastHit hit;
		this.line.SetPosition(0, shootPoint.position);
		int layerMask = LayerMask.GetMask("Unwalkable", "Monster"); // Only collisions in layer Unwalkable and Monster
        // raycast from 1 unit in front of camera and forward
		if (Physics.Raycast(Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.forward, out hit, range, layerMask)) {
			this.line.SetPosition(1, hit.point);
			RigCollider rigCollider = hit.transform.GetComponent<RigCollider>();
			if (rigCollider == null) {
				return;
			}
			Unit unit = rigCollider.RootUnit;
			if (unit is AggressiveUnit) {
				AggressiveUnit monster = (AggressiveUnit)unit;
				float damage = this.damage;
                damage *= damageMultiplier;
                damage = WeaponEmotionActionHandler.GetOnDamageAction(emotion)(this, monster, damage);
				damage = WeaponModifierActionHandler.GetOnDamageAction(modifier)(this, monster, damage);
				monster.Damage(damage, Player.transform);
				//PlayAttackAudio (1);
			}
			//if (hit.rigidbody != null) {
			//	hit.rigidbody.AddForce(-hit.normal * hitForce);
			//}
		} else {
			this.line.SetPosition(1, shootPoint.position + (Camera.main.transform.forward * range));
		}
	}

	private IEnumerator ShotEffect() {
		line.enabled = true;
		yield return new WaitForSeconds(0.1f);
		line.enabled = false;
	}

}
