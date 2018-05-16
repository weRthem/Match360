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

	public void Move(Vector3 newPos, Quaternion newRot, float time){
		if (moveCoroutine != null) {
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = MoveCoroutine(newPos, newRot, time);
		StartCoroutine(moveCoroutine);
	}

	private IEnumerator MoveCoroutine(Vector3 newPos, Quaternion newRot, float time) {
		piece.Pos = newPos;
		piece.Rot = newRot;

		Vector3 startPos = transform.position;

		for (float t = 0; t <= 1 * time; t += Time.deltaTime) {
			piece.transform.position = Vector3.Lerp(startPos, newPos, t / time);
			yield return 0;
		}

		piece.transform.localPosition = newPos;
	}
}

