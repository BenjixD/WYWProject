using UnityEngine;
using System.Collections;

public class TurretBehaviour : MonoBehaviour {

    public bool facingRight = true;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody2D guy = GameObject.Find("guy").GetComponent<Rigidbody2D>();
        Transform self = GetComponent<Transform>();
        //Guy left of bot
        if ((guy.position.x < self.position.x) && facingRight) Flip();
        //guy right of bot
        else if ((guy.position.x > self.position.x) && !facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        //Collider2D hitbox = GetComponent<CircleCollider2D>();       
        //hitbox.offset = new Vector2(hitbox.offset.x * -1, hitbox.offset.y);
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
