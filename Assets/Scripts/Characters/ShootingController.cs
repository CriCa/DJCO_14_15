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
	public float damage = 1f;

	private PlayerNetworkManager playerNetManager;
	private bool shooting;
	private float nextShot;


	void Start() {
		playerNetManager = transform.parent.parent.GetComponent<PlayerNetworkManager>();

		shooting = false;
		nextShot = 0f;
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
					hit.transform.GetComponent<PlayerNetworkManager>().TakeDamage(damage);
				}
			}
		}
	}
}
