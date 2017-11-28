﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {
    
    public float cooldownTime;
    public bool faceCameraDirection;
    protected bool applyOnFrame;
    private float cooldownTimer;
    private bool isAvailible = true;

	// Use this for initialization
	protected virtual void Start () {
        cooldownTimer = cooldownTime;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if(!isAvailible)
        {
            cooldownTimer -= Time.deltaTime;
            if(CooldownTimer < 0f)
            {
                cooldownTimer = cooldownTime;
                isAvailible = true;
            }
        }
	}

    public bool ApplyOnFrame
    {
        get { return applyOnFrame; }
    }

    public float CooldownTimer
    {
        get { return cooldownTimer; }
        set { cooldownTimer = value; }
    }

    public bool IsAvailible
    {
        get { return isAvailible; }
        set { isAvailible = value; }
    }

    public virtual void applyEffect(GameObject player)
    {
        if(faceCameraDirection) player.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
    }
}
