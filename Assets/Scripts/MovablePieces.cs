using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePieces : MonoBehaviour
{
	
	private GamePiece piece;
	private IEnumerator moveCoroutine;
	private IEnumerator moveBackCoroutine;

	public bool isMoving = false;

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
		//TODO check if MovablePieces is destroyed in the grid? or here idk
		if (moveCoroutine != null) {
			isMoving = true;
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = MoveCoroutine(newX, newY, newPos, newRot, time);
		StartCoroutine(moveCoroutine);
	}

	public void MoveBack(int newX, int newY, Vector3 newPos, Quaternion newRot, float time) {
		//TODO check if MovablePieces is destroyed in the grid? or here idk
		if (moveCoroutine != null){
			isMoving = true;
			StopCoroutine(moveCoroutine);
		}
		moveBackCoroutine = MoveBackCoroutine(newX, newY, newPos, newRot, time);
		StartCoroutine(moveBackCoroutine);
	}

	private IEnumerator MoveBackCoroutine(int newX, int newY, Vector3 newPos, Quaternion newRot, float time)
	{
		Vector3 startPos = transform.position;
		Quaternion startRot = transform.rotation;

		for (float t = 0; t <= 1 * time; t += Time.deltaTime)
		{
			piece.transform.rotation = Quaternion.Lerp(startRot, newRot, t / time);
			piece.transform.position = Vector3.Lerp(startPos, newPos, t / time);
			yield return 0;
		}

		piece.transform.localPosition = newPos;
		piece.transform.localRotation = newRot;

		for (float t = 0; t <= 1 * time; t += Time.deltaTime)
		{
			piece.transform.rotation = Quaternion.Lerp(newRot, startRot, t / time);
			piece.transform.position = Vector3.Lerp(newPos, startPos, t / time);
			yield return 0;
		}

		piece.transform.localPosition = startPos;
		piece.transform.localRotation = startRot;

		isMoving = false;
	}

	private IEnumerator MoveCoroutine(int newX, int newY,  Vector3 newPos, Quaternion newRot, float time) {
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
		piece.transform.localRotation = newRot;
		isMoving = false;
	}
}

