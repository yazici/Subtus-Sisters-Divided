﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnRadius : MonoBehaviour {

    private Vector3 startPosition;
    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private float radius = 15f;

    [HideInInspector]
    public Transform draggedBy;

    private Rigidbody2D rb;

	void Start ()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
		if(Vector3.Distance(transform.position, startPosition + new Vector3(offset.x, offset.y, 0f)) > radius)
        {
            if(draggedBy != null)
            {
                draggedBy.GetComponent<PullBoxes>().StopDragging();
                draggedBy = null;
            }
            RespawnObject();
        }
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.75f, 0f);
        if(Application.isPlaying)
            Gizmos.DrawWireSphere(startPosition + new Vector3(offset.x, offset.y, 0f), radius);
        else
            Gizmos.DrawWireSphere(transform.position + new Vector3(offset.x, offset.y, 0f), radius);
    }

    public void RespawnObject()
    {
        if (GameManager.instance.playerTop.GetComponent<Pickup>().pickedUp == transform)
            GameManager.instance.playerTop.GetComponent<Pickup>().DropIt();
        else if (GameManager.instance.playerBot.GetComponent<Pickup>().pickedUp == transform)
            GameManager.instance.playerBot.GetComponent<Pickup>().DropIt();

        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        GetComponent<GravityFlip>().SetGravity(startPosition.y);
    }
}
