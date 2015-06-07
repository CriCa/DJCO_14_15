using UnityEngine;
using System.Collections;

public class FakeMonstersRoomController : MonoBehaviour 
{
	void OnTriggerEnter(Collider other) {
		if (other.tag == "PlayerBody") {
			Debug.Log ("entered");
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "PlayerBody") {
			Debug.Log ("left");
		}
	}
}
