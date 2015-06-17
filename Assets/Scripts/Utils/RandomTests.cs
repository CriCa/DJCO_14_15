using UnityEngine;
using System.Collections;

public class RandomTests : MonoBehaviour 
{
	private Animator playerAnim;
	private Animator monsterAnim;


	void Start() {
		StartCoroutine("TransformIntoMonster");
	}


	IEnumerator TransformIntoMonster() {
		yield return new WaitForSeconds(8f);

		// hide player model
		playerAnim = GetComponent<Animator>();

		Transform player = transform.Find("PlayerModel");
		player.GetComponent<SkinnedMeshRenderer>().enabled = false;

		// set monster visible
		Transform monster = transform.Find("MonsterModel");
		monster.gameObject.SetActive(true);
		monsterAnim = monster.GetComponent<Animator>();

		monster.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
		monsterAnim.enabled = true;

		StartCoroutine("UpdateMonsterAnim");
	}


	IEnumerator UpdateMonsterAnim() {
		while(true) {
			bool running = playerAnim.GetBool("Walking") || playerAnim.GetBool("Running");

			monsterAnim.SetBool("Running", running);

			yield return null;
		}
	}
}
