using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{

    //Moving
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving 
    {
        get => _isMoving;
        private set
        {
            _isMoving = value;

           animator.SetBool(AnimationStrings.isMoving, _isMoving);
        }
    }
    public float moveSpeed = 5f;
    private Vector2 moveInput;

    //Jumping
    public float JumpSpeed = 10f;
    public float airSpeed = 3f;

    public float CurrentMoveSpeed
    {
        get
        {
            if (IsMoving && !touchingDirectionsScript.IsOnWall)
            {
                if (touchingDirectionsScript.IsGrounded)
                {
                    return moveSpeed;
                }
                else 
                    return airSpeed = moveSpeed * 0.7f;
            }
            else
                return 0;
        }
    }

    private TouchingDirections touchingDirectionsScript;
    private Rigidbody2D rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirectionsScript = GetComponent<TouchingDirections>();
    }

    void Start()
    {

    }


    void Update()
    {

    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * Mathf.Clamp(moveSpeed, 0f, 200f), rb.velocity.y);


    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;

        
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirectionsScript.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.jump);

            rb.velocity = new Vector2(moveSpeed * 3, JumpSpeed);
            
        }
    }


}


