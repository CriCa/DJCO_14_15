using UnityEngine;
using System.Collections;

/* 
 * Deadly Object Controller
 * Destroys player on touch. Must have a box collider with "isTrigger" set attached.
 */
public class DeadlyObjectController : MonoBehaviour 
{
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			// do something here...

			// the respawn process should only happen on the actual local player in order to avoid syncing problems
			if (other.gameObject == NetworkManager.instance.GetPlayer()) {
				NetworkManager.instance.RespawnPlayer();
			}
		}
	}
}
