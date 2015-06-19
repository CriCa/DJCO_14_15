using UnityEngine;
using System.Collections;

[RequireComponent(typeof (BoxCollider))]
public class MonsterAttackController : MonoBehaviour 
{
	public float damage = 2f;

	public AudioClip sound1;
	public AudioClip sound2;
	public AudioClip sound3;

	private AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource> ();
		StartCoroutine ("PlaySound");
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			other.gameObject.GetComponent<PlayerNetworkManager>().TakeDamage(damage);
		}
	}

	IEnumerator PlaySound() {
		while (true) {
			yield return new WaitForSeconds ((float)(new System.Random ().NextDouble () * 5.0 + 3.0));

			float ch = (float)new System.Random ().NextDouble();

			if (ch > 0.66)
				audioSource.clip = sound1;
			else if (ch > 0.33)
				audioSource.clip = sound2;
			else audioSource.clip = sound3;

			audioSource.Play ();
		}
	}
}
