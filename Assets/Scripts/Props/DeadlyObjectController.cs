using UnityEngine;
using System.Collections;

/* 
 * Deadly Object Controller
 * Destroys player on touch. Must have a box collider with "isTrigger" set attached.
 */
public class DeadlyObjectController : MonoBehaviour 
{
	void OnTriggerEnter(Collider other) {
		if (other.tag == "PlayerBody") {
			Debug.Log("kill player");
		}
	}
}
