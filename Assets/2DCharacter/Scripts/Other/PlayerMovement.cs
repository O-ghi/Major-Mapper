using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // ~~ 1. Controls All Player Movement
    // ~~ 2. Updates Animator to Play Idle & Walking Animations

    private float speed = 4f;
    private CharacterController controller;
    private Vector3 playerMovement;
    private Animator animator;
    private PlayerInput playerInput;

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        //if(DialogueManager.GetInstance().dialogueIsPlaying)
        //{
        //    return;
        //}
        playerMovement = InputManager.GetInstance().GetMoveDirection() ;
        controller.Move(playerMovement * speed * Time.deltaTime);

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
