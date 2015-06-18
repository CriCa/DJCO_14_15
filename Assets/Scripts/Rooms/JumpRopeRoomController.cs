using UnityEngine;
using System.Collections;

/*
 * Jump Rope Controller
 * A "rope" appear and starts moving between walls. Players must jump to avoid it.
 */
public class JumpRopeRoomController : MonoBehaviour 
{
	public float secondsToTrigger = 6f;
	public GameObject rope;

	public float baseRopeSpeed = 3f;
	public AnimationCurve speedCurve;
	public int jumpsNeeded = 8; // number of jumps needed before the doors open

	private bool triggered;
	private float currentRopeSpeed;
	private int jumpsTaken; // number of times the rope has already moved
	

	void Start() {
		triggered = false;
		currentRopeSpeed = baseRopeSpeed;
		jumpsTaken = 0;
	}
	

	void OnTriggerEnter(Collider other) {
		if (!triggered && other.tag == "PlayerBody") {
			triggered = true;
			StartCoroutine("SwingRope");
		}
	}


	IEnumerator SwingRope() {
		yield return new WaitForSeconds(secondsToTrigger);

		// enable rope object
		rope.SetActive(true);	

		// the initial goal is to move towards the opposite wall
		bool reachedGoal = false;
		float percentage = 0f;

		Vector3 goal = rope.transform.localPosition;
		goal.z = -goal.z;

		while (!reachedGoal) {
			rope.transform.localPosition = Vector3.MoveTowards(rope.transform.localPosition, goal, Time.deltaTime * currentRopeSpeed);

			// once we've arrived at the opposite wall, we want to go back
			if (Vector3.Distance(rope.transform.localPosition, goal) < 0.01f) {
				goal.z = -goal.z;
				jumpsTaken++;

				// increase speed based on base speed and animation curve
				percentage = (float)jumpsTaken/(float)jumpsNeeded;
				currentRopeSpeed = baseRopeSpeed * speedCurve.Evaluate(percentage);
			}

			// once the rope has moved enough times, it can stop
			if (jumpsTaken >= jumpsNeeded) {
				reachedGoal = true;
			}

			yield return null;
		}

		// disable rope object and open doors
		rope.SetActive(false);
		GetComponent<DoorsController>().TriggerDoors(true);
		Destroy(this);
	}
}
