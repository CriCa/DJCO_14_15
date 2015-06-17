using UnityEngine;
using System.Collections;

/* 
 * Shooting Controller
 * Listens to user input in order to shoot SOMETHING.
 */
public class ShootingController : MonoBehaviour 
{
	public GameObject impact;
	public float cooldown = 1f;
	public float maxShootingDistance = 50f;

	private PlayerNetworkManager playerNetManager;
	private GameObject impactObj;
	private bool shooting;
	private float nextShot;


	void Start() {
		playerNetManager = transform.parent.parent.GetComponent<PlayerNetworkManager>();

		shooting = false;
		nextShot = 0f;

		// spawning at start somewhere it can't be seen so it can simply be moved later largely improves performance
		impactObj = Instantiate(impact, new Vector3(3f, 12f, 18f), Quaternion.identity) as GameObject;
	}


	void Update() {
		if (Input.GetButtonDown("Fire1") && Time.time > nextShot) {
			shooting = true;
			nextShot = Time.time + cooldown;
		}
	}


	void FixedUpdate() {
		if (shooting) {
			shooting = false;
			playerNetManager.TriggerAnimation("Attack");

			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, maxShootingDistance)) {
				// if player is hit, should take damage
				if (hit.transform.tag == "Player") {
					Debug.Log("Player hurt.");
				}
				// otherwise, draw impact particles
				else {
					impactObj.transform.position = hit.point;
				}
			}
		}
	}
}
