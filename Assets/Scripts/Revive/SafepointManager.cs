﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafepointManager : MonoBehaviour {

    // Singleton instance
	public static SafepointManager instance;

    // Used for respawning both the players. Stays activated
    [HideInInspector]
    public Transform currentTopSafepoint;
    [HideInInspector]
    public Transform currentBotSafepoint;

    // Got from Safepoints being triggered. Will deactivate if one is empty or not a matching pair.
    [HideInInspector]
    public Transform topSafepoint;
    [HideInInspector]
    public Transform botSafepoint;

    // Used as the spawning point if only one player dies but is revived
    [HideInInspector]
    public Transform topCheckpoint;
    [HideInInspector]
    public Transform botCheckpoint;

    // References to the players. Fetched from GameManager
    private Transform playerTop;
    private Transform playerBot;

    private void Awake()
	{
        CreateSingleton();
    }

	private void Start()
	{
        playerTop = GameManager.instance.playerTop;
        playerBot = GameManager.instance.playerBot;
 
        // Set the initial respawn points to be the starting position of the players.
        currentTopSafepoint = new GameObject("Initial Top Safepoint").transform;
        currentTopSafepoint.position = playerTop.position;

        currentBotSafepoint = new GameObject("Initial Bot Safepoint").transform;
        currentBotSafepoint.position = playerBot.position;

        // Set the initial revive points to be the starting position of the players.
        topCheckpoint = new GameObject("Initial Top Checkpoint").transform;
        topCheckpoint.position = playerTop.position;

        botCheckpoint = new GameObject("Initial Bot Checkpoint").transform;
        botCheckpoint.position = playerBot.position;
    }

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void SetAsActivated(Transform safepoint) 
    {
        // Set the appropriate transform, depending on Y pos (Top/Bot)
        if (safepoint.position.y > 0f)
            topSafepoint = safepoint;
        else
            botSafepoint = safepoint;

        // If both the top and bot safepoint are assigned and the bot is a child of the top (Meaning a matching pair)
        if (topSafepoint != null && botSafepoint != null && botSafepoint == topSafepoint.GetChild(0))
        {
            // If the top safepoint has the script (if it hasn't, either it is the initial spawn point or an error)
            if(currentTopSafepoint.GetComponent<SafepointParent>() != null)
                currentTopSafepoint.GetComponent<SafepointParent>().ResetSafepoint();

            // Assign the new current safepoints
            currentTopSafepoint = topSafepoint;
            currentBotSafepoint = botSafepoint;

            // Tell the parent safepoint to make them current
            currentTopSafepoint.GetComponent<SafepointParent>().MakeCurrent();

            // Null these to enable checking more of them.
            topSafepoint = null;
            botSafepoint = null;
        }
    }
    
    public void SetAsDeactivated(Transform safepoint) 
    {
        // Null the activated transforms if they match
        if(topSafepoint == safepoint)
            topSafepoint = null;
        else if(botSafepoint == safepoint)
            botSafepoint = null;
    }

    public void SetCheckpoint(Transform checkpoint, Transform player)
    {
        // Set the new checkpoints for the players
        if (player == playerTop)
            topCheckpoint = checkpoint;
        else
            botCheckpoint = checkpoint;
    }

    public void PlacePlayerOnCheckpoint(Transform player)
    {
        if(player == playerTop)
        {
            Vector2 rayOrigin = topCheckpoint.position;
            Vector2 rayDirection = Vector2.down;
            // TODO: Set this correctly. Should for example not detect the players
            //LayerMask layer = 1 << 0;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 11f);
            if (hit.transform != null)  // If it hits something, prevent the player from spawning in the ground
                player.position = hit.point + new Vector2(0f, player.GetComponent<SpriteRenderer>().bounds.size.y / 2f);    // Raise the spawnpoint by half the players height
            else
                player.position = topCheckpoint.position;   // Else, just spawn on the checkpoint
        }
        else
        {
            Vector2 rayOrigin = botCheckpoint.position;
            Vector2 rayDirection = Vector2.up;
            // TODO: Set this correctly. Should for example not detect the players
            //LayerMask layer = 1 << 0;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 11f);
            if (hit.transform != null)  // If it hits something, prevent the player from spawning in the ground
                player.position = hit.point + new Vector2(0f, -player.GetComponent<SpriteRenderer>().bounds.size.y / 2f);   // Raise the spawnpoint by half the players height
            else
                player.position = botCheckpoint.position;   // Else, just spawn on the checkpoint
        }
    }
}
