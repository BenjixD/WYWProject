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
    //float groundRadius = 0.12f;
    public LayerMask whatIsGround;
    public float jumpForce = 950f;
    public float jumpSpeed = 20f;
    Vector2[] prevFrames = new Vector2[5];



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
        
        grounded = Physics2D.OverlapArea(new Vector3(groundCheck.position.x - 0.25f, groundCheck.position.y - 0.2f), 
                                         new Vector3(groundCheck.position.x + 0.25f, groundCheck.position.y + 0.2f), 
                                         whatIsGround);
        a.SetBool("Ground", grounded);
        a.SetFloat("vSpeed", rb.velocity.y);

        if (grounded)
            doubleJump = false;

        isDashing();

        //Horizontal Directional Input
        float move = Input.GetAxis("Horizontal");

        ////////////////////////////Lock Movement
        if (animationLock())
        {
            move = 0.0f;
            a.SetFloat("Speed", Mathf.Abs(move));
            hillAdjuster(rb);
            return;
        }
        ////////////////////////////
             
        a.SetFloat("Speed", Mathf.Abs(move));
        shadowUpdate(rb, a.GetBool("Dash"));
        transformShadows();

        hillAdjuster(rb);
        hitCheck();

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
            // Jump-Dash
            if (Input.GetKeyDown(KeyCode.X) && grounded)
                a.SetBool("Dash", true);
            //Jump Attack
            if (Input.GetKeyDown(KeyCode.Z))
                a.SetBool("attack", true);
            if (!doubleJump && !grounded)
            {
                doubleJump = true;
            }
        }

        //Dashing
        else if(grounded && !animationLock() && Input.GetKeyDown(KeyCode.X))
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
           a.GetCurrentAnimatorStateInfo(0).IsName("sheathe") ||
           a.GetCurrentAnimatorStateInfo(0).IsName("landState"))
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

    void shadowUpdate(Rigidbody2D rb, bool Dash)
    {
        //Fill Frames with current location if not dashing
        if (!Dash)
        {
            for (int i = 0; i < prevFrames.Length; i++)
            {
                prevFrames[i] = rb.position;
            }
            return;
        }

        //Update Previous Frames list
        for (int i = prevFrames.Length - 1; i > 0; i--)
        {
             prevFrames[i] = prevFrames[i - 1];
        }
        prevFrames[0] = rb.position;
    }

    void transformShadows()
    {
        //Update Shadow Positions
        SpriteRenderer[] s = GetComponentsInChildren<SpriteRenderer>();
        if (a.GetBool("Dash"))
        {
            foreach (SpriteRenderer i in s)
            {
                if (i.name == "Shadow1")
                {
                    i.transform.position = prevFrames[2];
                }
                else if (i.name == "Shadow2")
                {
                    i.transform.position = prevFrames[4];
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
    }


    void hitCheck()
    {
        Collider2D hitbox = GetComponent<Collider2D>();
        Collider2D turretShot = GameObject.Find("TurretDummy").GetComponent<CircleCollider2D>();
        if (hitbox.IsTouching(turretShot)) a.SetBool("hit", true);
        else a.SetBool("hit", false);
    }

    void hillAdjuster(Rigidbody2D rb)
    {
        ////////////////////////////hill adjusters
        if (Mathf.Abs(a.GetFloat("Speed")) < 0.1f && grounded)
        {
            rb.gravityScale = 0;
        }

        else
        {
            rb.gravityScale = 2;
        }
        ////////////////////////////
    }
}
