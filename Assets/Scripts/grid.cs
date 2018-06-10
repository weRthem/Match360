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

	private bool inverse = false;

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
		StartCoroutine(Fill());

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
				pieces[x, y] = null;

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

	public IEnumerator Fill(){
		while (FillStep()) {
			inverse = !inverse;
			yield return new WaitForSeconds(fillTime);
		}
	}

	public bool FillStep()
	{
		bool movedPiece = false;

		for (int y = collumnHeight - 1; y > 0; y--) {
			for (int loopX = 0; loopX < totalRows; loopX++) {

				int x = loopX;

				if (inverse) {
					x = totalRows - 1 - loopX;
				}

				GamePiece piece = pieces[x, y];

				GamePiece pieceBelow = emptyPieces[x, y - 1];

				if (pieces[x, y - 1] == null && piece != null) {
					piece.MovableComponent.Move(x, y - 1, pieceBelow.Pos, pieceBelow.Rot, fillTime);
					pieces[x, y] = null;
					pieces[x, y - 1] = piece;
					movedPiece = true;
				} /*else {
					for (int diag = -1; diag <= 1; diag++) {
						if (diag != 0) {
							int diagX = x + diag;

							if (inverse) {
								diagX = x - diag;
							}

							if (diagX >= 0 && diagX < totalRows) {
								GamePiece diagonalPiece = pieces[diagX, y - 1];

								if (diagonalPiece.Type == PieceType.EMPTY) {
									bool hasPieceAbove = true;

									for (int aboveY = y; aboveY >= 0; aboveY--) {
										GamePiece pieceAbove = pieces[diagX, aboveY];

										if (pieceAbove.IsMovable()) {
											break;
										} else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY) {
											hasPieceAbove = false;
											break;
										}
									}

									if (!hasPieceAbove) {
										Destroy(diagonalPiece.gameObject);
										piece.MovableComponent.Move(diagX, y + 1, pieceBelow.Pos, pieceBelow.Rot ,fillTime);
										pieces[diagX, y + 1] = piece;
										SpawnNewPiece(x, y, pieceBelow.Pos, pieceBelow.Rot, PieceType.EMPTY);
										movedPiece = true;
										break;
									}
								}
							}
						}
					}
				}*/

			}
		}

		for (int x = 0; x < totalRows; x++) {
			GamePiece pieceBelow = emptyPieces[x, collumnHeight - 1];

			if (pieces[x, collumnHeight - 1] == null) {
				Vector3 startPos = pieceBelow.Pos;
				startPos.y += 1;
				GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], startPos, pieceBelow.Rot);
				newPiece.transform.parent = transform;

				pieces[x, collumnHeight - 1] = newPiece.GetComponent<GamePiece>();
				pieces[x, collumnHeight - 1].Init(x, collumnHeight, pieceBelow.Pos, pieceBelow.Rot, this, PieceType.NORMAL);
				pieces[x, collumnHeight - 1].MovableComponent.Move(x, collumnHeight - 1, pieceBelow.Pos, pieceBelow.Rot, fillTime);
				pieces[x, collumnHeight - 1].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, collumnHeight - 1].ColorComponent.NumColor));
				//pieceBelow.gameObject.SetActive(false);
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