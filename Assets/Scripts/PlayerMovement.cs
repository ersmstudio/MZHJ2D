using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public Rigidbody2D playerRb;
    Vector2 movement;
    void Update()
    {
        //input and control
     
       movement.x= Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }
    void FixedUpdate()
    {
        //movement
        playerRb.MovePosition(playerRb.position+movement*moveSpeed*Time.fixedDeltaTime);

    }
}
