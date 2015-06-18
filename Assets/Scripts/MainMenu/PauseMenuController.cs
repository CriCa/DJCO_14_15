using UnityEngine;
using System.Collections;

public class PauseMenuController : MonoBehaviour {

	public void Resume() {

	}

	public void Disconnect() {
		PhotonNetwork.Disconnect ();
		PhotonNetwork.LoadLevel (0);
	}

	public void Exit() {
		Application.Quit ();
	}
}
