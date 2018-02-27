﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBoxes : MonoBehaviour {

	public float distance = 1f;
	public LayerMask boxMask;
	public float pushSpeed;

	private GameObject box;
	private Vector2 rayLine;
	private bool isPulling;

	private string controllerCode;
	private string controllerOne = "_C1";
	private string controllerTwo = "_C2";

	private string pullBox = "Pulling";

	private PlayerController playerController;

	void Start(){
		playerController = GetComponent<PlayerController> ();

		if (playerController.Player == Controller.Player1)
		{
			pullBox += controllerOne;

		}
		else
		{
			pullBox += controllerTwo;

		}
	}

	void Update () 
	{
		rayLine = new Vector2 (transform.position.x, transform.position.y - 1);

		Physics2D.queriesStartInColliders = false;
		RaycastHit2D hit = Physics2D.Raycast (rayLine, Vector2.right * transform.localScale.x, distance, boxMask);

		if (hit.collider != null && hit.collider.gameObject.tag == "Pickup"/*or box*/ && (Input.GetKeyDown (KeyCode.G) 
			|| Input.GetButtonDown(pullBox)) && !playerController.inAir) 
		{
			if (hit.collider.GetComponent<OverEdgeFalling> ().IsGrounded ())
			{
				isPulling = true;
				box = hit.collider.gameObject;
				box.GetComponent<Rigidbody2D> ().mass = pushSpeed;
				box.GetComponent<FixedJoint2D> ().enabled = true;
				box.GetComponent<FixedJoint2D> ().connectedBody = this.GetComponent<Rigidbody2D> ();
			}
			//code
			//cant jump while pulling/pushing
			//code

		} 
		else if ((Input.GetKeyUp (KeyCode.G) || Input.GetButtonUp(pullBox)) && isPulling) 
		{
			isPulling = false;
			box.GetComponent<Rigidbody2D> ().mass = 100;
			box.GetComponent<Rigidbody2D> ().velocity = new Vector2(0, box.GetComponent<Rigidbody2D>().velocity.y);
			box.GetComponent<FixedJoint2D> ().enabled = false;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;

		Gizmos.DrawLine (rayLine, rayLine + Vector2.right * transform.localScale.x * distance);
	}
}
