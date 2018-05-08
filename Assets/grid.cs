using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid : MonoBehaviour { //3D MATCH 3

	public enum PieceType{
		NORMAL,
		COUNT, //this is just to make looping easier. no more -1
	};

	[System.Serializable]
	public struct PiecePrefab{ //The name of a piece and its prefab
		public PieceType type;
		public GameObject prefab;
	};

	[SerializeField] PiecePrefab[] piecePrefabs;
	[SerializeField] GameObject backgroundPrefab;

	[SerializeField] int totalRows = 9;
	[SerializeField] int collumnHeight = 9;
	[SerializeField] int angIncrease = 5; //that ammount the angle from the center increases with each new block
	[SerializeField] float distBetweenRows = 1f; //how much higher the next block will be placed

	private Dictionary<PieceType, GameObject> piecePrefabDict;

	private GameObject[,] pieces;

	int ang = 0;

	// Use this for initialization
	void Start ()
	{
		PlaceGrid();
		piecePrefabDict = new Dictionary<PieceType, GameObject>();

		for (int i = 0; i < piecePrefabs.Length; i++) { //matches the piece prefab to the type
			if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type)) {
				piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
			}
		}

		Vector3 center = transform.position; //the grids center
		pieces = new GameObject[totalRows, collumnHeight];
		ang = 0;
		for (int x = 0; x < totalRows; x++) {
			Vector3 pos = GridCirlcle(center, 3.0f);
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
			for (int y = 0; y < collumnHeight; y++) {
				pieces[x, y] = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], pos, rot);
				pieces[x, y].name = "Piece(" + x + "," + y + ")";
				pieces[x, y].transform.parent = transform;
				pos.y += distBetweenRows;
			}
		}
	}

	private void PlaceGrid(){
		ang = 0;
		Vector3 center = transform.position; //the grids center
		for (int x = 0; x < totalRows; x++) {
			Vector3 pos = GridCirlcle(center, 3.0f);
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos); //Faces the tiles toward the center
			for (int y = 0; y < collumnHeight; y++) {
				//creates a tile and childs it to the grid
				GameObject background = (GameObject)Instantiate(backgroundPrefab, pos, rot);
				background.transform.parent = transform;
				pos.y += distBetweenRows;
			}
		}
	}

	Vector3 GridCirlcle(Vector3 center, float radius) {
		Vector3 pos;
		ang += angIncrease;
		pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		pos.y = 0f;
		pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);

		return pos;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
