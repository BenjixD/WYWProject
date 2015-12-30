using UnityEngine;
using System.Collections;

public class movement : MonoBehaviour
{

    public float maxSpeed = 8f;
    public float AdjustSpeed;
    public float dashModifier = 1.5f;

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

        isDashing();

        float move = Input.GetAxis("Horizontal");

        ////////////////////////////Lock Movement
        if (animationLock())
        {
            return;
        } 
        ////////////////////////////

        a.SetFloat("Speed", Mathf.Abs(move));

        SpriteRenderer[] s = GetComponentsInChildren<SpriteRenderer>();
        if (a.GetBool("Dash"))
        {
            
            float shadowLag = -0.15f;
            float dir = shadowLag;
            if (!facingRight) dir *= -1;
            foreach (SpriteRenderer i in s)
            {
                if (i.name == "Shadow1")
                {
                    i.transform.localPosition = new Vector3(dir * (rb.velocity.x / AdjustSpeed), shadowLag * (rb.velocity.y/20));
                }
                else if (i.name == "Shadow2")
                {
                    i.transform.localPosition = new Vector3(2 * dir * (rb.velocity.x / AdjustSpeed), 2 * shadowLag * (rb.velocity.y/20));
                }
            }
        }
        else
        {
            foreach (SpriteRenderer i in s)
            {
                if (i.name == "Shadow1")
                {
                    i.transform.localPosition = new Vector3(0, 0);
                }
                else if (i.name == "Shadow2")
                {
                    i.transform.localPosition = new Vector3(0, 0);
                }
            }
        }
        
        rb.velocity = new Vector2(move * AdjustSpeed, rb.velocity.y);

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
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

            if (!doubleJump && !grounded)
            {
                doubleJump = true;
            }
        }

        //Dashing
        if(grounded && Input.GetKeyDown(KeyCode.X))
        {
            a.SetBool("Dash", true);
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

    void isDashing()
    {
        if (a.GetBool("Dash"))
        {
            AdjustSpeed = maxSpeed * dashModifier;
        }

        else
        {
            AdjustSpeed = maxSpeed;
        }
    }
}
