﻿using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
    #region Properties
    public bool IsJumping { get; set; }
    public bool IsSuiciding { get; set; }
    public bool isGrounded { get; private set; }
    #endregion

    #region Variable
    private Vector3 playerVelocity;
    private float currentGravity;
    private bool isFalling;
    private float actionTimer;

    [Header("Extra Player")]
    [SerializeField]
    private CharacterController charController;
    [SerializeField]
    private PlayerConfig pConfig;


    private IAction defaultAction;
    private BaseConfig defaultConfig;

    #endregion

    private void Start()
    {
        currentGravity = config.gravityForce;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.right * 5, Color.blue);
        isGrounded = charController.isGrounded;
        playerVelocity.x = direction * config.speed;
        CheckInput();

    }

    #region Mouvement
    private void FixedUpdate()
    {
        if (canMove)
        {
            charController.Move(playerVelocity * Time.fixedDeltaTime);
        }
            
        if (playerVelocity.x > 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (playerVelocity.x < 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
        }

        if (!isGrounded)
        {
            currentGravity = Mathf.Min((currentGravity * config.gravityMultiplier), config.gravityLimit);
            playerVelocity.y -= currentGravity;
            isFalling = true;
        }
        if (isGrounded && isFalling)
        {
            Debug.Log("landed");
            currentGravity = config.gravityForce;
            isFalling = false;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = config.jumpForce;
            Debug.Log("Y velocity added :" + config.jumpForce);
        }
    }
    #endregion

    #region Input

    private void CheckInput()
    {
        if (IsJumping)
        {
            Jump();
            IsJumping = false;
        }

        if (IsInAction)
        {
            Action();
        }

        if (IsSuiciding)
        {
            Suicide();
        }
    }
    private void Action()
    {
        if (actionTimer == 0)
        {
            actionController?.UseAction(this, transform, config.actionDuration, config.actionRange);
        }
        else
        {
            actionTimer += Time.deltaTime;
            if (actionTimer > config.actionCooldown)
            {
                IsInAction = false;
                actionTimer = 0;
            }
        }
    }

    private void Suicide()
    {

    }
    #endregion

    public override void ChangeIdentity(Controllable controllable)
    {
        base.ChangeIdentity(controllable);
        charController.radius = controllable.identity.collider.radius;
        charController.height = controllable.identity.collider.height;
    }
}
