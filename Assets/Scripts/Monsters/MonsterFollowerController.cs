using UnityEngine;
using System.Collections;

[RequireComponent(typeof (BoxCollider))]
public class MonsterFollowerController : MonoBehaviour 
{
	public float speed = 5f;

	private Animator animator;
	private Transform target;


	void Start() {
		animator = GetComponent<Animator>();
	}


	public void SetTarget(Transform target) {
		if (this.target != target)
		{
			this.target = target;
			StopCoroutine("FollowTarget");
			StartCoroutine("FollowTarget");
		}
	}
	
	public void StopFollowing()
	{
		StopCoroutine("FollowTarget");
		target = null;
		animator.SetBool("Running", false);
	}


	IEnumerator FollowTarget() {
		Vector3 direction;
		Vector3 newDir;
		float step;

		animator.SetBool("Running", true);

		while (true) {
			step = Time.deltaTime * speed;

			direction = target.position - transform.position;
			newDir = Vector3.RotateTowards(transform.forward, direction, step, 0f);
			transform.rotation = Quaternion.LookRotation(newDir);

			transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);

			yield return null;
		}
	}
}
