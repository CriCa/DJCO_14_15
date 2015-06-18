using UnityEngine;
using System.Collections;

[RequireComponent(typeof (BoxCollider))]
public class MonsterAttackController : MonoBehaviour 
{
	public float damage = 2f;


	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			other.gameObject.GetComponent<PlayerNetworkManager>().TakeDamage(damage);
		}
	}
}
