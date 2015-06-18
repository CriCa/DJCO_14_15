using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	public string VERSION;

	public Text connState;
	public InstantGuiElement RoomName;
	public InstantGuiList RoomsList;

	private bool connected;

	
	// Use this for initialization
	void Start () {
		PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
	}
	
	// Update is called once per frame
	void Update () {
		connState.text = PhotonNetwork.connectionStateDetailed.ToString ();
	}
	
	public void Play() {
		if(!connected)
			connected = PhotonNetwork.ConnectUsingSettings(VERSION);
		
		// show rooms menu
		
	}
	
	void OnJoinedLobby() {
		RoomInfo[] list = PhotonNetwork.GetRoomList ();

		Debug.Log (PhotonNetwork.countOfRooms);

		PopulateList (list);
	}
	
	void OnReceivedRoomListUpdate()
	{
		Debug.Log ("Received room list update");

		Debug.Log (PhotonNetwork.countOfRooms);

		RoomInfo[] list = PhotonNetwork.GetRoomList ();
		PopulateList (list);
	}
	
	void OnJoinedRoom() {
		Debug.Log ("load other level");
	}
	
	void PopulateList(RoomInfo[] list) {
		//RoomsList.labels.Initialize();

		for (int i = 0; i < list.Length; i++) {
			RoomsList.labels[i] = list[i].name;
			Debug.Log (list[i].name);
		}

	}
	
	public void Refresh() {
		RoomInfo[] list = PhotonNetwork.GetRoomList ();
		PopulateList (list);
	}
	
	public void Create() {
		Debug.Log (RoomName.GetComponent<InstantGuiElement> ().text);

		RoomOptions op = new RoomOptions () { isVisible = true, maxPlayers = 4 };
		
		if(RoomName.text != "")
			PhotonNetwork.JoinOrCreateRoom (RoomName.text, op, TypedLobby.Default);
	}
	
	public void JoinRoom() {
		string roomName = RoomsList.labels [RoomsList.selected];
		PhotonNetwork.JoinRoom(roomName);
	}
	
	public void Exit() {
		Application.Quit ();
	}

}
