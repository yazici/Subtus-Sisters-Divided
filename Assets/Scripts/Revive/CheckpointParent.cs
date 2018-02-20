﻿using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 

public class CheckpointParent : MonoBehaviour 
{
    [Header("Child Settings")]
    [SerializeField]
    private Vector2 childOffset = Vector2.zero;
    public Transform child;

    [Header("Line Settings")]
    [SerializeField]
    private Color color = Color.blue;
    [SerializeField]
    private float lineHeight = 11f; // The distance from 0 on Y to the tip of the rays

    private Transform playerTop;
    private Transform playerBot;

    private void Start()
    {
        playerTop = GameManager.instance.playerTop;
        playerBot = GameManager.instance.playerBot;
        child = transform.GetChild(0);
    }

    private void Update()
    {
        if (transform != SafepointManager.instance.topCheckpoint)
            if (playerTop.position.x > transform.position.x - 1f && playerTop.position.x < transform.position.x + 1f)
                SafepointManager.instance.SetCheckpoint(transform, playerTop);

        if (child != SafepointManager.instance.botCheckpoint)
            if (playerBot.position.x > child.position.x - 1f && playerBot.position.x < transform.position.x + 1f)
                SafepointManager.instance.SetCheckpoint(child, playerBot);
    }

    private void OnDrawGizmosSelected()
    {
        child = transform.GetChild(0);                                                                      // Get the child safepoint
        child.transform.position = new Vector2(transform.position.x, -transform.position.y) + childOffset;  // Offset using the set vector

        // Draw lines representing the safepoints in the scene
        Gizmos.color = color;
        Gizmos.DrawLine(new Vector3(transform.position.x, lineHeight),
                new Vector3(transform.position.x, 0f));
        Gizmos.DrawLine(new Vector3(child.position.x, -lineHeight),
                new Vector3(child.position.x, 0f));
    }
}
