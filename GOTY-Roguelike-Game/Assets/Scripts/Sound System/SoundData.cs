﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundData : MonoBehaviour {

	public List<AudioClip> soundEffects;
	AudioSource source;
	public int directIndex;
	public int loopIndex;
	public float delay;

	public bool oneTime = false;

	//public float offset = 0.2f;

	GameObject mm;

	// Use this for initialization
	protected virtual void Start () {
		this.gameObject.AddComponent<AudioSource>();
		source = GetComponent<AudioSource> ();

		if (GameObject.Find ("MusicManager") != null) {
			mm = GameObject.Find ("MusicManager");
			source.volume = mm.GetComponent<MusicManager>().getVolume();
			//Debug.Log (source.volume);
		} else {
			source.volume = 1f;
		}
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (mm != null) {
			/*if (mm.GetComponent<MusicManager> ().getVolume () > offset) {
				source.volume = mm.GetComponent<MusicManager> ().getVolume () + offset;
			} else {
				source.volume = mm.GetComponent<MusicManager> ().getVolume ();
			}*/
			source.volume = mm.GetComponent<MusicManager> ().getVolume ();
		} else {
			source.volume = 1f;
		}
		//Debug.Log("THING" + mm.GetComponent<MusicManager> ().getVolume ());
		//Debug.Log ("SOUND EFFECTS"source.volume);
		
	}

	public virtual void playSound(int index){
		if (soundEffects [index] != null) {
			source.PlayOneShot (soundEffects [index]);
		}
	}

	public virtual void playSound(){
		if (soundEffects [directIndex] != null) {
			source.clip = soundEffects [directIndex];
			//source.PlayOneShot (soundEffects [directIndex]);
			source.PlayDelayed(delay);
		}
	}

	public virtual void playLoop(){
		if (soundEffects [loopIndex] != null) {
			source.loop = true;
			source.PlayOneShot (soundEffects [loopIndex]);
		}
	}

	public virtual void stopLoop(){
		source.Stop ();
		source.loop = false;
	}
}
