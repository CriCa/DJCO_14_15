using UnityEngine;
using System.Collections;

public class RandomTests : MonoBehaviour 
{
	void Start () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		Debug.Log("ola " + other.tag);
	}
}
