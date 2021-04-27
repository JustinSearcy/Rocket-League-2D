using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Forces")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpSpeed = 1f;
    [SerializeField] private float doubleJumpSpeed = 1f;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private float flipRotateSpeed = 1f;
    [SerializeField] private float flipSpeed = 1f;
    [SerializeField] private float boostSpeed = 1f;

    [Header("Other Movement Values")]
    [SerializeField] private float flipTimer = 1.5f;
    [SerializeField] private float maxFlipTime = 1.5f;
    [SerializeField] private float flipDuration = 1f;

    [Header("Collision Detection")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float heightVariance = 0.03f;
    [SerializeField] private float hoodHeightVariance = 0.03f;
    [SerializeField] private float halfColliderHeight = 0.33f;

    [Header("Visuals and Effects")]
    [SerializeField] private ParticleSystem backTireTrailParticles = null;
    [SerializeField] private ParticleSystem frontTireTrailParticles = null;
    [SerializeField] private float trailParticleRequiredSpeed = 1f;
    [SerializeField] private ParticleSystem boostParticles = null;

    private PlayerControls controls;
    private Rigidbody2D rb;
    private new PolygonCollider2D collider;

    private float xForce = 0;

    private float forwardValue = 0;
    private float reverseValue = 0;
    private float driveValue = 0;
    private float boostValue = 0;
    private Vector2 rotateValue;
    private int flipDirection = 1;

    private bool flipUsed = true;
    private bool isFlipping = false;
    private bool isPlaying = false;
    private bool gameActive = false;

    private bool isBlue = false;

    GameManager gameManager;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
        StopParticles();
        gameManager = FindObjectOfType<GameManager>();

        if(this.gameObject.tag == "Blue Player")
        {
            isBlue = true;
        }

        SetControls();
    }

    private void StopParticles()
    {
        backTireTrailParticles.Stop();
        frontTireTrailParticles.Stop();
        boostParticles.Stop();
    }

    private void SetControls()
    {
        controls.Gameplay.Drive.performed += cntxt => forwardValue = cntxt.ReadValue<float>();
        controls.Gameplay.Drive.canceled += cntxt => forwardValue = 0f;

        controls.Gameplay.Reverse.performed += cntxt => reverseValue = cntxt.ReadValue<float>();
        controls.Gameplay.Reverse.canceled += cntxt => reverseValue = 0f;

        controls.Gameplay.Rotate.performed += cntxt => rotateValue = cntxt.ReadValue<Vector2>();
        controls.Gameplay.Rotate.canceled += cntxt => rotateValue = Vector2.zero;

        controls.Gameplay.Jump.performed += cntxt => Jump();

        controls.Gameplay.Boost.performed += cntxt => boostValue = cntxt.ReadValue<float>();
        controls.Gameplay.Boost.canceled += cntxt => boostValue = 0f;
        controls.Gameplay.Boost.performed += cntxt => StartBoostParticles();
        controls.Gameplay.Boost.canceled += cntxt => StopBoostParticles();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        FlipTimer();
        DisplayTrailParticles();
    }

    private void FlipTimer()
    {
        if (!flipUsed)
        {
            flipTimer -= Time.deltaTime;
        }
    }

    private void DisplayTrailParticles()
    {
        if (!IsGrounded())
        {
            isPlaying = false;
            backTireTrailParticles.Stop();
            frontTireTrailParticles.Stop();
        }
        else if(IsGrounded() && Mathf.Abs(rb.velocity.x) >= trailParticleRequiredSpeed && !isPlaying)
        {
            isPlaying = true;
            backTireTrailParticles.Play();
            frontTireTrailParticles.Play();
        }
        else if(IsGrounded() && Mathf.Abs(rb.velocity.x) < trailParticleRequiredSpeed && isPlaying)
        {
            isPlaying = false;
            backTireTrailParticles.Stop();
            frontTireTrailParticles.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (gameActive)
        {
            Drive();
            Rotate();
            Boost();
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(collider.bounds.center, -gameObject.transform.up, halfColliderHeight + heightVariance, groundLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(collider.bounds.center, -gameObject.transform.up * (halfColliderHeight + heightVariance), rayColor);

        return raycastHit.collider != null;
    }

    private bool IsOnHood()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(collider.bounds.center, gameObject.transform.up, halfColliderHeight + hoodHeightVariance, groundLayerMask);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(collider.bounds.center, gameObject.transform.up * (halfColliderHeight + hoodHeightVariance), rayColor);

        return raycastHit.collider != null;
    }

    private void Drive()
    {
        if (IsGrounded())
        {
            driveValue = forwardValue - reverseValue;
            xForce = driveValue * moveSpeed * Time.deltaTime;
            Vector2 moveForce = gameObject.transform.right.normalized * xForce;

            if (isBlue)
            {
                rb.AddRelativeForce(moveForce);
            }
            else
            {
                rb.AddRelativeForce(-moveForce);
            }
            
        }
    }

    private void Rotate()
    {
        if (!IsGrounded())
        {
            if (!isFlipping)
            {
                gameObject.transform.Rotate(new Vector3(0, 0, -rotateValue.x * rotateSpeed));
            }
            else if (isFlipping)
            {
                gameObject.transform.Rotate(new Vector3(0, 0, flipDirection * flipRotateSpeed));
            }
        }
    }

    private void Boost()
    {
        if(boostValue > 0 && gameActive)
        {
            Vector2 boostForce = new Vector2(boostSpeed * Time.deltaTime, 0);

            if (isBlue)
            {
                rb.AddRelativeForce(boostForce);
            }
            else
            {
                rb.AddRelativeForce(-boostForce);
            }
        }
    }

    private void Jump()
    {
        if (gameActive)
        {
            if (IsGrounded())
            {
                Vector2 jumpForce = transform.up * jumpSpeed;
                rb.velocity += jumpForce;
                flipTimer = maxFlipTime;
                flipUsed = false;
            }
            else if (!flipUsed && flipTimer > 0 && rotateValue.x <= Mathf.Epsilon && rotateValue.x >= -Mathf.Epsilon) //Double jump
            {
                Vector2 jumpForce = Vector2.up * doubleJumpSpeed;
                rb.velocity += jumpForce;
                flipUsed = true;
            }
            else if (!flipUsed && flipTimer > 0 && rotateValue.x >= Mathf.Epsilon) //Flip right
            {
                flipDirection = -1;
                StartCoroutine(Flip());
            }
            else if (!flipUsed && flipTimer > 0 && rotateValue.x <= Mathf.Epsilon) //Flip left
            {
                flipDirection = 1;
                StartCoroutine(Flip());
            }
            else if (IsOnHood())
            {
                Vector2 jumpForce = -transform.up * jumpSpeed;
                rb.velocity += jumpForce;
                flipTimer = maxFlipTime;
                flipUsed = false;
            }
        } 
    }

    IEnumerator Flip()
    {
        Vector2 flipForce = new Vector2(-flipDirection * flipSpeed, 0);
        rb.AddForce(flipForce, ForceMode2D.Impulse);
        isFlipping = true;
        yield return new WaitForSeconds(flipDuration);
        isFlipping = false;
    }

    private void StartBoostParticles()
    {
        boostParticles.Play();
    }

    private void StopBoostParticles()
    {
        boostParticles.Stop();
    }

    public void GameStarted()
    {
        gameActive = true;
    }

    public void GameStopped()
    {
        gameActive = false;
    }
}
