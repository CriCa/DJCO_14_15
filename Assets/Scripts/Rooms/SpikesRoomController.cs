using UnityEngine;
using System.Collections;

/*
 * Spikes Room Controller
 * Two spiked walls slide down from the ceiling, encircling the players. Players must move quickly to survive.
 */
public class SpikesRoomController : MonoBehaviour 
{
	public Transform rightWall;
	public Transform leftWall;
	public float speed = 3f; // walls movement speed

	bool triggered;
	int currentPoint;
	float downDistance = 5.25f;
	float centerDistance = 6.8f;

	Vector3[] rightWallPoints; // each wall moves from point to point
	Vector3[] leftWallPoints;
	
	void Start() {
		triggered = false;

		rightWallPoints = new Vector3[2];
		rightWallPoints[0] = rightWall.position + new Vector3(0f, -downDistance, 0f);
		rightWallPoints[1] = rightWall.position + new Vector3(0f, -downDistance, centerDistance);

		leftWallPoints = new Vector3[2];
		leftWallPoints[0] = leftWall.position + new Vector3(0f, -downDistance, 0f);
		leftWallPoints[1] = leftWall.position + new Vector3(0f, -downDistance, -centerDistance);

		currentPoint = 0;
	}

	void OnTriggerEnter(Collider other) {
		if (!triggered && other.tag == "PlayerBody") {
			triggered = true;
			StartCoroutine("MoveWalls");
		}
	}

	IEnumerator MoveWalls() {
		bool reachedGoal = false;
		
		while (!reachedGoal) {
			// move walls towards respective point
			rightWall.position = Vector3.MoveTowards(rightWall.position, rightWallPoints[currentPoint], Time.deltaTime * speed);
			leftWall.position = Vector3.MoveTowards(leftWall.position, leftWallPoints[currentPoint], Time.deltaTime * speed);
			
			// once we've arrived at a point, we go towards the next
			if (Vector3.Distance(rightWall.position, rightWallPoints[currentPoint]) < 0.01f) {
				currentPoint++;
				
				// once we've moved towards all points, we can stop
				if (currentPoint >= rightWallPoints.Length) {
					reachedGoal = true;
				}
			}

			yield return null;
		}
	}
}
