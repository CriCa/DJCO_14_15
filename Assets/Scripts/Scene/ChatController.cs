using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;

public class ChatController : MonoBehaviour {
	
	public Text chatWindow;
	public int maxMessages = 4;
	
	PhotonView photonView;
	InputField input;
	FirstPersonController fpController;
	Queue<string> messages;
	
	void Start () {
		photonView = GetComponent<PhotonView>();
		input = this.GetComponent<InputField>();
		messages = new Queue<string>();
	}

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Y))
		{
			if (fpController == null) {

				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

				foreach (GameObject player in players) {
					if (player.GetPhotonView().isMine) {
						fpController = player.GetComponent<FirstPersonController>();
						break;
					}
				}
			}

			fpController.enabled = false;
			input.interactable = true;
			input.ActivateInputField();
			input.Select();
		}

		if (input.IsActive() && Input.GetKeyDown(KeyCode.Return))
		{
			fpController.enabled = true;
			AddMessage(input.text);
			input.text = "";
			input.interactable = false;
			input.DeactivateInputField();
		}
		
		if (input.IsActive() && Input.GetKeyDown(KeyCode.Escape)) {
			fpController.enabled = true;
			input.text = "";
			input.interactable = false;
			input.DeactivateInputField();
		}
	}

	void AddMessage(string message) {
		photonView.RPC("AddMessage_RPC", PhotonTargets.All, message);
	}
	
	[RPC]
	void AddMessage_RPC(string message) {
		messages.Enqueue(message);
		
		if (messages.Count > maxMessages) {
			messages.Dequeue();
		}
		
		chatWindow.text = "";
		
		foreach (string m in messages) {
			chatWindow.text += m + "\n";
		}
	}
}
