﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityData : MonoBehaviour {

	public float damage;
	public ParticleSystem effect;
	public Vector3 effectPos;

	// Use this for initialization
	void Start () {
		//effect = Instantiate (effect);
		//effect.transform.SetParent (gameObject.transform);
		effect.transform.position = effectPos;
		//effect.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public abstract void applyEffect();

}