﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOnCollision : MonoBehaviour {

    [SerializeField]
    [FMODUnity.EventRef]
    private string eventPath;

    [SerializeField]
    private float magnitude = 2f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.relativeVelocity.magnitude > magnitude)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(eventPath, gameObject);
        }
    }
}