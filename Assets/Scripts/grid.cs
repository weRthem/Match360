using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid : MonoBehaviour
{ //3D MATCH 3

	public enum PieceType
	{
		EMPTY,
		NORMAL,
		ROW_CLEAR,
		COLLUMN_CLEAR,
		RAINBOW,
		OBSTICAL,
		COUNT, //this is just to make looping easier. no more -1
	};

	[System.Serializable]
	public struct PiecePrefab
	{ //The name of a piece and its prefab
		public PieceType type;
		public GameObject prefab;
	};

	[SerializeField] PiecePrefab[] piecePrefabs;
	[SerializeField] GameObject backgroundPrefab;

	[SerializeField] int totalRows = 9;
	[SerializeField] int collumnHeight = 9;
	[SerializeField] int angIncrease = 5; //that ammount the angle from the center increases with each new block
	[SerializeField] float distBetweenRows = 1f; //how much higher the next block will be placed
	[SerializeField] float radius = 3.0f;
	[SerializeField] float fillTime = 0.1f;

	private Dictionary<PieceType, GameObject> piecePrefabDict;

	private GamePiece[,] pieces;
	private GamePiece[,] emptyPieces;

	private GamePiece pressedPiece;
	private GamePiece enteredPiece;

	int ang = 0;

	Vector3 center; //the grids center

	// Use this for initialization
	void Start()
	{
		center = transform.position; //the grids center
		piecePrefabDict = new Dictionary<PieceType, GameObject>();

		for (int i = 0; i < piecePrefabs.Length; i++) { //matches the piece prefab to the type
			if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type)) {
				piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
			}
		}

		emptyPieces = new GamePiece[totalRows, collumnHeight];
		pieces = new GamePiece[totalRows, collumnHeight];

		PlaceGrid();
		SpawnPieces();
		Fill();

	}

	private void PlaceGrid(){
		ang = 0;
		for (int x = 0; x < totalRows; x++) {
			Vector3 pos = GridCirlcle();
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos); //Faces the tiles toward the center
			for (int y = 0; y < collumnHeight; y++) {
				//creates a tile and childs it to the grid
				GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.EMPTY], pos, rot);
				newPiece.name = "EmptyPiece(" + x + ", " + y + ")"; //names the piece to make it easier to debug
				newPiece.transform.parent = transform;

				emptyPieces[x, y] = newPiece.GetComponent<GamePiece>();
				emptyPieces[x, y].Init(x, y, pos, rot, this, PieceType.EMPTY);

				//does not move as to avoid bugs =)

				pos.y += distBetweenRows;
			}
		}
	}

	private void SpawnPieces() {
		ang = 0;
		for (int x = 0; x < totalRows; x++) {
			Vector3 pos = GridCirlcle();
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos); //Faces the tiles toward the center
			for (int y = 0; y < collumnHeight; y++) {
				SpawnNewPiece(x, y, pos, rot, PieceType.NORMAL);
				pos.y += distBetweenRows;
			}
		}
	}

	private GamePiece SpawnNewPiece(int x, int y, Vector3 pos, Quaternion rot, PieceType type){
		//creates a tile and childs it to the grid
		GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], pos, rot);
		newPiece.name = "Piece(" + x + ", " + y + ")"; //names the piece to make it easier to debug
		newPiece.transform.parent = transform;


		pieces[x, y] = newPiece.GetComponent<GamePiece>();
		pieces[x, y].Init(x, y, pos, rot, this, type);

		return pieces[x, y];
	}

	public Vector3 GridCirlcle()
	{
		Vector3 pos;
		ang += angIncrease;
		pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		pos.y = 0f;
		pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);

		return pos;
	}

	public void Fill()
	{
		while (FillStep()) {
		}
	}

	public bool FillStep()
	{
		bool movedPiece = false;

		for (int y = collumnHeight - 2; y >= 0; y--) {
			for (int x = 0; x < totalRows; x++) {
				GamePiece piece = pieces[x, y];

				GamePiece pieceBelow = emptyPieces[x, y + 1];

				if (pieceBelow.isActiveAndEnabled) {
					piece.MovableComponent.Move(x, y + 1, pieceBelow.Pos, pieceBelow.Rot, 0f);
					emptyPieces[x, y + 1].gameObject.SetActive(false);
					pieces[x, y + 1] = piece;
					emptyPieces[x, y].gameObject.SetActive(true);
					movedPiece = true;
				}

			}
		}

		for (int x = 0; x < totalRows; x++) {
			GamePiece pieceBelow = emptyPieces[x, 0];

			if (pieceBelow.isActiveAndEnabled) {
				GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], pieceBelow.Pos, pieceBelow.Rot);
				newPiece.transform.parent = transform;

				pieces[x, 0] = newPiece.GetComponent<GamePiece>();
				pieces[x, 0].Init(x, -1, pieceBelow.Pos, pieceBelow.Rot, this, PieceType.NORMAL);
				pieces[x, 0].MovableComponent.Move(x, 0, pieceBelow.Pos, pieceBelow.Rot, 0f);
				pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColor));
				pieceBelow.gameObject.SetActive(false);
				movedPiece = true;
			}
		}

		return movedPiece;
	}

	// Update is called once per frame
	void Update()
	{

	}



}