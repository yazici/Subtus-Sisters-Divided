﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    [SerializeField]
    private bool doClamp = true;

    private Camera myCam;
    private GameManager gM;

    private GameObject pTop;
    private GameObject pBot;

    float height;
    float width;

    // Use this for initialization
    void Start()
    {
        myCam = Camera.main;
        gM = GameManager.instance;

        pTop = gM.playerTop.transform.gameObject;
        pBot = gM.playerBot.transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (doClamp)
        {
            FetchCameraInfo();

            ClampObject(pTop);
            ClampObject(pBot);
        }
    }

    private void FetchCameraInfo()
    {
        height = 2f * myCam.orthographicSize;
        width = height * myCam.aspect;
    }

    public void ClampObject(GameObject obj)
    {
        Vector3 pos = obj.transform.position;

        float X = obj.transform.position.x;
        pos.x = Mathf.Clamp(X, myCam.transform.position.x - (width * 0.5f) + 0.5f, myCam.transform.position.x + (width * 0.5f) - 1f);
        obj.transform.position = pos;
    }

    public void SetClamp(bool clamp)
    {
        doClamp = clamp;
    }
}
