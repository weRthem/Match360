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
		ANY,
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
		bool needsRefill = true;

		while (needsRefill) {
			yield return new WaitForSeconds(fillTime * 2f);

			while (FillStep()) {
				inverse = !inverse;
				yield return new WaitForSeconds(fillTime);
			}
			needsRefill = ClearAllValidMatches();
		}

		CheckIfCorrectSpot();
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
			int y = collumnHeight - 1;

			if (pieces[x, collumnHeight - 1] == null) {
				Vector3 startPos = pieceBelow.Pos;
				startPos.y += 1;
				GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], startPos, pieceBelow.Rot);
				newPiece.transform.parent = transform;

				pieces[x, y] = newPiece.GetComponent<GamePiece>();
				pieces[x, y].Init(x, y, pieceBelow.Pos, pieceBelow.Rot, this, PieceType.NORMAL);
				pieces[x, y].MovableComponent.Move(x, y, pieceBelow.Pos, pieceBelow.Rot, fillTime);
				pieces[x, y].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, collumnHeight - 1].ColorComponent.NumColor));
				movedPiece = true;
			}
		}

		return movedPiece;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public bool IsAdjacent(GamePiece piece1, GamePiece piece2){

		//the bug may be in here
		return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
			|| (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1)
			|| (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == totalRows - 1);
	}

	public void SwapPieces(GamePiece piece1, GamePiece piece2){

		if (piece1.IsMovable() && piece2.IsMovable()) {
			int piece1X = piece1.X;
			int piece1Y = piece1.Y;
			Vector3 piece1Pos = piece1.Pos;
			Quaternion piece1Rot = piece1.Rot;

			int piece2X = piece2.X;
			int piece2Y = piece2.Y;
			Vector3 piece2Pos = piece2.Pos;
			Quaternion piece2Rot = piece2.Rot;

			//Checks if Piece 1 is a hyper cube
			if (piece1.Type == PieceType.ANY && piece1.IsClearable() && piece2.IsColored()) {
				ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

				if (clearColor) {
					clearColor.Color = piece2.ColorComponent.Color;
				}

				ClearPiece(piece1.X, piece1.Y);
				StartCoroutine(Fill());
			//Checks if piece 2 is a hyper cube
			} else if (piece2.Type == PieceType.ANY && piece2.IsClearable() && piece1.IsColored()) {
				ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

				if (clearColor) {
					clearColor.Color = piece1.ColorComponent.Color;
				}

				ClearPiece(piece2.X, piece2.Y);
				StartCoroutine(Fill());
			//if there are no hyper cubes finish checking for a match
			} else {

				piece1.MovableComponent.Move(piece2X, piece2Y, piece2Pos, piece2Rot, fillTime);
				piece2.MovableComponent.Move(piece1X, piece1Y, piece1Pos, piece1Rot, fillTime);


				if (piece1.X == piece2X && piece2.X == piece1X && piece1.Y == piece2Y && piece2.Y == piece1Y) {
					pieces[piece1X, piece1Y] = piece2;
					pieces[piece2X, piece2Y] = piece1;
					if (GetMatch(piece1, piece2X, piece2Y) != null || GetMatch(piece2, piece1X, piece1Y) != null
						|| piece1.Type == PieceType.ANY || piece2.Type == PieceType.ANY) {

						ClearSwappedMatches(piece1X, piece1Y);
						ClearSwappedMatches(piece2X, piece2Y);

						if (piece1.Type == PieceType.COLLUMN_CLEAR || piece1.Type == PieceType.ROW_CLEAR) {
							ClearPiece(piece1X, piece1Y);
						}

						if (piece2.Type == PieceType.COLLUMN_CLEAR || piece2.Type == PieceType.ROW_CLEAR) {
							ClearPiece(piece2X, piece2Y);
						}


						pressedPiece = null;
						enteredPiece = null;

						StartCoroutine(Fill());
					} else {
						StartCoroutine(SwapBack(piece1, piece2, piece1X, piece1Y, piece1Pos, piece1Rot, piece2X, piece2Y, piece2Pos, piece2Rot));
						pressedPiece = null;
						enteredPiece = null;
						pieces[piece1X, piece1Y] = piece1;
						pieces[piece2X, piece2Y] = piece2;
					}
				} else {
					Debug.LogError("Piece1: " + piece1.X + " " + piece1.Y);
					Debug.LogError("Piece2: " + piece2.X + " " + piece2.Y);
				}

			}

		}
			pressedPiece = null;
	}

	private IEnumerator SwapBack(GamePiece piece1, GamePiece piece2, int piece1X, int piece1Y, Vector3 piece1Pos, Quaternion piece1Rot, int piece2X, int piece2Y, Vector3 piece2Pos, Quaternion piece2Rot)
	{ //swaps the pieces then swaps them back if they dont match
		piece1.MovableComponent.Move(piece2X, piece2Y, piece2Pos, piece2Rot, fillTime);
		piece2.MovableComponent.Move(piece1X, piece1Y, piece1Pos, piece1Rot, fillTime);

		yield return new WaitForSeconds(fillTime * 1.5f);

		piece1.MovableComponent.Move(piece1X, piece1Y, piece1Pos, piece1Rot, fillTime);
		piece2.MovableComponent.Move(piece2X, piece2Y, piece2Pos, piece2Rot, fillTime);

		CheckIfCorrectSpot();
	}

	void CheckIfCorrectSpot() {
		for (int x = 0; x < totalRows; x++) {
			for (int y = 0; y < collumnHeight; y++) {
				if (pieces[x, y] != null) {
					if (emptyPieces[x, y].X != pieces[x, y].X) {
						Debug.Log("piece [" + x + ", " + y + "] X does not match");
					} else if (emptyPieces[x, y].Y != pieces[x, y].Y) {
						Debug.Log("piece [" + x + ", " + y + "] Y does not match");
					} else if (emptyPieces[x, y].Pos != pieces[x, y].Pos) {
						Debug.Log("piece [" + x + ", " + y + "] Pos does not match");
					} else if (emptyPieces[x, y].Rot != pieces[x, y].Rot) {
						Debug.Log("piece [" + x + ", " + y + "] Rot does not match");
					} else {
						Debug.Log("all g");
					}

				}
			}
		}
	}

	public GamePiece whatIsPressed()
	{
		return pressedPiece;
	}

	public void PressedPiece(GamePiece piece)
	{
		pressedPiece = piece;
	}

	public void EnterPiece(GamePiece piece){

		if (IsAdjacent(pressedPiece, piece)) {
			enteredPiece = piece;
			SwapPieces(pressedPiece, piece);
		}
	}

	public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY){
		if (piece.IsColored()) {
			ColorPiece.ColorType color = piece.ColorComponent.Color;
			List<GamePiece> horizontalPieces = new List<GamePiece>();
			List<GamePiece> verticalPieces = new List<GamePiece>();
			List<GamePiece> matchingPieces = new List<GamePiece>();

			//First check horizontally
			horizontalPieces.Add(piece);

			for (int dir = 0; dir <= 1; dir++) { //changes direction of traversal
				for (int xOffset = 1; xOffset < totalRows; xOffset++) {
					int x;

					if (dir == 0) { //go left?
						x = newX - xOffset;
						if (x < 0) {
							x = x + totalRows;
						}
					} else { //go Right?
						x = newX + xOffset;
						if (x >= totalRows) {
							x = x - totalRows;
						}
					}

					if (x == newX) {
						break;
					}

					if (pieces[x, newY] != null) {
						if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color) {
							horizontalPieces.Add(pieces[x, newY]);
						} else {
							break;
						}
					}
				}
			}

			if (horizontalPieces.Count >= 3) {
				for (int i = 0; i < horizontalPieces.Count; i++) {
					matchingPieces.Add(horizontalPieces[i]);
				}
			}

			//Traverse vertically if we find a match to look for a L or T shape
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

							if (pieces[horizontalPieces[i].X, y] != null) {
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
			}


			if (matchingPieces.Count >= 3) {
				return matchingPieces;
			}

			//Check Vertically
			horizontalPieces.Clear();
			verticalPieces.Clear();
			verticalPieces.Add(piece);

			for (int dir = 0; dir <= 1; dir++) { //changes direction of traversal
				for (int yOffset = 1; yOffset < collumnHeight; yOffset++) {
					int y;

					if (dir == 0) { //go up?
						y = newY - yOffset;
					} else { //go down?
						y = newY + yOffset;
					}

					if (y < 0 || y >= collumnHeight) {
						break;
					}

					if (pieces[newX, y] != null) {
						if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color) {
							verticalPieces.Add(pieces[newX, y]);
						} else {
							break;
						}
					}

				}
			}

			if (verticalPieces.Count >= 3) {
				for (int i = 0; i < verticalPieces.Count; i++) {
					matchingPieces.Add(verticalPieces[i]);
				}
			}

			//Traverse horizontally if we find a match to look for a L or T shape
			//bug in here somewhere that dosent find the upright "T" as a match
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

							if (x == newX) {
								break;
							}

							if (pieces[x, verticalPieces[i].Y] != null) {
								if (pieces[x, verticalPieces[i].Y].ColorComponent.Color == color) {
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
			}

			if (matchingPieces.Count >= 3) {
				return matchingPieces;
			}

		}

		return null;

	}

	public bool ClearAllValidMatches(){
		bool needsRefill = false;
		for (int y = 0; y < collumnHeight; y++) {
			for (int x = 0; x < totalRows; x++) {
				if (pieces[x, y] == null) {
					break;
				}
				if (pieces[x, y].IsClearable()) {
					List<GamePiece> match = GetMatch(pieces[x, y], x, y);
					if (match != null) {
						PieceType specialPieceType = PieceType.COUNT;
						GamePiece randomPiece = match[Random.Range(0, match.Count)];
						int specialPieceX = randomPiece.X;
						int specialPieceY = randomPiece.Y;
						Vector3 specialPiecePos = randomPiece.Pos;
						Quaternion specialPieceRot = randomPiece.Rot;

						if (match.Count == 4) {
							if (pressedPiece == null || enteredPiece == null) {
								specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLLUMN_CLEAR); //casts the piece types to a int then chooses randomly between them
							} else if (pressedPiece.Y == enteredPiece.Y) {
								specialPieceType = PieceType.ROW_CLEAR;
							} else {
								specialPieceType = PieceType.COLLUMN_CLEAR;
							}
						} else if (match.Count >= 5) {
							specialPieceType = PieceType.ANY;
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

								if (pieces[match[i].X, match[i].Y] != null) {
									pieces[match[i].X, match[i].Y] = null;
								}
							}
						}

						if (specialPieceType != PieceType.COUNT) {
							//Destroy(pieces[specialPieceX, specialPieceY]);
							GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPiecePos, specialPieceRot, specialPieceType);

							if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLLUMN_CLEAR)
								&& newPiece.IsColored() && match[0].IsColored()) {
								newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
							} else if (specialPieceType == PieceType.ANY && newPiece.IsColored()) {
								newPiece.ColorComponent.SetColor(ColorPiece.ColorType.ANY);
							}
						}


					}

				}

			}
		}

		return needsRefill;
	}

	public void ClearSwappedMatches(int x, int y) {
		if (pieces[x, y].IsClearable()) {
			List<GamePiece> match = GetMatch(pieces[x, y], x, y);
			if (match != null) {
				PieceType specialPieceType = PieceType.COUNT;
				GamePiece randomPiece = match[Random.Range(0, match.Count)];
				int specialPieceX = randomPiece.X;
				int specialPieceY = randomPiece.Y;
				Vector3 specialPiecePos = randomPiece.Pos;
				Quaternion specialPieceRot = randomPiece.Rot;

				if (match.Count == 4) {
					if (pressedPiece == null || enteredPiece == null) {
						specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLLUMN_CLEAR); //casts the piece types to a int then chooses randomly between them
					} else if (pressedPiece.Y == enteredPiece.Y) {
						specialPieceType = PieceType.ROW_CLEAR;
					} else {
						specialPieceType = PieceType.COLLUMN_CLEAR;
					}
				} else if (match.Count >= 5) {
					specialPieceType = PieceType.ANY;
				}


				for (int i = 0; i < match.Count; i++) {

					if (ClearPiece(match[i].X, match[i].Y)) {
						//needsRefill = true;

						if (match[i] == pressedPiece || match[i] == enteredPiece) {
							specialPieceX = match[i].X;
							specialPieceY = match[i].Y;
							specialPiecePos = match[i].Pos;
							specialPieceRot = match[i].Rot;
						}

						if (pieces[match[i].X, match[i].Y] != null) {
							pieces[match[i].X, match[i].Y] = null;
						}
					}
				}

				if (specialPieceType != PieceType.COUNT) {
					//Destroy(pieces[specialPieceX, specialPieceY]);
					GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPiecePos, specialPieceRot, specialPieceType);

					if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLLUMN_CLEAR)
						&& newPiece.IsColored() && match[0].IsColored()) {
						newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
					} else if (specialPieceType == PieceType.ANY && newPiece.IsColored()) {
						newPiece.ColorComponent.SetColor(ColorPiece.ColorType.ANY);
					}
				}


			}

		}
	}

	public bool ClearPiece(int x, int y){
		if (pieces[x, y] != null) {
			if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared) {
				pieces[x, y].ClearableComponent.Clear();
				if (pieces[x, y] != null) {
					pieces[x, y] = null;
				}
				return true;
			}
		}

		return false;
	}

	public void ClearRow(int row)
	{
		for (int x = 0; x < totalRows; x++) {
			ClearPiece(x, row);
		}
	}

	public void ClearCollumn(int collumn)
	{
		for (int y = 0; y < collumnHeight; y++) {
			ClearPiece(collumn, y);
		}
	}

	public void ClearColor(ColorPiece.ColorType color)
	{
		for (int x = 0; x < totalRows; x++) {
			for (int y = 0; y < collumnHeight; y++) {
				if (pieces[x, y] != null) {
					if (pieces[x, y].IsColored() && (pieces[x, y].ColorComponent.Color == color)
						|| color == ColorPiece.ColorType.ANY) {
						ClearPiece(x, y);
					}
				}
			}
		}
	}

}