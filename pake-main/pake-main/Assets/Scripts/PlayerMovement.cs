using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{

    public CharacterController controller;

    public float Speed = 0f;
    public float StartSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 0.75f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    [SerializeField] private float sideMovementMultiplier;
    [SerializeField] private float backMovementMultiplier;
    [SerializeField] private GameObject SoldierMeshBody;
    public bool isCrouching;
    public bool Idle;
    public bool isMoving = false;

    Vector3 velocity;

    bool isGrounded;

    [SerializeField]
    private Animator anim;


    private void Start()
    {
        if (isLocalPlayer)
        {
            Speed = StartSpeed;
            //SoldierMeshBody.GetComponent<SkinnedMeshRenderer>().enabled = false;
            SoldierMeshBody.GetComponent<MeshRenderer>().enabled = false;
        }
    }


    void Update()
    {
        if (isLocalPlayer)
        {
            HandleMovement();
            Jump();
            Crouch();
            Sprint();

            //if (anim.GetBool("isWalking") == false && anim.GetBool("isSprinting") == false)
            //{
            //    Idle = true;
            //}
            //else
            //{
            //    Idle = false;
            //}           
        }
    }

    //Metodi hypylle
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            AudioManager.instance.Play("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);


            //if (Idle == true)
            //{
            //    anim.SetTrigger("Jump");

            //}

        }
    }

    void Sprint()
    {
        if (!isCrouching)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                AudioManager.instance.Play("Walk");
                //anim.SetBool("isSprinting", true);
                Speed = 12.5f;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
               
                AudioManager.instance.StopPlay("Walk");
                //anim.SetBool("isSprinting", false);
                Speed = StartSpeed;
            }
            //if (anim.GetBool("isWalking") == true)
            //{
            //} else
            //{
            //    anim.SetBool("isSprinting", false);
            //}
        }
    }
    //Metodi Crouchille
    void Crouch()
    {        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            Speed = 2.8f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            Speed = StartSpeed;
        }
    }


    void HandleMovement()
    {
        if (isCrouching)
        {
            //anim.SetBool("Squat", true);
            anim.SetBool("IsCrouching", true);
        }
        else
        {
            //anim.SetBool("Squat", false);
            anim.SetBool("IsCrouching", false);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        x *= sideMovementMultiplier;

        if (z <= 0)
        {
            z *= backMovementMultiplier;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        if (move.x != 0 || move.z != 0)
        {
            isMoving = true;
            anim.SetBool("isMoving", true);
            //anim.SetBool("isWalking", true);
        }
        else
        {
            isMoving = false;
            anim.SetBool("isMoving", false);
            //anim.SetBool("isWalking", false);
        }

        controller.Move(move * Speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime * 2;

        controller.Move(velocity * Time.deltaTime);
    }


}
