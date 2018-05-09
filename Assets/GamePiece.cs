using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

	private float x;
	private float y;
	private float z;

	public float X
	{
		get { return x; }
		set {
			if (IsMovable()) {
				x = value;
			}
		}
	}

	public float Y
	{
		get { return y; }
		set {
			if (IsMovable()) {
				y = value;
			}
		}
	}

	public float Z
	{
		get { return z; }
		set {
			if (IsMovable()) {
				z = value;
			}
		}
	}

	private grid.PieceType type;

	public grid.PieceType Type
	{
		get { return type; }
	}

	private grid Grid;

	public grid GridRef
	{
		get { return Grid; }
	}

	private MovablePieces movableComponent;

	public MovablePieces MovableComponent
	{
		get { return movableComponent; }
	}

	private ColorPiece colorComponent;

	public ColorPiece ColorComponent
	{
		get { return colorComponent; }
	}

	private void Awake()
	{
		movableComponent = GetComponent<MovablePieces>();
		colorComponent = GetComponent<ColorPiece>();
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Init(float _x, float _y, float _z, grid _grid, grid.PieceType _type){
		x = _x;
		y = _y;
		z = _z;
		Grid = _grid;
		type = _type;
	}

	public bool IsMovable(){
		return movableComponent != null;
	}

	public bool IsColored()
	{
		return colorComponent != null;
	}
}
