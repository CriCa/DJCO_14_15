using UnityEngine;
using System.Collections;

public class HopscotchRoomController : MonoBehaviour {

	public GameObject pressurePlate;
	private int[,] pressurePlates;

	public float plateSize;
	public int planeScale;
	public float spacement;

	private int matrixSize;

	// Use this for initialization
	void Start ()
	{
		float planeSize = planeScale * plateSize;
		matrixSize = (int) (planeSize / plateSize);
		pressurePlates = new int[matrixSize,matrixSize];

		for (int i=0; i < pressurePlates.GetLength(0); i++)
			for (int j=0; j < pressurePlates.GetLength(1); j++)
			{
				GameObject pressurePlateObj = Instantiate (pressurePlate, Vector3.zero, transform.rotation) as GameObject;
				
				Vector3 scale = pressurePlateObj.transform.localScale;
				scale.x = plateSize;
				scale.z = plateSize;
				pressurePlateObj.transform.localScale = scale;

				pressurePlateObj.transform.parent = transform;

				float centeringOffset = planeSize/2f - plateSize/2f + (matrixSize-1)*spacement/2f;
				
				Vector3 position = new Vector3();
				position.x = i*plateSize + i*spacement - centeringOffset;
				position.z = j*plateSize + j*spacement - centeringOffset;
				pressurePlateObj.transform.localPosition = position;
			}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
