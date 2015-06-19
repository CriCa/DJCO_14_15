using UnityEngine;
using System.Collections;

[RequireComponent(typeof (BoxCollider))]
public class FallingSpikesController : MonoBehaviour 
{
	public float speed = 3f;
	public float distanceToFloor = 3.3f;

	private bool moving = false;
	private Vector3 initialPosition;
	private Vector3 floorPosition;


	void Start() {
		initialPosition = transform.localPosition;
		floorPosition = transform.localPosition - new Vector3(0f, distanceToFloor, 0f);
	}


	public void StartSmashProcess() {
		StopCoroutine("Smash");
		StartCoroutine("Smash");
	}


	IEnumerator Smash() {
		bool arrivedAtGoal = false;
		Vector3 currentGoal = floorPosition;

		while (!arrivedAtGoal) {
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentGoal, Time.deltaTime * speed);

			if (transform.localPosition == currentGoal) {
				// if our goal is getting to the floor, we should go back
				if (currentGoal == floorPosition) {
					currentGoal = initialPosition;
				}

				// otherwise, we're back where we started and we can stop
				else if (currentGoal == floorPosition) {
					arrivedAtGoal = true;
				}
			}

			yield return null;
		}
	}
}
