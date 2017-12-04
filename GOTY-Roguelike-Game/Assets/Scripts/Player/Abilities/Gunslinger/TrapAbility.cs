﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAbility : Ability
{
    public float trapDuration = 300f;
    public float trapDiameter = 1f;
    public float slowDuration = 5f;
    public float slowPercent = .7f;
    public int durability = 5;

    Trap trapObject;

    protected override void Start()
    {
        base.Start();
        applyOnFrame = true;
        trapObject = this.transform.GetChild(0).GetComponent<Trap>();
		sfx = GetComponent<SoundData> ();
    }

    public override void applyEffect(GameObject player)
    {
        base.applyEffect(player);

        Trap trap = Instantiate(trapObject);
        trap.Duration = trapDuration;
        trap.Diameter = trapDiameter;
        trap.SlowDuration = slowDuration;
        trap.SlowPercent = slowPercent;
        trap.Durability = durability;
        trap.transform.position = player.transform.position;
        trap.transform.localScale = new Vector3(trapDiameter, 1, trapDiameter);
        trap.gameObject.SetActive(true);
        GameObject particleSys = trap.transform.GetChild(0).gameObject;
        ParticleSystem.MainModule particleMain = particleSys.GetComponent<ParticleSystem>().main;
        particleMain.startSize = particleMain.startSize.constant * trapDiameter;
        particleSys.SetActive(true);
    }
}