using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

/* 
 * Network Manager
 * Responsible for sending player movement info over the network.
 */
public class PlayerNetworkManager : Photon.MonoBehaviour 
{
	float smoothing = 10f; // lerping movement update speed

	Vector3 position = Vector3.zero;
	Quaternion rotation = Quaternion.identity;

	Transform flashlight;
	Light flashlightToggle;

	void Start () {
		// TODO: not sure if this should be moved with its own photonview, but it'll remain here for now
		flashlightToggle = GetComponentInChildren<Light>();
		flashlight = GetComponentInChildren<FlashlightController>().transform;

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
		else {
			StartCoroutine("UpdateData");
		}
	}

	IEnumerator UpdateData()
	{
		// continuously update the positions of all players in the game
		while(true) {
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
			yield return null;
		}
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		// if the stream is writing, player is local and we should tell others to update according to our info
		if(stream.isWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);

			stream.SendNext(flashlight.rotation);
			stream.SendNext(flashlightToggle.enabled);
		}
		// otherwise, we need to update this player according to its owner
		else {
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();

			flashlight.rotation = (Quaternion)stream.ReceiveNext();
			flashlightToggle.enabled = (bool) stream.ReceiveNext();
		}
	}
}