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

	public void Move(Vector3 newPos, Quaternion newRot){
		piece.Pos = newPos;
		piece.Rot = newRot;

		piece.transform.localPosition = newPos;
	}
}

