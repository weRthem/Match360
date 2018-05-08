using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid : MonoBehaviour {

	[SerializeField] GameObject backgroundPrefab;

	[SerializeField] int totalRows = 9;
	[SerializeField] float collumnHeight = 9;
	[SerializeField] float angIncrease = 5;
	[SerializeField] float distBetweenRows = 1;

	float ang = 0;

	// Use this for initialization
	void Start () {
		Vector3 center = transform.position;
		for (int x = 0; x < totalRows; x++) {
			Vector3 pos = RandomCircle(center, 3.0f);
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
			for (float y = 0; y < collumnHeight; y += distBetweenRows) {
				pos.y = y;
				GameObject background = (GameObject)Instantiate(backgroundPrefab, pos, rot);
				background.transform.parent = transform;
			}
		}
	}

	Vector3 RandomCircle(Vector3 center, float radius) {
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		pos.y = center.y;
		pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
		ang += angIncrease;

		return pos;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
