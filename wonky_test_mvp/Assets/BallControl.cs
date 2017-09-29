using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{

    private Rigidbody2D rb2d;
    private Vector2 vel;

    private uint noCollisions;
    private uint maxNoCollisions = 8;
    private bool isTemp;

    private bool started = false;//there's got to be a better way to do this

    void GoBall()
    {
        float rand = Random.Range(0, 2);
        if (rand < 1)
            rb2d.AddForce(new Vector2(20, -15));
        else
            rb2d.AddForce(new Vector2(-20, -15));
    }

    void ResetBall()
    {
        vel = Vector2.zero;
        rb2d.velocity = vel;
        transform.position = Vector2.zero;
    }

    void RestartGame()
    {
        ResetBall();
        Invoke("GoBall", 1);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("Player"))
        {
            vel.x = rb2d.velocity.x;
            vel.y = (rb2d.velocity.y / 2.0f) + (coll.collider.attachedRigidbody.velocity.y / 3.0f);
            rb2d.velocity = vel;
        }
        noCollisions++;
        if ((noCollisions > maxNoCollisions) && isTemp)
            Destroy(gameObject);
    }

    public uint NoCollisions
    {
        get { return noCollisions; }
        set { noCollisions = value; }
    }

    public bool IsTemp
    {
        get { return isTemp; }
        set { isTemp = value; }
    }

    public Vector2 Vel
    {
        get { return vel; }
        set { vel = value; }
    }

    public uint MaxNoCollisions
    {
        get { return maxNoCollisions; }
        set { maxNoCollisions = value; }
    }

    public bool Started
    {
        get { return started; }
        set { started = value; }
    }
    public Rigidbody2D Rb2d
    {
        get { return rb2d; }
        set { Rb2d = value; }
    }


    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (!started)
        {
            //print("called start");
            isTemp = false;
            maxNoCollisions = 0;
            noCollisions = 0;
            Invoke("GoBall", 3);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
