using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clearable : MonoBehaviour
{

	public AnimationClip clearAnimation;

	private bool isBeingCleared = false;

	public bool IsBeingCleared
	{
		get { return isBeingCleared; }
	}

	protected GamePiece piece;

	private void Awake()
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

	public virtual void Clear()
	{
		isBeingCleared = true;
		StartCoroutine(ClearCoroutine());
	}

	private IEnumerator ClearCoroutine()
	{
		Animator animator = GetComponent<Animator>();

		if (animator) {
			animator.Play(clearAnimation.name);

			yield return new WaitForSeconds(clearAnimation.length + 1f);

			Destroy(gameObject);
		}
	}
}
