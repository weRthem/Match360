using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePieces : MonoBehaviour
{

	private GamePiece piece;

	void Awake()
	{
		piece = GetComponent<GamePiece>();
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Move(float newX, float newY, float newZ){
		piece.X = newX;
		piece.Y = newY;
		piece.Z = newZ;
		Vector3 pos = new Vector3(newX, newY, newZ);

		piece.transform.localPosition = pos;
	}
}

