using UnityEngine;
using System.Collections;

public class movement : MonoBehaviour
{

    public float maxSpeed = 8f;

    bool grounded = false;
    bool doubleJump = false;
    public Transform groundCheck;
    float groundRadius = 0.12f;
    public LayerMask whatIsGround;
    public float jumpForce = 950f;
    public float jumpSpeed = 20f;



    bool facingRight = true;

    Animator a;

    // Use this for initialization
    void Start()
    {
        a = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        a.SetBool("Ground", grounded);
        a.SetFloat("vSpeed", rb.velocity.y);

        if (grounded)
            doubleJump = false;

        float move = Input.GetAxis("Horizontal");

        ////////////////////////////Lock Movement
        if (animationLock())
        {
            return;
        } 
        ////////////////////////////

        a.SetFloat("Speed", Mathf.Abs(move));
        
        rb.velocity = new Vector2(move * maxSpeed, rb.velocity.y);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();
    }

    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

         //Jumping
        if ((grounded || !doubleJump) && Input.GetKeyDown(KeyCode.Space) && !animationLock())
        {
            a.SetBool("Ground", false);
            //rb.AddForce(new Vector2(0, jumpForce));
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

            if (!doubleJump && !grounded)
            {
                doubleJump = true;
            }
        }

        //attacks
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            a.SetBool("attack", true);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    bool animationLock()
    {
        if(a.GetCurrentAnimatorStateInfo(0).IsName("neutral1") ||
           a.GetCurrentAnimatorStateInfo(0).IsName("neutral2") ||
           a.GetCurrentAnimatorStateInfo(0).IsName("neutral3") ||
           a.GetCurrentAnimatorStateInfo(0).IsName("sheathe"))
        {
            return true;
        }

        return false;
    }
}
