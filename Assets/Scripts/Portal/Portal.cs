﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour 
{

	public Vector2 vec;

	void Update()
	{
		
	}

	void OnTriggerExit2D(Collider2D otherObj)
	{
		if (otherObj.gameObject.GetComponent<PortalBehaviour>() != null) 
		{
			if (otherObj.gameObject.transform.position.y < Mathf.Abs(vec.y)) {
				Debug.Log ("p");
				otherObj.GetComponent<PortalBehaviour> ().OnPortalContact ();
			}





		}
	}

	void OnTriggerEnter2D(Collider2D otherObj)
	{
		if (otherObj.gameObject.GetComponent<PlayerPortal> () != null) {
			otherObj.GetComponent<PlayerPortal> ().OnPortalContact ();
		} else {
			vec = otherObj.gameObject.transform.position;
		}
	}


}
