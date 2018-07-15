using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColorPiece : Clearable
{
	
	private ColorPiece.ColorType color;

	public ColorPiece.ColorType Color
	{
		get { return color; }
		set { color = value; }
	}

	// Use this for initialization
	void Start()
	{
		color = ColorPiece.ColorType.COUNT;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public override void Clear(){
		base.Clear();
		Debug.Log(color);
		piece.GridRef.ClearColor(color);
	}
	
}
