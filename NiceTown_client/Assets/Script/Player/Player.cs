using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movmentInput;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        movmentInput = new Vector2(inputX, inputY).normalized;
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
}

