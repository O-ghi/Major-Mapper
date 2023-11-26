using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // ~~ 1. Controls All Player Movement
    // ~~ 2. Updates Animator to Play Idle & Walking Animations

    private float speed = 5f;
    private Rigidbody2D rigidbody2D;
    private Vector3 playerMovement;
    private Animator animator;
    private PlayerInput playerInput;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //if(DialogueManager.GetInstance().dialogueIsPlaying)
        //{
        //    return;
        //}
        playerMovement = InputManager.GetInstance().GetMoveDirection() ;
        rigidbody2D.MovePosition(transform.position + playerMovement * speed * Time.fixedDeltaTime);

        //playerMovement.x = Input.GetAxisRaw("Horizontal");
        //playerMovement.y = Input.GetAxisRaw("Vertical");
        UpdateAnimationAndMove();
    }

    private void UpdateAnimationAndMove()
    {
        if (playerMovement != Vector3.zero)
        {
            animator.SetFloat("moveX", playerMovement.x);
            animator.SetFloat("moveY", playerMovement.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
