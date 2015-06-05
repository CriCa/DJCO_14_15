using UnityEngine;
using System.Collections;

/* 
 * Doors Controller
 * Responsible for closing and opening doors on a room. Doors are automatically closed on trigger after a certain amount of time has passed.
 */
public class DoorsController : MonoBehaviour 
{
	public float secondsToTrigger = 3f;
	public float smoothing = 0.5f;
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

	IEnumerator CloseDoors() {
		yield return new WaitForSeconds(secondsToTrigger);

		Vector3 goal = new Vector3(270f, 180f, 0f);
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
