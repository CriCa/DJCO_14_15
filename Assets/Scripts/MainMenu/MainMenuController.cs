using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour {

	public string VERSION;

	public InstantGuiElement RoomName;
	public InstantGuiList RoomsList;
	public InstantGuiInputText UsernameInput;
	public InstantGuiSlider VolumeInput;
	public GameObject title;

	private bool connected;

	private RoomInfo[] roomsList;

	private static string USERNAME_KEY = "username_key";
	private static string VOLUME_KEY = "volume_key";
	private static string ROOM_KEY = "room_key";
	

	// Use this for initialization
	void Start () {
		PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
		roomsList = null;

		VolumeInput.value = PlayerPrefs.GetFloat (VOLUME_KEY, 100f);

		UsernameInput.text = PlayerPrefs.GetString (USERNAME_KEY, "UnknownPlayer" + Random.Range (0, 999));
		PlayerPrefs.SetString (USERNAME_KEY,UsernameInput.text );
	}

	void ApplySettings() {
		PlayerPrefs.SetString (USERNAME_KEY,UsernameInput.text );
		PlayerPrefs.SetFloat (VOLUME_KEY,VolumeInput.value );
	}
	
	// Update is called once per frame
	void Update () {
		System.Random rnd = new System.Random();
		if(rnd.NextDouble () > 0.9)
			title.SetActive(true);
		else
			title.SetActive(false);
	}
	
	public void Play() {
		if(!connected)
			connected = PhotonNetwork.ConnectUsingSettings(VERSION);
	}
	
	void OnJoinedLobby() {
		roomsList = PhotonNetwork.GetRoomList ();
		PopulateList ();
	}
	
	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList ();
		PopulateList ();
	}
	
	void OnJoinedRoom() {
		Debug.Log ("load other level");
	}
	
	void PopulateList() {
		List<string> rooms = new List<string> ();

		foreach (RoomInfo r in roomsList)
			rooms.Add (r.name + " - " + r.playerCount + " playing");

		RoomsList.labels = rooms.ToArray ();

	}
	
	public void Create() {
		if (connected) {
			RoomOptions op = new RoomOptions () { isVisible = true, maxPlayers = 4 };
		
			if (RoomName.text != "") {
				PlayerPrefs.SetString(ROOM_KEY, RoomName.text);
				PhotonNetwork.LoadLevel("Level");
			}
				//PhotonNetwork.JoinOrCreateRoom (RoomName.text, op, TypedLobby.Default);
		}
	}
	
	public void JoinRoom() {
		if (connected && RoomsList.selected < roomsList.Length) {
			string roomName = roomsList [RoomsList.selected].name;
			PlayerPrefs.SetString(ROOM_KEY, roomName);
			PhotonNetwork.LoadLevel("Level");
		}
	}
	
	public void Exit() {
		PlayerPrefs.SetString (USERNAME_KEY,UsernameInput.text );
		PlayerPrefs.SetFloat (VOLUME_KEY,VolumeInput.value );
		Application.Quit ();
	}
}
