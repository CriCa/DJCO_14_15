using UnityEngine;
using System.Collections;

/* 
 * Doors Controller
 * Responsible for closing and opening doors on a room. Doors are automatically closed on trigger after a certain amount of time has passed.
 */
public class DoorsController : MonoBehaviour 
{
	public float secondsToTrigger = 3f;
	public float smoothing = 0.8f;
	public GameObject[] doors;

	bool triggered;

	void Start() {
		triggered = false;
	}
	
	void OnTriggerEnter(Collider other) {
		if (!triggered && other.tag == "PlayerBody") {
			triggered = true;
			StartCoroutine("CloseDoors");
		}
	}

	// provides external access to other scripts
	public void TriggerDoors(bool open) {
		Vector3 angle = new Vector3(270f, 0f, 0f);
		
		if (open) {
			angle.y = 90f;
		}
		else {
			angle.y = 180f;
		}

		// in case the doors are already moving, we should stop them first
		StopCoroutine("SwingDoors");
		StartCoroutine("SwingDoors", angle);
	}

	// used only internally, for the initial event
	IEnumerator CloseDoors() {
		yield return new WaitForSeconds(secondsToTrigger);

		TriggerDoors(false);
	}

	// the actual co-routine moving the doors is private
	IEnumerator SwingDoors(Vector3 goal) {
		bool reachedGoal = false;
		
		while (!reachedGoal) {
			foreach (GameObject door in doors) {
				// rotate locals towards goal
				if (Vector3.Distance(door.transform.localEulerAngles, goal) > 0.01f)
				{
					door.transform.localEulerAngles = Vector3.Lerp(door.transform.localEulerAngles, goal, Time.deltaTime * smoothing);
				}
				// once we're close enough, snap to goal and stop
				else
				{
					door.transform.localEulerAngles = goal;
					reachedGoal = true;
				}
			}
			
			yield return null;
		}
	}
}
