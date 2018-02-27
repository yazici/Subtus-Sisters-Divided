﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Controller
{
    Player1, Player2
}

public enum AirControl
{
    Full, Semi, SemiFull, None
}

public class PlayerController : MonoBehaviour
{
    // Saved State for Different Jumping states.
    private float X;
    private Vector2 savedVelocity;
    private bool changedMade;
    private bool inAir;
    float temp;
    private float wallNormal = 0.9f;

    // Input Manager 
    public Controller Player;
    [NonSerialized]
    public string controllerCode;

    public const string controllerOne = "_C1";
    public const string controllerTwo = "_C2";

    private string horAx = "Horizontal";
    private string verAx = "Vertical";
    private string jumpInput = "Jump";
    private float distanceGraceForJump = 0.02f; // how faar outside the boxcollider do you want the ray to travel when reseting jump?

    // Components
    private CapsuleCollider2D myBox;
    private float capsuleRadiusX;
    private float capsuleRadiusY;
    private float capsuleOffSetX;
    private float capsuleOffSetY;

    private BasicAnimator bodyAnim;
    private BasicAnimator armAnim;
    private new Rigidbody2D rigidbody2D;
    private RaycastHit2D[] objHit;

    // Settings
    [HideInInspector]
    public bool isActive;

    bool landing;

    private SpriteRenderer sr;

    [Header("Physics")]

    private bool flipped;
    private int flippValue = 1; // Value between 1 and -1

    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpVelocity;
    [SerializeField]
    private AirControl AirControl;
    [SerializeField]
    [Range(0, 1)]
    private float AirControlPrecentage;

    [GiveTag]
    [SerializeField]
    private string[] resetJumpOn = new string[] { };

    [Header("Sounds")]
    [SerializeField]
    private AudioClip jumpSound;
    [SerializeField]
    private AudioClip landingSound;
    private AudioSource audioSource;

    [Header("Reviving")]
    [SerializeField]
    private Vector2 lastSafe;
    [SerializeField]
    private GameObject revivePlacerPrefab;
    private GameObject revivePlacer;
    [SerializeField]
    private GameObject dyingAnimationGO;

    void Awake()
    {
        myBox = GetComponent<CapsuleCollider2D>();

        capsuleRadiusX = myBox.size.x * transform.localScale.x / 2;
        capsuleRadiusY = myBox.size.y * transform.localScale.y / 2;

        capsuleOffSetX = myBox.offset.x;
        capsuleOffSetY = myBox.offset.y;

        // FIX JUMP
        //
        // Arm, ThrowArm, Hide sprite renderer ocn "Arm" when attacking, show "ThrowArm" wait until throw is done, then hide throw arm
        // and show normal "arm" again.
        //
        bodyAnim = GetComponent<BasicAnimator>();
        GameObject arm = this.transform.Find("Arm").gameObject;
        armAnim = arm.GetComponent<BasicAnimator>();
        //
        //

        rigidbody2D = GetComponent<Rigidbody2D>();


        if (Player == Controller.Player1)
        {
            controllerCode = controllerOne;
            flipped = false;
        }
        else
        {
            controllerCode = controllerTwo;
            flipped = true;
        }

        horAx += controllerCode;
        verAx += controllerCode;
        jumpInput += controllerCode;

        if (flipped)
            Flip();

        isActive = true;
        inAir = true;

        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isActive)
        {
            ResetJump();
        }

        if(bodyAnim.GetLandState() == false)
        {
            landing = false;
        }

