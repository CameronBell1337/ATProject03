using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput input;
    private CharacterController controller;

    public float gravity = -9.81f;
    public float speed = 10.0f;
    public float velY;
    public bool test = false;

    float currentSpeed;
    float speedSVel;

    Vector3 velocity;
    Vector3 playerDir;

    void Start()
    {
        controller = FindObjectOfType<CharacterController>();
        input = FindObjectOfType<PlayerInput>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (controller != null)
        {
            PlayerMovement();
        }
    }

    void PlayerMovement()
    {
        
        Vector3 playerMov = new Vector3(input.horizontal, input.vertical, velY);
        playerDir = playerMov.normalized;

        velY += Time.deltaTime * gravity;
        velocity = transform.forward * input.vertical + Vector3.up * velY + transform.right * input.horizontal;
        
        controller.Move(velocity * speed * Time.deltaTime);
        //gameObject.transform.localEulerAngles = input.moveCharacter;
        if (controller.isGrounded)
        {
            velY = 0f;
        }
    }

    public float GetVelocity()
    {
        if (controller == null)
        {
            return 1f;
        }
        else
        {
            currentSpeed = playerDir.magnitude;
            return currentSpeed;
        }
    }
}
