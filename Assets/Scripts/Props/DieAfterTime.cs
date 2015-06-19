using UnityEngine;
using System.Collections;

public class DieAfterTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<ParticleSystem> ().playbackSpeed = 3;
		Invoke ("DestroyParticles", 3.0f);
	}
	
	// Update is called once per frame
	void DestroyParticles () {
		Destroy (gameObject);
	}
}
