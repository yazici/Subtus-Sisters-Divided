﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveSpot : MonoBehaviour {

    Transform playerTransform;

    public void Initialize(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerTransform.GetComponent<PlayerController>().Ressurect();
            Destroy(gameObject);
        }
    }
}