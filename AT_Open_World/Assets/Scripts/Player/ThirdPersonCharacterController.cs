using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour
{

    public float speedSTime = 0.2f;
    public float turnST = 0.14f;
    public float jumpForce = 5.5f;
    public float gravity = -38.0f;
    public float currentSpeed;
    public int maxJumpCount = 2;
    public int currentJumpCount;

    float speedSVel;
    float velocityY;
    float characterSpeed;
    float runSpeed = 9.0f;
    float walkSpeed = 5.0f;
    float currentJumpHeight;
    float duration = 1f;
    public float currentTime = 0.0f;

    public bool doubleJumpEnabled;
    public bool attacking;

    bool isDoubleJumping = false;

    Vector3 velocity;
    Vector3 currentRot;
    Vector3 rotSmoothVel;

    public Animator anim;

    public LayerMask groundLayers;
    Transform camT;
    public CharacterController col;

    void Start()
    {
        anim = GetComponent<Animator>();
        camT = Camera.main.transform;
        doubleJumpEnabled = false;

        col = GetComponent<CharacterController>();
        

        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        playerMovement();
        if (col.isGrounded)
        {
            onAttack();
        }

        anim.SetInteger("jumpCounter", currentJumpCount);
        anim.SetBool("isDoubleJumping", isDoubleJumping);


        if (currentJumpCount == 2 && velocityY <= 10)
        {
            isDoubleJumping = false;
        }


        if (doubleJumpEnabled && maxJumpCount > currentJumpCount && Input.GetKeyDown("joystick button 0"))
        {
            jump();
        }
        else if (doubleJumpEnabled && maxJumpCount > currentJumpCount && Input.GetKeyDown("space"))
        {
            jump();
        }

        if (!doubleJumpEnabled && col.isGrounded && Input.GetKey("joystick button 0"))
        {
            jump();
        }
        else if (!doubleJumpEnabled && col.isGrounded && Input.GetKey("space"))
        {
            jump();
        }

        if (doubleJumpEnabled)
        {

            anim.SetBool("isdoubleJumpTrue", true);
        }





    }

    void FixedUpdate()
    {

        float rJoyStickX = Input.GetAxisRaw("rHorizontal");
        float mouseX = Input.GetAxisRaw("Mouse X");


        if (rJoyStickX > 0 || rJoyStickX < 0)
        {
            rotateCharacter(rJoyStickX);

        }
        else
        {
            rotateCharacter(mouseX);
        }

    }

    void playerMovement()
    {
        velocityY += Time.deltaTime * gravity;

        float horiztonal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float rHorizontal = Input.GetAxisRaw("rHorizontal");
        bool shift = Input.GetKey("left shift");
        bool lStick = Input.GetKey("joystick button 8");
        bool jumping = Input.GetKey("joystick button 0");
        bool bButton = Input.GetKey("joystick button 1");
        float mouseX = Input.GetAxisRaw("Mouse X");


        anim.SetFloat("velY", velocityY, turnST, Time.deltaTime);
        anim.SetFloat("Hor", horiztonal, turnST, Time.deltaTime);
        anim.SetFloat("Vert", vertical, turnST, Time.deltaTime);
        anim.SetBool("isShiftPressed", (shift || lStick));
        



        bool rSticInUse = false;
        bool lStickInUse = false;

        if (rHorizontal > 0.5f || rHorizontal < -0.5f)
        {
            rSticInUse = true;

        }

        if (mouseX > 0f || mouseX < 0f)
        {
            rSticInUse = true;

        }

        if (vertical > 0 || vertical < 0)
        {
            lStickInUse = true;

        }

        if (horiztonal > 0 || horiztonal < 0)
        {
            lStickInUse = true;

        }

        if (!bButton)
        {
            anim.SetBool("isBPressed", false);
            col.height = 2.7f;
            col.center = new Vector3(col.center.x, 1.35f, col.center.z);
            //col2.height = 2.7f;
           // col2.center = new Vector3(col.center.x, 1.35f, col.center.z);
        }
        else if (lStickInUse && bButton)
        {
            anim.SetBool("isBPressed", true);
            col.height = 1.5f;
            col.center = new Vector3(col.center.x, 0.75f, col.center.z);
            //col2.height = 1.5f;
            //col2.center = new Vector3(col.center.x, 0.75f, col.center.z);
        }

        if (!rSticInUse)
        {
            anim.SetFloat("rHor", 0);
            anim.SetBool("isUsingRStick", false);
        }
        else if (rSticInUse)
        {
            anim.SetFloat("rHor", rHorizontal);
            anim.SetFloat("rHor", mouseX);
            anim.SetBool("isUsingRStick", true);
        }

        if (!lStickInUse)
        {

            anim.SetBool("isUsingLStick", false);
        }
        else if (lStickInUse)
        {

            anim.SetBool("isUsingLStick", true);
        }

        //anim.SetBool("isShiftPressed", lStick);

        Vector2 playerMov = new Vector2(horiztonal, vertical);
        Vector2 playerDir = playerMov.normalized;

        characterSpeed = ((vertical > 0 && (lStick || shift)) ? runSpeed : walkSpeed) * playerDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, characterSpeed, ref speedSVel, speedSTime);



        if (!col.isGrounded && velocityY < jumpForce)
        {

            anim.SetBool("isFalling", true);
            //anim.SetBool("isdoubleJumpTrue", false);
            //anim.SetBool("");

        }



        velocity = Vector3.up * velocityY;


        if (vertical > 0 && !col.isGrounded)
        {
            velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        }

        if (vertical < 0 && !col.isGrounded)
        {
            velocity = (-transform.forward * currentSpeed) + Vector3.up * velocityY;
        }

        col.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(col.velocity.x, col.velocity.z).magnitude;



        if (col.isGrounded)
        {
            velocityY = 0;
            anim.SetBool("isJumpPressed", false);
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
            anim.SetBool("isdoubleJumpTrue", false);
            isDoubleJumping = false;
            currentJumpCount = 0;
            currentTime = duration;
        }
    }

    /*private bool isGrounded()
    {
        
        return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.radius * 1.5f, groundLayers);
        //return false;

    }*/

    void rotateCharacter(float inputX)
    {
        Rigidbody charBody = this.GetComponent<Rigidbody>();

        //Vector2 input = new Vector2(inputX, 0.0f);
        //Vector2 inputDir = input.normalized;
        Quaternion deltaRot = Quaternion.Euler(0.0f, inputX * (turnST + 1.5f), 0.0f);
        //float targetRot = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
        //currentRot = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.x, targetRot, ref speedSVel, speedSTime);

        //Quaternion.ToEulerAngles.currentRot;
        charBody.MoveRotation(charBody.rotation * deltaRot);

    }

    void jump()
    {
        if (velocityY > -30)
        {
            currentJumpCount++;
        }

        
        if (currentJumpCount <= maxJumpCount)
        {
            anim.SetBool("isJumpPressed", true);
            anim.SetBool("isJumping", true);
            if (currentJumpCount < maxJumpCount)
            {
                float jumpVel = Mathf.Sqrt(-2 * gravity * jumpForce);
                velocityY = 0;
                velocityY += jumpVel;
            }
            else if (currentJumpCount == 2)
            {
                isDoubleJumping = true;
                float jumpVel = Mathf.Sqrt(-2 * gravity * (jumpForce + 1));
                velocityY = 0;
                velocityY += jumpVel;
            }
        }
    }

    void onAttack()
    {

        attacking = Input.GetKey("joystick button 2");

        anim.SetBool("isAttacking", attacking);

    }
}
