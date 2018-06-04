using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePieces : MonoBehaviour
{
	
	private GamePiece piece;
	private IEnumerator moveCoroutine;

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

	public void Move(int newX, int newY, Vector3 newPos, Quaternion newRot, float time){

		piece.X = newX;
		piece.Y = newY;
		piece.Pos = newPos;
		piece.Rot = newRot;

		piece.transform.localPosition = newPos;
		piece.transform.localRotation = newRot;

		/*if (moveCoroutine != null) {
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = MoveCoroutine(newX, newY, newPos, newRot, time);
		StartCoroutine(moveCoroutine);*/
	}

	/*private IEnumerator MoveCoroutine(int newX, int newY,  Vector3 newPos, Quaternion newRot, float time) {
		piece.X = newX;
		piece.Y = newY;
		piece.Pos = newPos;
		piece.Rot = newRot;

		Vector3 startPos = transform.position;
		Quaternion startRot = transform.rotation;

		for (float t = 0; t <= 1 * time; t += Time.deltaTime) {
			piece.transform.rotation = Quaternion.Lerp(startRot, newRot, t / time);
			piece.transform.position = Vector3.Lerp(startPos, newPos, t / time);
			yield return 0;
		}

		piece.transform.localPosition = newPos;
	}*/
}

