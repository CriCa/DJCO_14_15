using UnityEngine;
using System.Collections;

public class SwingController : MonoBehaviour {

	public AnimationCurve curve;
	public int angle;
	public float swingSpeed;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine ("Swing");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	IEnumerator Swing()
	{
		Vector3 goal = new Vector3(angle, 0f, 0f);
		Vector3 position = transform.localEulerAngles;
		float initialDistance = Vector3.Distance(position, goal);

		while (true)
		{
			float distance = Vector3.Distance(position, goal);

			float distancePerc = distance / initialDistance;

			position = Vector3.MoveTowards (position, goal, Time.deltaTime * curve.Evaluate(distancePerc) * swingSpeed);
			transform.localEulerAngles = position;

			if (position == goal)
			{
				goal.x = -goal.x;
				initialDistance = Vector3.Distance (position, goal);
			}
		
			yield return null;
		}

	}
}
