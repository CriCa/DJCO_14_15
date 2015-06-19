using UnityEngine;
using System.Collections;

public class FinalRoomController : MonoBehaviour 
{
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			if (other.gameObject == NetworkManager.instance.GetPlayer()) {
				other.transform.GetComponent<PlayerNetworkManager>().EndSession();
			}
		}
	}
}
