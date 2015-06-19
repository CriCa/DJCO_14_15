using UnityEngine;
using System.Collections;

public class MonsterTagController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			transform.parent.GetComponent<StatuesRoomController>().MonsterTagged();
			gameObject.SetActive(false);
			Destroy (this);
		}
	}
}
