using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* 
 * GameHint Controller
 * Displays info on the screen when the local player approaches an object with this script.
 * Must have a box collider with "isTrigger" set attached. Only one GameHint should ever be displayed at the same time.
 */
public class GameHintController : MonoBehaviour 
{
	public string tip;
	
	Text output;
	
	void Start () {
		output = GameObject.FindGameObjectWithTag("GameHint").GetComponent<Text>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player" && other.gameObject == NetworkManager.instance.GetPlayer()) {
			output.text = tip;
			output.enabled = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player" && other.gameObject == NetworkManager.instance.GetPlayer()) {
			output.text = "";
			output.enabled = false;
		}
	}
}
