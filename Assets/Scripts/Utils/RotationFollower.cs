using UnityEngine;
using System.Collections;

/* 
 * Rotation Follower
 * Copies the rotation of a given target.
 */
public class RotationFollower : MonoBehaviour 
{
	public Transform target;
	
	void Update () {
		transform.rotation = target.rotation;
	}
}
