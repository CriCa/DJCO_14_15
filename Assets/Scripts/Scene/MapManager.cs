using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour 
{
	public static MapManager instance = null; // singleton object	
	public int mapSize;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject); 
		}
	}

	void Start () {
	
	}
}
