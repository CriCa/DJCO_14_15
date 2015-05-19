using UnityEngine;
using System.Collections;

/* 
 * Flashlight Controller
 * Listens to user input in order to toggle a spotlight.
 */
public class FlashlightController : MonoBehaviour
{
	public Light spotlight; // flashlight's light
	public KeyCode activationKey = KeyCode.E; // keypress needed to toggle the light
	public Transform target; // flashlight direction should follow the main camera

	void Update () {
		transform.rotation = target.rotation;

		if (Input.GetKeyDown(activationKey)) {
			spotlight.enabled = !spotlight.enabled;
		}
	}
}
