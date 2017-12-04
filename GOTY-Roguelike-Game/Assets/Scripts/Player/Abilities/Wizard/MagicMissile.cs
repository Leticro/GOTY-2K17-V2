﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : MonoBehaviour
{
    ParticleSystem missileParticle;
    ParticleSystem explosionParticle;
    float timer;
    float damage;
    float missileRadius;
    float damageRadius;
    float particleRadius;
    bool exploded;

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public float DamageRadius
    {
        get { return damageRadius; }
        set { damageRadius = value; }
    }
    public float MissileRadius
    {
        get { return missileRadius; }
        set { missileRadius = value; }
    }

    public float ParticleRadius
    {
        get { return particleRadius; }
        set { particleRadius = value; }
    }

    public float Timer
    {
        get { return timer; }
        set { timer = value; }
    }

    private void Start()
    {
        missileParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        explosionParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        StartCoroutine(Active());


        // change particleEffect size for to match object radiuses
        ParticleSystem[] pSystems = missileParticle.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem pSys in pSystems)
        {
            ParticleSystem.MainModule mainEffect = pSys.main;
            mainEffect.startSize = missileRadius * mainEffect.startSize.constant;
        }

        pSystems = explosionParticle.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem pSys in pSystems)
        {
            ParticleSystem.MainModule mainEffect = pSys.main;
            mainEffect.startSize = damageRadius * mainEffect.startSize.constant / 2f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster") || other.gameObject.layer == LayerMask.NameToLayer("Unwalkable") ||
            other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            StartCoroutine(Explode());
        }
    }

    IEnumerator Active()
    {
        yield return new WaitForSeconds(timer);
        if(!exploded) StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        exploded = true;
        missileParticle.transform.gameObject.SetActive(false);
        GetComponent<Rigidbody>().velocity = new Vector3();

        explosionParticle.transform.position = this.transform.position;
        ParticleSystem.MainModule mainSystem = explosionParticle.main;
        mainSystem.startSize = particleRadius;
        explosionParticle.gameObject.SetActive(true);

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, damageRadius);
        foreach (Collider collider in colliders)
        {
            RigCollider rigCollider = collider.gameObject.GetComponent<RigCollider>();
            if (rigCollider != null && rigCollider.RootUnit is AggressiveUnit)
            {
                AggressiveUnit monster = ((AggressiveUnit)rigCollider.RootUnit);
                float damage = this.damage;
                monster.Damage(damage);
            }
        }

        yield return new WaitWhile(() => explosionParticle.IsAlive(true));
        Destroy(this.gameObject);
    }
}