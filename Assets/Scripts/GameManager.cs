﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Header("Instantiated Players")]
    public Transform playerTop;
    public Transform playerBot;

    [Header("Prefab Players")]
    public GameObject playerTopPrefab;
    public GameObject playerBotPrefab;

    [Header("Game Variables")]
    public bool onePlayerDead = false;

    [Header("Screen")]
    [SerializeField]
    private int resolutionX = 1920;
    [SerializeField]
    private int resolutionY = 1080;
    [Header("Keys")]
    [SerializeField]
    private KeyCode resetKey;
    [SerializeField]
    private KeyCode pauseKey;
    [SerializeField]
    private KeyCode killKey;

    [Header("Pausing")]
    [SerializeField]
    private Canvas pauseMenu;
    public bool isPaused = false;
    [SerializeField]
    private float player1A = 0f;
    [SerializeField]
    private float player2A = 0f;
    private float fillRate = 1f;
    private float emptyRate = 0.4f;

    private void Awake()
    {
        CreateSingleton();
        SetStartResolution(resolutionX, resolutionY);
    }

    private void Start()
    {
        if (playerTop == null)
        {
            Debug.LogError("PlayerTop not assigned on Start() in GameManager!");
            playerTop = GameObject.Find("PlayerTop").transform;
        }
        if (playerBot == null)
        {
            Debug.LogError("PlayerBot not assigned on Start() in GameManager!");
            playerBot = GameObject.Find("PlayerBot").transform;
        }
        if (playerTopPrefab == null)
            Debug.LogError("PlayerTopPrefab not assigned on Start() in GameManager!");
        if (playerBotPrefab == null)
            Debug.LogError("PlayerBotPrefab not assigned on Start() in GameManager!");

        pauseMenu.gameObject.SetActive(false);
        onePlayerDead = false;
    }

    private void Update () {
        RestartKey();
        PauseKey();
        KillKey();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadScene(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadScene(2);

        if (isPaused)
        {
            // Holding A to fill the progress bars
            if (Input.GetAxis("Jump_C1") > 0f || Input.GetKey(KeyCode.U))
                player1A += Time.deltaTime * fillRate;
            else
                player1A -= Time.deltaTime * emptyRate; // Empty if not holding A

            if (Input.GetAxis("Jump_C2") > 0f || Input.GetKey(KeyCode.I))
                player2A += Time.deltaTime * fillRate;
            else
                player2A -= Time.deltaTime * emptyRate;

            player1A = Mathf.Clamp(player1A, 0f, 1f);
            player2A = Mathf.Clamp(player2A, 0f, 1f);
        }
    }

    private void CreateSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void SetStartResolution(int resX, int resY)
    {
        Screen.SetResolution(resX, resY, true);
    }

    private void RestartKey()
    {
        if (Input.GetKeyDown(resetKey))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PauseKey()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            Application.Quit();
            //pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
            // Use something else instead
            //Time.timeScale = pauseMenu.gameObject.activeSelf ? 0f : 1f;
            //isPaused = pauseMenu.gameObject.activeSelf;
        }
    }

    private void KillKey()
    {
        if(Input.GetKeyDown(killKey))
        {
            playerTop.GetComponent<Reviving>().Die();
            onePlayerDead = true;
            playerBot.GetComponent<Reviving>().Die();
        }
    }

    public float PreventZero(float value) 
    {
        value = value < 0.01f ? 0.01f : value;
        return value;
    }

    public float PreventZero(float value, float min)
    {
        value = value < min ? min : value;
        return value;
    }

    public float PreventZero(float value, float min, float newValue)
    {
        value = value < min ? newValue : value;
        return value;
    }
}
