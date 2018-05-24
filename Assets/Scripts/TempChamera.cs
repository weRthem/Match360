using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempChamera : MonoBehaviour {

	Vector3 rotateVal;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.LeftArrow)) {
			rotateVal = new Vector3(0, 3, 0);
			transform.eulerAngles = transform.eulerAngles - rotateVal;
		} else if (Input.GetKey(KeyCode.RightArrow)) {
			rotateVal = new Vector3(0, -3, 0);
			transform.eulerAngles = transform.eulerAngles - rotateVal;
		} else if (Input.GetKey(KeyCode.UpArrow)) {
			rotateVal = new Vector3(3, 0, 0);
			transform.eulerAngles = transform.eulerAngles - rotateVal;
		} else if (Input.GetKey(KeyCode.DownArrow)) {
			rotateVal = new Vector3(-3, 0, 0);
			transform.eulerAngles = transform.eulerAngles - rotateVal;
		}
	}

}
