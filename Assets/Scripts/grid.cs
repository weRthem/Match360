using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid : MonoBehaviour { //3D MATCH 3

	public enum PieceType{
		EMPTY,
		NORMAL,
		ROW_CLEAR,
		COLLUMN_CLEAR,
		OBSTICAL,
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
	[SerializeField] float radius = 3.0f;
	[SerializeField] float fillTime = 0.1f;

	private Dictionary<PieceType, GameObject> piecePrefabDict;

	private GamePiece[,] pieces;

	private GamePiece pressedPiece;
	private GamePiece enteredPiece;

	int ang = 0;

	Vector3 center; //the grids center

	// Use this for initialization
	void Start ()
	{
		center = transform.position; //the grids center
		PlaceGrid();
		piecePrefabDict = new Dictionary<PieceType, GameObject>();

		for (int i = 0; i < piecePrefabs.Length; i++) { //matches the piece prefab to the type
			if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type)) {
				piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
			}
		}


		PlacePieces();

		StartCoroutine(Fill());
	}

	private void PlacePieces()
	{
		pieces = new GamePiece[totalRows, collumnHeight];
		ang = 0;
		for (int x = 0; x < totalRows; x++) {
			Vector3 pos = GridCirlcle();
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
			for (int y = 0; y < collumnHeight; y++) {
				SpawnNewPiece(x, y, pos, rot, PieceType.EMPTY);
				pos.y += distBetweenRows;
			}
		}
	}

	private void PlaceGrid(){
		ang = 0;
		Vector3 center = transform.position; //the grids center
		for (int x = 0; x < totalRows; x++) {
			Vector3 pos = GridCirlcle();
			Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos); //Faces the tiles toward the center
			for (int y = 0; y < collumnHeight; y++) {
				//creates a tile and childs it to the grid
				GameObject background = (GameObject)Instantiate(backgroundPrefab, pos, rot);
				background.transform.parent = transform;
				pos.y += distBetweenRows;
			}
		}
	}

	public Vector3 GridCirlcle() {
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

	public IEnumerator Fill(){
		bool needsRefill = true;

		while (needsRefill) {
			yield return new WaitForSeconds(fillTime);

			while (FillStep()) {
			yield return new WaitForSeconds(fillTime);
		}


		needsRefill = ClearAllValidMatches();
	}
}

	public bool FillStep(){ //TODO invert the fill so it fills from the top and not the bottom
		bool movedPiece = false;

		for (int y = collumnHeight - 1; y >= 1; y--) {
			for (int x = 0; x < totalRows; x++) {
				GamePiece piece = pieces[x, y];
				//Stores the above pieces original pos and rot
				Vector3 piecePos = piece.Pos;
				Quaternion pieceRot = piece.Rot;

				if (piece.IsMovable()) {
					GamePiece pieceBelow = pieces[x, y - 1];

					if (pieceBelow.Type == PieceType.EMPTY) {
						Destroy(pieceBelow.gameObject);
						piece.MovableComponent.Move(x, y - 1, pieceBelow.Pos, pieceBelow.Rot, fillTime);
						pieces[x, y - 1] = piece;
						SpawnNewPiece(x, y, piecePos, pieceRot, PieceType.EMPTY);
						movedPiece = true;
					}
				}
			}
		}

		for (int x = 0; x < totalRows; x++) {
			GamePiece pieceBelow = pieces[x, collumnHeight - 1];
			Vector3 pieceBelowPos = pieceBelow.Pos;
			pieceBelowPos.y += 1;

			if (pieceBelow.Type == PieceType.EMPTY) {
				GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], pieceBelowPos, pieceBelow.Rot);
				newPiece.transform.parent = transform;

				pieces[x, collumnHeight - 1] = newPiece.GetComponent<GamePiece>();
				pieces[x, collumnHeight - 1].Init(x, collumnHeight - 1, pieceBelowPos, pieceBelow.Rot, this, PieceType.NORMAL);
				pieces[x, collumnHeight - 1].MovableComponent.Move(x, collumnHeight - 1, pieceBelow.Pos, pieceBelow.Rot, fillTime);
				pieces[x, collumnHeight - 1].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, collumnHeight - 1].ColorComponent.NumColor));
				Destroy(pieceBelow.gameObject);
				movedPiece = true;
			}
		}


		return movedPiece;
	}

	public GamePiece SpawnNewPiece(int x, int y, Vector3 pos, Quaternion rot, PieceType type) {
		GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], pos, rot);
		newPiece.transform.parent = transform;
		pieces[x, y] = newPiece.GetComponent<GamePiece>();
		pieces[x, y].Init(x, y, pos, rot, this, type);

		return pieces[x, y];
	}


	public bool IsAdjacent(GamePiece piece1, GamePiece piece2){
		Debug.Log("Piece1: " + piece1.X + " : " + piece1.Y);
		Debug.Log("Piece2: " + piece2.X + " : " + piece2.Y);
		return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
			|| (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1)
			|| (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == totalRows - 1);
	}

	public void SwapPieces(GamePiece piece1, GamePiece piece2){
		if (piece1.IsMovable() && piece2.IsMovable()) {
			pieces[piece1.X, piece1.Y] = piece2;
			pieces[piece2.X, piece2.Y] = piece1;

			if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null) {
				int piece1X = piece1.X;
				int piece1Y = piece1.Y;
				Vector3 piece1Pos = piece1.Pos;
				Quaternion piece1Rot = piece1.Rot;

				piece1.MovableComponent.Move(piece2.X, piece2.Y, piece2.Pos, piece2.Rot, fillTime);
				piece2.MovableComponent.Move(piece1X, piece1Y, piece1Pos, piece1Rot, fillTime);

				ClearAllValidMatches();

				if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLLUMN_CLEAR) {
					ClearPiece(piece1.X, piece1.Y);
				}

				if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLLUMN_CLEAR) {
					ClearPiece(piece2.X, piece2.Y);
				}

				pressedPiece = null;
				enteredPiece = null;

				StartCoroutine(Fill());
			} else {
				pieces[piece1.X, piece1.Y] = piece1;
				pieces[piece2.X, piece2.Y] = piece2;
			}
		}
	}

	public void PressedPiece(GamePiece piece)
	{
		pressedPiece = piece;
	}

	public void EnterPiece(GamePiece piece)
	{
		enteredPiece = piece;
	}

	public void ReleasePiece()
	{
		if (IsAdjacent(pressedPiece, enteredPiece)) {
			SwapPieces(pressedPiece, enteredPiece);
		}
	}

	public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
	{
		if (piece.IsColored()) {
			ColorPiece.ColorType color = piece.ColorComponent.Color;
			List<GamePiece> horizontalPieces = new List<GamePiece>();
			List<GamePiece> verticalPieces = new List<GamePiece>();
			List<GamePiece> matchingPieces = new List<GamePiece>();

			//First check horizontal pieces
			horizontalPieces.Add(piece);

			for (int dir = 0; dir <= 1; dir++) {
				for (int xOffset = 1; xOffset < totalRows; xOffset++) {
					int x;
					if (dir == 0) { //Left
						x = newX - xOffset;
						if (x < 0) {
							x = x + totalRows;
						}
					} else { //Right
						x = newX + xOffset;
						if (x >= totalRows) {
							x = x - totalRows;
						}
					}

					Debug.Log(xOffset);

					if (x == newX) {
						break;
					}

					if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color) {
						horizontalPieces.Add(pieces[x, newY]);
					} else {
						break;
					}
				}
			}

			//Traverse Vertically if we found a match (for L and T shape)
			if (horizontalPieces.Count >= 3) {
				for (int i = 0; i < horizontalPieces.Count; i++) {
					for (int dir = 0; dir <= 1; dir++) {
						for (int yOffset = 1; yOffset < collumnHeight; yOffset++) {
							int y;

							if (dir == 0) { //UP
								y = newY - yOffset;
							} else { //Down
								y = newY + yOffset;
							}

							if (y < 0 || y >= collumnHeight) {
								break;
							}

							if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color) {
								verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
							} else {
								break;
							}
						}
					}

					if (verticalPieces.Count < 2) {
						verticalPieces.Clear();
					} else {
						for (int j = 0; j < verticalPieces.Count; j++) {
							matchingPieces.Add(verticalPieces[j]);
						}
						break;
					}
				}
			}

			if (horizontalPieces.Count >= 3) {
				for (int i = 0; i < horizontalPieces.Count; i++) {
					matchingPieces.Add(horizontalPieces[i]);
				}
			}

			if (matchingPieces.Count >= 3) {
				return matchingPieces;
			}

			//Didnt find anything going horizontally first now
			//Check Vertical
			horizontalPieces.Clear();
			verticalPieces.Clear();
			verticalPieces.Add(piece);

			for (int dir = 0; dir <= 1; dir++) {
				for (int yOffset = 1; yOffset < collumnHeight; yOffset++) {
					int y;
					if (dir == 0) { //Left
						y = newY - yOffset;
					} else { //Right
						y = newY + yOffset;
					}

					Debug.Log(yOffset + " Y offset");

					if (y < 0 || y >= collumnHeight) {
						break;
					}

					if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color) {
						verticalPieces.Add(pieces[newX, y]);
					} else {
						break;
					}
				}
			}

			if (verticalPieces.Count >= 3) {
				for (int i = 0; i < verticalPieces.Count; i++) {
					Debug.Log(verticalPieces[i].Pos);
					matchingPieces.Add(verticalPieces[i]);
				}
			}

			//Traverse horizontally if we found a match (for L and T shape)
			if (verticalPieces.Count >= 3) {
				for (int i = 0; i < verticalPieces.Count; i++) {
					for (int dir = 0; dir <= 1; dir++) {
						for (int xOffset = 1; xOffset < totalRows; xOffset++) {
							int x;

							if (dir == 0) { //Left
								x = newX - xOffset;
								if (x < 0) {
									x = x + totalRows;
								}
							} else { //Right
								x = newX + xOffset;
								if (x >= totalRows) {
									x = x - totalRows;
								}
							}


							if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color) {
								horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
							} else {
								break;
							}
						}
					}

					if (horizontalPieces.Count < 2) {
						horizontalPieces.Clear();
					} else {
						for (int j = 0; j < horizontalPieces.Count; j++) {
							matchingPieces.Add(horizontalPieces[j]);
						}
						break;
					}
				}
			}

			if (matchingPieces.Count >= 3) {
				return matchingPieces;
			}
		}

		return null;
	}

	public bool ClearAllValidMatches()
	{
		bool needsRefill = false;

		for (int y = 0; y < collumnHeight; y++) {
			for (int x = 0; x < totalRows; x++) {
				if (pieces[x, y].IsClearable()) {
					List<GamePiece> match = GetMatch(pieces[x, y], x, y);

					if (match != null) {
						PieceType specialPieceType = PieceType.COUNT;
						GamePiece randomPiece = match[Random.Range(0, match.Count)];
						Vector3 specialPiecePos = randomPiece.Pos;
						Quaternion specialPieceRot = randomPiece.Rot;
						int specialPieceX = randomPiece.X;
						int specialPieceY = randomPiece.Y;

						if (match.Count == 4) {
							if (pressedPiece == null || enteredPiece == null) {
								specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLLUMN_CLEAR);
							} else if (pressedPiece.Y == enteredPiece.Y) {
								specialPieceType = PieceType.ROW_CLEAR;
							} else {
								specialPieceType = PieceType.COLLUMN_CLEAR;
							}
						}

						for (int i = 0; i < match.Count; i++) {
							if (ClearPiece(match[i].X, match[i].Y)) {
								needsRefill = true;
								if (match[i] == pressedPiece || match[i] == enteredPiece) {
									specialPieceX = match[i].X;
									specialPieceY = match[i].Y;
									specialPiecePos = match[i].Pos;
									specialPieceRot = match[i].Rot;
								}
							}
						}

						if (specialPieceType != PieceType.COUNT) {
							Destroy(pieces[specialPieceX, specialPieceY]);
							GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPiecePos, specialPieceRot,specialPieceType);

							if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLLUMN_CLEAR) && newPiece.IsColored() && match[0].IsColored()) {
								newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
							}
						}
					}
				}
			}
		}

		return needsRefill;
	}

	public bool ClearPiece(int x, int y)
	{
		if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared) {
			Vector3 piecePos = pieces[x, y].Pos;
			Quaternion pieceRot = pieces[x, y].Rot;
			pieces[x, y].ClearableComponent.Clear();
			SpawnNewPiece(x, y, piecePos, pieceRot, PieceType.EMPTY);

			return true;
		}

		return false;
	}

}
