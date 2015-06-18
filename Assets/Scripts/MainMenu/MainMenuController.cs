using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	public string VERSION;

	public Text connState;
	
	// Use this for initialization
	void Start () {
		PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
	}
	
	// Update is called once per frame
	void Update () {
		connState.text = PhotonNetwork.connectionStateDetailed.ToString ();
	}
	
	public void Play() {
		PhotonNetwork.ConnectUsingSettings(VERSION);
		
		// show rooms menu
		
	}
	
	void OnJoinedLobby() {
		Debug.Log ("in lobby");
		
		RoomInfo[] list = PhotonNetwork.GetRoomList ();

		Debug.Log (list);

		PopulateList ();
	}
	
	void OnReceivedRoomListUpdate()
	{
		RoomInfo[] list = PhotonNetwork.GetRoomList ();
		
		PopulateList ();
	}
	
	void OnJoinedRoom() {
		Debug.Log ("joined room");
		PhotonNetwork.LeaveRoom ();
	}
	
	void PopulateList() {
		Debug.Log ("populate list");
	}
	
	public void Refresh() {
		RoomInfo[] list = PhotonNetwork.GetRoomList ();
		
		PopulateList ();
	}
	
	public void Create() {
		//RoomOptions op = new RoomOptions () { isVisible = true, maxPlayers = 5 };
		
		//if(roomName.text != "")
		//	PhotonNetwork.CreateRoom (roomName.text, op, TypedLobby.Default);
	}
	
	public void JoinRoom(string roomName) {
		Debug.Log ("joined " + roomName);
	}
	
	public void Exit() {
		Application.Quit ();
	}

}
