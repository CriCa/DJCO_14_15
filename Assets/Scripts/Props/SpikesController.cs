using UnityEngine;
using System.Collections;

/* 
 * Spikes Controller
 * Destroys player on touch. Must have a box collider with "isTrigger" set attached.
 */
public class SpikesController : MonoBehaviour 
{
	void OnTriggerEnter(Collider other) {
		if (other.tag == "PlayerBody") {
			Debug.Log("Player should die now.");
		}
	}
}
