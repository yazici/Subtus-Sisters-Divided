﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BuzzerMovement : MonoBehaviour {

    [Header("Target")]
    [SerializeField]
    private BuzzerTarget buzzerTarget;
    [SerializeField]
    private Transform playerTarget;
    private float followDistance;


    [Header("Movement")]
    [SerializeField]
    private float speed = 4f;
    [SerializeField]
    private float moveTimeMin = 0.2f;
    [SerializeField]
    private float moveTimeMax = 0.5f;
    private Vector2 flyDir;
    private bool changePath = true;

    private Vector2 targetPos;

    private Rigidbody2D rb;

    [Header("Attacking")]
    [SerializeField]
    private float attackTimeWait = 2f;
    [SerializeField]
    private float attackSpeed = 5f;
    [SerializeField]
    private float attackDistance = 5f;

    private bool attacking = false;
    private float attackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPos = buzzerTarget.transform.position;
    }

    void Update ()
    {
        followDistance = buzzerTarget.radius;
        if (buzzerTarget == null)
        {
            Debug.LogError("The buzzer needs a BuzzerTarget");
            return;
        }

        float targetDistance = Vector2.Distance(transform.position, targetPos);

        if (targetDistance < followDistance)
        {
            if (changePath)
            {
                float moveTime = Random.Range(moveTimeMin, moveTimeMax);
                flyDir = new Vector2(Random.value * 2f - 1f, Random.value * 2f - 1f);
                changePath = false;
                StartCoroutine(MoveInterval(moveTime));
            }

            flyDir = Vector2.ClampMagnitude(flyDir, 3f);
            if (flyDir != Vector2.zero)
                rb.AddForce(flyDir * speed);


        }
        else
        {
            flyDir = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y);
            flyDir = flyDir * (targetDistance - followDistance);
            flyDir = Vector2.ClampMagnitude(flyDir, 3f);

            rb.AddForce(flyDir);

            changePath = true;
        }

        if (attacking)
            return;

        float playerDistance = Vector2.Distance(transform.position, playerTarget.position);

        if(playerDistance < attackDistance && playerTarget.GetComponent<PlayerController>().isActive)
        {
            StartCoroutine(AttackInterval(attackTimeWait));
        }
	}

    private IEnumerator MoveInterval(float time)
    {
        yield return new WaitForSeconds(time);
        changePath = true;
    }

    private IEnumerator AttackInterval(float time)
    {
        attacking = true;
        rb.AddForce(new Vector2(0f, speed * 24f), ForceMode2D.Impulse);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, 2f);
        Vector2 targetingPos = playerTarget.position;
        yield return new WaitForSeconds(time);
        Vector2 attackDir = new Vector2(targetingPos.x - transform.position.x, targetingPos.y - transform.position.y);
        rb.AddForce(attackDir.normalized * attackSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(time);
        attacking = false;
    }

    private void FixedUpdate()
    {
        Vector2 rayOrigin = transform.position + new Vector3(rb.velocity.normalized.x, rb.velocity.normalized.y);

        Vector2 rayDirection = rb.velocity;
        rayDirection.Normalize();

        float rayDistance = 0.5f;

        // TODO: Set this correctly
        LayerMask layer = 1 << 0;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, layer);
        Color color = hit ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, rayDirection, color);

        if (hit.transform != null && hit.transform.tag != "Player")
        {
            rb.AddForce(-rayDirection * 2f, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
            collision.transform.GetComponent<PlayerController>().Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Portal")
            Destroy(gameObject);
    }
}
