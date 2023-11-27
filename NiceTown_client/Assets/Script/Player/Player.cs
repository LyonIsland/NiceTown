using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movmentInput;
    private bool isMoving;
    private Animator[] animators;
    
    private void Awake()
    { 
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    
    }
    private void PlayerInput()
    {
        if(Input.GetKey(KeyCode.LeftShift)){
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");
            movmentInput = new Vector2(inputX, inputY).normalized;
        }else{
            inputX = Input.GetAxisRaw("Horizontal")*0.5f;
            inputY = Input.GetAxisRaw("Vertical")*0.5f;
            movmentInput = new Vector2(inputX, inputY).normalized*0.5f;
        }
        
        
        isMoving = movmentInput != Vector2.zero;
    }
    private void Movement()
    {
        rb.MovePosition(rb.position + movmentInput * speed * Time.deltaTime);

    }

    private void FixedUpdate()
    {                                                
        PlayerInput();
        Movement();
    }

    private void Update(){
        SwitchAnimation();
    }


    private void SwitchAnimation()
    {
        foreach(var anim in animators)
        {
            anim.SetBool("isMoving",isMoving);
            if (isMoving)
            {
                anim.SetFloat("InputX",inputX);
                anim.SetFloat("InputY",inputY);
            }
        }
    }
}

