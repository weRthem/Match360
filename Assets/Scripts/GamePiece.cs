using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

	private Vector3 pos;
	private Quaternion rot;
	private int x;
	private int y;

	public Vector3 Pos //allows other scripts to access the pieces location
	{
		get { return pos; }
		set {
			if (IsMovable()) {
				pos = value;
			}
		}
	}

	public Quaternion Rot //allows other scripts to access the pieces Rotation
	{
		get { return rot; }
		set {
			if (IsMovable()) {
				rot = value;
			}
		}
	}

	public int X //allows other scripts to access the x position in the 3d array
	{
		get { return x; }
		set {
			if (IsMovable()) {
				x = value;
			}
		}
	}

	public int Y //allows other scripts to access the y position in the 3d array
	{
		get { return y; }
		set {
			if (IsMovable()) {
				y = value;
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

	private Clearable clearableComponent;

	public Clearable ClearableComponent
	{
		get { return clearableComponent; }
	}

	private void Awake()
	{
		movableComponent = GetComponent<MovablePieces>();
		colorComponent = GetComponent<ColorPiece>();
		clearableComponent = GetComponent<Clearable>();
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Init(int _x, int _y, Vector3 _pos, Quaternion _rot, grid _grid, grid.PieceType _type){ //Sets the pieces location
		x = _x;
		y = _y;
		pos = _pos;
		rot = _rot; 
		Grid = _grid;
		type = _type;
	}


	private void OnMouseEnter(){
		Grid.EnterPiece(this);
	}

	private void OnMouseUp(){
		Grid.ReleasePiece();
	}

	private void OnMouseDown(){
		Grid.PressedPiece(this);
	}

	public bool IsMovable(){
		return movableComponent != null;
	}

	public bool IsColored()
	{
		return colorComponent != null;
	}

	public bool IsClearable()
	{
		return clearableComponent != null;
	}
}
