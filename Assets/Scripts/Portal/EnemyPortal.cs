﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : PortalBehaviour 
{
	
	public GameObject particleEffect;
	public override void OnPortalContact()
	{
        
        FMODEmitter[] emitters = GetComponents<FMODEmitter>();
        foreach (FMODEmitter emitter in emitters)
            emitter.Stop();
        GameObject clone = Instantiate(particleEffect, transform.position, Quaternion.identity);
        clone.transform.localScale = new Vector2(clone.transform.localScale.x, transform.localScale.y);
        Destroy(clone, t: 1f);
        Destroy(gameObject);
    }

   

}

// this changes the sprite and script when passing through portals

/*
	[Header ("Scripts")]
	public MonoBehaviour enemyScript;
	public MonoBehaviour enemyScript2;
	public MonoBehaviour friendlyScript;

	[Header("AnimatorController")]
	public RuntimeAnimatorController original;
	public RuntimeAnimatorController reversed;

	[Header("Colliders")]
	public Collider2D originalCollider;
	public Collider2D reversedCollider;

	private Rigidbody2D rb;
	private int q = 0;
	private bool enabler = true;
	private bool startBool = false;

	void Start()
	{
		this.GetComponent<Animator> ().runtimeAnimatorController = original as RuntimeAnimatorController;
		if (GetComponent<Rigidbody2D> () != null) 
		{
			rb = GetComponent<Rigidbody2D> ();
		}
	}
		

	void Invoker()
	{
		originalCollider.enabled = !originalCollider.enabled;
		enemyScript.enabled = !enabler;
		enemyScript2.enabled = !enabler;
		reversedCollider.enabled = !reversedCollider.enabled;
		friendlyScript.enabled = enabler;
		enabler = !enabler;
		startBool = !startBool;

	}



	void ReverseComponents()
	{
		q++;
		Debug.Log(enabler);
		if (!startBool) 
		{
			this.GetComponent<Animator> ().runtimeAnimatorController = original as RuntimeAnimatorController;
			transform.localScale = new Vector3 (transform.localScale.x, -transform.localScale.y, transform.localScale.z);
			rb.gravityScale = -rb.gravityScale;

		} else 
		{
			this.GetComponent<Animator> ().runtimeAnimatorController = reversed as RuntimeAnimatorController;
			transform.localScale = new Vector3 (transform.localScale.x, -transform.localScale.y, transform.localScale.z);
			rb.gravityScale = -rb.gravityScale;
		}




	}

	public override void OnPortalContact()
	{
		Invoker ();
		//Invoke ("Invoker", 0.1f);
		ReverseComponents ();
	}

*/
