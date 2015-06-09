using UnityEngine;
using System.Collections;

/*
 * Falling Room Controller
 * The floor in this room starts falling after a determined number of seconds. Players must stand on props to survive.
 */
public class FallingRoomController : MonoBehaviour 
{
	public float secondsToTrigger = 5f;
	public GameObject floor;

	bool triggered;
	Rigidbody floorRigidBody;

	void Start() {
		triggered = false;
		floorRigidBody = floor.GetComponent<Rigidbody>();
	}

	void OnTriggerEnter(Collider other) {
		if (!triggered && other.tag == "PlayerBody") {
			triggered = true;
			StartCoroutine("CollapseFloor");
		}
	}

	IEnumerator CollapseFloor() {
		yield return new WaitForSeconds(secondsToTrigger);

		floor.transform.position -= new Vector3(0f, 0.9f, 0f); // floor is "stuck" inside other building blocks, so we need to free it first
		floorRigidBody.useGravity = true;
		floorRigidBody.constraints = RigidbodyConstraints.None;

		GetComponent<DoorsController>().TriggerDoors(true);
		this.enabled = false; // disable script, as everything has already been done

		Invoke ("DestroyFloor", 5f);
	}

	void DestroyFloor() {
		Destroy(floor);
	}
}
