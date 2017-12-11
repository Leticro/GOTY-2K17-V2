﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeAbility : Ability {

    public float bonusHealAmount;
    public float bonusHealRadius = 20f;

    protected override void Start()
    {
        base.Start();
		sfx = GetComponent<SoundData>();
    }

    public override void applyEffect(GameObject player)
    {
        base.applyEffect(player);

        if (bonusEffect)
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, bonusHealRadius);
            foreach (Collider collider in colliders)
            {
                RigCollider rigCollider = collider.gameObject.GetComponent<RigCollider>();
                Debug.Log(collider);
                if (rigCollider != null && rigCollider.RootUnit is AggressiveUnit)
                {
                    AggressiveUnit monster = ((AggressiveUnit)rigCollider.RootUnit);
                    if (bonusEffect)
                    {
                        player.GetComponent<HealthManager>().Heal(bonusHealAmount);
                        break;
                    }
                }
            }
        }
    }
		
}