        //  if (!inAir)
        //  lastSafe = transform.position;
    }
    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();
            NormalizeSlope();
        }
    }

    private void Move()
    {
        // JUMP
        if (Input.GetAxis(jumpInput) > 0 && (!inAir) && landing == false)
        {
            inAir = true;
            rigidbody2D.velocity = Vector2.up * flippValue * jumpVelocity;


            bodyAnim.Jump();
            armAnim.Jump();

            Debug.Log("audio being played");
            audioSource.PlayOneShot(jumpSound); 
        }

        // Input Manager
        X = Input.GetAxis(horAx); // Valute between 0 and 1 from input manager.
        float Y = GetComponent<Rigidbody2D>().velocity.y;

        // Round it to nearest .5
        temp = X;
        temp = (float)Math.Round(temp * 2, MidpointRounding.AwayFromZero) / 2;

        if (Mathf.Abs(temp) <= 0.5f)
        {
            bodyAnim.ToggleWalk(true);
            armAnim.ToggleWalk(true);
        }
        else
        {
            bodyAnim.ToggleWalk(false);
            armAnim.ToggleWalk(false);
        }

        if (!inAir)
            temp *= speed;

        // Fixing all the Jumping and shit
        ControllingAir();

        // Creating SavedVelocity.
        if (!inAir)
        {
            rigidbody2D.velocity = new Vector2(temp, Y);
            savedVelocity = rigidbody2D.velocity;
            changedMade = false;
        }

        //
        //
        bodyAnim.Walking(new Vector2(temp, Y), true);
        armAnim.Walking(new Vector2(temp, Y), false);
        //
        //

        rigidbody2D.velocity = new Vector2(temp, Y);
    }

    private void ControllingAir()
    {
        // Full Controll
        if (AirControl == AirControl.Full && inAir)
            temp *= speed * AirControlPrecentage;

        // Semi Controll 
        if (inAir && AirControl != AirControl.Full)
        {
            // If it's not Semi it's No controll, in which case it will just use, temp = savedvelocity.
            if (AirControl == AirControl.Semi)
            {
                if (temp > 0) // go right
                {
                    if (savedVelocity.x < 0 && !changedMade)
                    {
                        savedVelocity.x += savedVelocity.x * AirControlPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * speed * AirControlPrecentage;
                        changedMade = true;
                    }
                }
                else if (temp < 0)
                {
                    if (savedVelocity.x > 0 && !changedMade)
                    {
                        savedVelocity.x += savedVelocity.x * AirControlPrecentage * -1;
                        changedMade = true;
                    }
                    else if (savedVelocity.x == 0 && !changedMade)
                    {
                        savedVelocity.x += temp * speed * AirControlPrecentage;
                        changedMade = true;
                    }

                }
            }
            temp = savedVelocity.x;
        }
    }

    public void Die()
    {
        if (isActive)
        {
            isActive = false;
            gameObject.SetActive(false);

            // If any of the players is dead and the last dies, call BothDead() instead.
            if (GameManager.instance.onePlayerDead)
            {
                BothDead();
            }
            else
            {
                GameObject dead;
                dead = Instantiate(dyingAnimationGO, transform.position, transform.rotation);
                dead.transform.localScale = transform.localScale;

                Destroy(dead, dead.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

                Vector2 spawnPos = new Vector2(lastSafe.x, 0f);
                revivePlacer = Instantiate(revivePlacerPrefab, spawnPos, Quaternion.identity);
                revivePlacer.GetComponent<RevivePlacer>().Initialize(Player, transform);
                Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingOne, transform);
                GameManager.instance.onePlayerDead = true;
            }
        }
    }

    private void Revive()
    {
        Transform player;

        if (Player == Controller.Player1)
            player = GameManager.instance.playerBot;
        else
            player = GameManager.instance.playerTop;

        SafepointManager.instance.PlacePlayerOnCheckpoint(player);
        player.gameObject.SetActive(true);
        player.GetComponent<PlayerController>().isActive = true;

        Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingBoth);

        GameManager.instance.onePlayerDead = false;
    }

    private void BothDead()
    {
        // Delete all revive objects, if there are any
        GameObject[] revives = GameObject.FindGameObjectsWithTag("Revive");
        foreach (GameObject revive in revives)
            Destroy(revive);

        // Get the player transforms
        Transform playerTop = GameManager.instance.playerTop;
        Transform playerBot = GameManager.instance.playerBot;

        // Activate them
        playerTop.gameObject.SetActive(true);
        playerBot.gameObject.SetActive(true);

        // Place them on the current safepoints
        playerTop.transform.position = SafepointManager.instance.currentTopSafepoint.position;
        playerBot.transform.position = SafepointManager.instance.currentBotSafepoint.position;

        // Set the camera to follow both
        Camera.main.GetComponent<CameraController>().SetCameraState(CameraState.FollowingBoth, transform);

        GameManager.instance.onePlayerDead = false;

        playerTop.GetComponent<PlayerController>().isActive = true;
        playerBot.GetComponent<PlayerController>().isActive = true;
    }

    private void ResetJump()
    {
        // If we asume we're always falling until told otherwise we get a more proper behaviour when falling off things.

        bool tempInAir = true;

        // transform.parent = null;

        //
        //
        bodyAnim.Falling(true);
        armAnim.Falling(true);
        //
        //

        for (int i = 0; i < resetJumpOn.Length; i++)
        {
            bool quickBreak = false;

            for (int l = -1; l < 2; l += 2)
            {
                if (quickBreak)
                    break;

                /*
                (transform.position + new Vector3((rayOffSetX - 0.05f) * l, 0, 0),
                -Vector2.up * flipValue,
                rayOffSetY + distanceGraceForFalling); */

                objHit = Physics2D.RaycastAll(transform.position + new Vector3((capsuleRadiusX - 0.05f) * l, 0, 0) + new Vector3(capsuleOffSetX * transform.localScale.x, capsuleOffSetY * flippValue, 0),
                    -Vector2.up * flippValue,
                capsuleRadiusY + distanceGraceForJump);

                Debug.DrawRay(transform.position + new Vector3((capsuleRadiusX - 0.05f) * l, 0, 0) + new Vector3(capsuleOffSetX * transform.localScale.x, capsuleOffSetY * flippValue, 0),
                    -Vector2.up * flippValue,
                    Color.red);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag == resetJumpOn[i])
                    {
                        if (Mathf.Abs(objHit[j].normal.x) < wallNormal) // So we cant jump on walls.
                        {

                            if (inAir)
                            {
                                // LANDING
                                //play land sound
                                if (bodyAnim.GetJumpState() == false)
                                {
                                    Debug.Log("LANDING");

                                    bodyAnim.Land();

                                    audioSource.PlayOneShot(landingSound, 0.1f);
                                    landing = true;

                                }

                                // play land animation

                            }

                            /*
                            if (resetJumpOn[i] == "MovingFloor" || objHit[j].transform.GetComponent<MovingPlatform>() != null)
                                transform.parent = objHit[j].transform;
*/
                            // inAir = false;
                            tempInAir = false;
                            lastSafe = transform.position;

                            //
                            //
                            bodyAnim.Falling(false);
                            armAnim.Falling(false);
                            //
                            //

                            quickBreak = true;
                        }
                    }
                }
            }
            inAir = tempInAir;
        }
    }

    public void Flip()
    {
        // mabey flip sprite insted??
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.x);
        rigidbody2D.gravityScale *= -1; // E.V move this to portal ??
        flippValue *= -1;
    }

    void NormalizeSlope()
    {
        float slopeFriction = 0.11f;
        // Small optimization (if first ray hit we dont raycast a second time).
        bool slope = false;

        // Raycasting twice, meanwhile using L as direction because we want L to be between -1 and 1 so we raycast left and right...
        for (int l = -1; l < 2; l += 2)
        {
            // Attempt vertical normalization
            for (int i = 0; i < resetJumpOn.Length; i++)
            {
                objHit = Physics2D.RaycastAll(transform.position, new Vector2(l, -flippValue),
                    Mathf.Abs((myBox.size.y * GetComponent<Transform>().localScale.y)) / 2 + distanceGraceForJump);

                for (int j = 0; j < objHit.Length; j++)
                {
                    if (objHit[j].transform.tag != resetJumpOn[i])
                        continue;

                    if (objHit[j].collider != null && Mathf.Abs(objHit[j].normal.x) > 0.1f && Mathf.Abs(objHit[j].normal.x) < wallNormal && inAir == false)
                    {


                        slopeFriction *= Mathf.Abs(objHit[j].normal.x);

                        Rigidbody2D body = GetComponent<Rigidbody2D>();
                        // Apply the opposite force against the slope force 
                        body.velocity = new Vector2(body.velocity.x - (objHit[j].normal.x * slopeFriction), body.velocity.y);

                        //Move Player up or down to compensate for the slope below them
                        Vector3 pos = transform.position;
                        float offSet = 0;
                        //              "-1"          normalen     *       hastigheten          *       deltatime     *  hastigheten - (1 / -1)
                        offSet += (flippValue * -1) * objHit[j].normal.x * Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x - objHit[j].normal.x > 0 ? 1f : -1f);

                        if (offSet * flippValue > 0)
                            offSet *= 0.5f;
                        else
                            offSet *= 2;

                        pos.y += offSet;
                        transform.position = pos;

                        inAir = false;
                        slope = true;

                    }
                }
            }
            if (slope)
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Portal")
        {
            Die();
        }
        else if (collision.transform.tag == "Revive")
        {
            Revive();
            Destroy(collision.gameObject);
        }
    }
}
