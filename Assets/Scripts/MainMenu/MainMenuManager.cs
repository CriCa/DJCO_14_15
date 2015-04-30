using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RoomListItem {
	public string name;
	public int playerCount;
	public Button.ButtonClickedEvent enterRoom;

	public RoomListItem(string n, int c) {
		name = n; playerCount = c;
	}
}

public class MainMenuManager : MonoBehaviour {

	public string VERSION;

	public GameObject sampleRoom;
	public List<RoomListItem> roomList;
	public Transform roomListObject;

	// Use this for initialization
	void Start () {
		PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play() {
		PhotonNetwork.ConnectUsingSettings(VERSION);

		// show rooms menu

	}

	void OnJoinedLobby() {
		Debug.Log ("in lobby");

		RoomInfo[] list = PhotonNetwork.GetRoomList ();

		roomList.Clear ();

		foreach (RoomInfo item in list)
			roomList.Add(new RoomListItem(item.name, item.playerCount));

		PopulateList ();
	}

	void OnReceivedRoomListUpdate()
	{
		RoomInfo[] list = PhotonNetwork.GetRoomList ();
		
		roomList.Clear ();
		
		foreach (RoomInfo item in list)
			roomList.Add(new RoomListItem(item.name, item.playerCount));
		
		PopulateList ();
	}

	void OnJoinedRoom() {
		Debug.Log ("joined room");
		PhotonNetwork.LeaveRoom ();
	}

	void PopulateList() {
		foreach (var item in roomList) {
			GameObject newRoomItem = Instantiate(sampleRoom) as GameObject;
			newRoomItem.transform.SetParent (roomListObject, false);

			RoomListItemButton newRoom = newRoomItem.GetComponent<RoomListItemButton> ();
			newRoom.nameLabel.text = item.name;
			newRoom.button.onClick = item.enterRoom;
		}
	}

	public void Refresh() {
		RoomInfo[] list = PhotonNetwork.GetRoomList ();
		
		roomList.Clear ();
		
		foreach (RoomInfo item in list)
			roomList.Add(new RoomListItem(item.name, item.playerCount));
		
		PopulateList ();
	}

	public void Create(InputField roomName) {
		RoomOptions op = new RoomOptions () { isVisible = true, maxPlayers = 5 };

		if(roomName.text != "")
			PhotonNetwork.CreateRoom (roomName.text, op, TypedLobby.Default);
	}

	public void JoinRoom(string roomName) {
		Debug.Log (roomName);
	}

	public void ShowOptions() {
		// show options
		Debug.Log ("options");
	}
	
	public void Exit() {
		Application.Quit ();
	}
}
