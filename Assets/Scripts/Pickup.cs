﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    [GiveTag]
    [SerializeField]
    private string[] pickUp;
    private Transform pickedUp;

    private Transform hitTransform;

    [GiveTag]
    [SerializeField]
    private string[] walls;

    private Rigidbody2D rb;
    private bool goingRight = true;

    private Vector2 grabPosition;

    private bool isPickedUp = false;
    private bool pickupEnabled = true;



    [Header("Grabbing")]
    [SerializeField]
    private float range;
    [SerializeField]
    private Vector3 grabRight;
    [SerializeField]
    private Vector3 grabLeft;

    [Space]
    [SerializeField]
    private float throwDistance = 2.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grabPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // TODO: Replace with last direction in PlayerController
        if (rb.velocity.x > 0.1f)
            goingRight = true;
        else if (rb.velocity.x < -0.1f)
            goingRight = false;


        Vector2 rayDirection = Vector2.zero;
        if (goingRight)
        {
            rayDirection = Vector2.right;
            grabPosition = transform.position + grabRight;
        }
        else
        {
            rayDirection = Vector2.left;
            grabPosition = transform.position + grabLeft;
        }

        // TODO: Set this correctly
        LayerMask layer = 1 << 0;
/*
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, layer);
        Color color = hit ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, rayDirection, color);

        if (hit.transform != null)
        {
            foreach (string tag in pickUp)
            {
                if (hit.transform.tag == tag)
                    hitTransform = hit.transform;
            }
        }
        else if (!isPickedUp)
            hitTransform = null; */

        if (isPickedUp)
        {
            pickedUp.position = grabPosition;
            DisableOnPickup();
        }
        else if (!pickupEnabled)
        {
            EnableOnPickup(rayDirection);
        }
    }

    private void Update()
    {
        PickUpAction();
    }

    public void PickUpAction()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isPickedUp)
        {
            bool blocked = false;
            Collider2D[] allObjs = Physics2D.OverlapCircleAll(transform.position, range);
            {
                for (int t = 0; t < pickUp.Length; t++)
                {
                    if (blocked)
                        break;

                    for (int i = 0; i < allObjs.Length; i++)
                    {
                        if (allObjs[i].tag == pickUp[t])
                        {
                            // RAYCAST TO CHECK WALL
                            Debug.Log("YES");
                            float distance = Vector2.Distance(transform.position, allObjs[i].transform.position);
                            Vector2 direction = (allObjs[i].transform.position - transform.position).normalized;
                            Debug.DrawRay(transform.position, direction, Color.red);
                            // (transform.position + new Vector3((rayOffset + 0.1f)
                            // (transform.position + new Vector3((rayOffSetX - 0.1f) * directionMultiplier.x, 0, 0)
                            RaycastHit2D[] objHit = Physics2D.RaycastAll(transform.position, direction, distance);


                            for (int l = 0; l < objHit.Length; l++)
                            {
                                if (blocked)
                                    break;

                                for (int j = 0; j < walls.Length; j++)
                                {
                                    if (objHit[l].transform.tag == walls[j])
                                    {
                                        blocked = true;
                                        break;
                                    }
                                }
                            }
                            if (!blocked)
                            {
                                pickedUp = allObjs[i].transform;
                                isPickedUp = true;
                                Debug.Log("KEY IS ON");
                                break;
                            }
                        }
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) && isPickedUp)
        {
            isPickedUp = false;
        }
    }

    private void DisableOnPickup()
    {
        pickedUp.GetComponent<Rigidbody2D>().isKinematic = true;
        Collider2D[] colliders = pickedUp.GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.enabled = false;

        pickupEnabled = false;
    }

    private void EnableOnPickup(Vector2 direction)
    {
        pickedUp.GetComponent<Rigidbody2D>().isKinematic = false;
        Collider2D[] colliders = pickedUp.GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
            collider.enabled = true;

        pickedUp.GetComponent<Rigidbody2D>().velocity = (direction + rb.velocity) * throwDistance;

        pickupEnabled = true;
    }
}
