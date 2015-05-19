using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

/* 
 * Network Manager
 * Responsible for sending player movement info over the network.
 * Also includes all info regarding a player's children objects.
 */
public class PlayerNetworkManager : Photon.MonoBehaviour 
{
	Vector3 positionGoal = Vector3.zero; // lerping destination
	Quaternion rotationGoal = Quaternion.identity; // we don't want to move directly to the new position, we want to go towards it
	float smoothing = 10f; // lerping movement update speed

	Transform flashlight; // original flashlight
	Quaternion flashlightGoal = Quaternion.identity; // lerping destionation
	Light flashlightToggle; // spotlight status (on or off)

	// using Awake, and not Start, because OnPhotonSerializeView may run before Start has finished
	void Awake() {
		// should always grab a reference to the flashlight
		flashlight = transform.Find("Flashlight");
		flashlightToggle = GetComponentInChildren<Light>();

		// if it's a local object, we want to enable all controls
		if(photonView.isMine) {
			// lock cursor and hide it
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			// enable movement components
			GetComponent<Rigidbody>().useGravity = true;
			GetComponent<AudioSource>().enabled = true;
			GetComponent<AudioListener>().enabled = true;
			GetComponent<FirstPersonController>().enabled = true;

			// enable all cameras
			foreach (Camera cam in GetComponentsInChildren<Camera>()) {
				cam.enabled = true;
			}

			// flashlight should be drawn on top of everything for this player only
			transform.Find("Flashlight/FlashlightModel/Mesh").gameObject.layer = 8;
			GetComponentInChildren<FlashlightController>().enabled = true;
		}

		// otherwise, we want to update our object with the info we might receive
		else {
			StartCoroutine("UpdateData");
		}
	}

	IEnumerator UpdateData() {
		// continuously update the positions of all players in the game
		while(true) {

			/* TODO:
			 * keep an eye on aScalar lerp error, it's possible we need to clamp the smoothing value
			 * http://forum.unity3d.com/threads/quaternion-lerp-problem-compareapproximately-ascalar-0-0f.154218/
			 */

			transform.position = Vector3.Lerp(transform.position, positionGoal, Time.deltaTime * smoothing);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotationGoal, Time.deltaTime * smoothing);
			flashlight.rotation = Quaternion.Lerp(flashlight.rotation, flashlightGoal, Time.deltaTime * smoothing);
			yield return null;
		}
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		// if the stream is writing, player is local and we should tell others to update according to our info
		if(stream.isWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);

			stream.SendNext(flashlight.rotation);
			stream.SendNext(flashlightToggle.enabled);
		}
		// otherwise, we need to update this player according to its owner
		else {
			positionGoal = (Vector3)stream.ReceiveNext();
			rotationGoal = (Quaternion)stream.ReceiveNext();

			flashlightGoal = (Quaternion)stream.ReceiveNext();
			flashlightToggle.enabled = (bool)stream.ReceiveNext(); // since this one is a simple bool, we can update it directly
		}
	}
}