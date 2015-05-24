using UnityEngine;
using System.Collections;

/*
 * Falling Room Controller
 * The floor in this room starts falling after a determined number of seconds. Players must stand on props to survive.
 * This script must be added to the floor game object.
 */
public class FallingRoomController : MonoBehaviour 
{
	public float fallingTime = 5f;

	bool falling;
	Rigidbody rigidBody;

	void Start() {
		falling = false;
		rigidBody = GetComponent<Rigidbody>();
	}

	void OnCollisionEnter(Collision collision) {
		Collider other = collision.collider;

		if (!falling && other.gameObject.tag == "PlayerBody") {
			falling = true;
			StartCoroutine("Fall");
		}
	}

	IEnumerator Fall() {
		yield return new WaitForSeconds(fallingTime);

		transform.position -= new Vector3(0f, 0.9f, 0f); // floor is "stuck" inside other building blocks, so we need to free it first
		rigidBody.useGravity = true;
		rigidBody.constraints = RigidbodyConstraints.None;

		this.enabled = false; // disable script, as everything has already been done
	}
}
