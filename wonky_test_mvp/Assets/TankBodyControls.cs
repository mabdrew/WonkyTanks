using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TankBodyControls : MonoBehaviour {

    public KeyCode Foward = KeyCode.W;
    public KeyCode Backward = KeyCode.S;
    public KeyCode TurnLeft = KeyCode.A;
    public KeyCode TurnRight = KeyCode.D;

    public const float MaxSpeed = 3.0f;
    public const float RotSpeed = 1.0f;

    private Rigidbody2D TankBody;    

	// Use this for initialization
	void Start () {
        TankBody = GetComponent<Rigidbody2D>();
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        //print("called");
        //if(coll.collider.CompareTag("wall"))
        //{
        //    print("collided!");
        //    var P = transform.position;
        //    P.x -= TankBody.velocity.x;
        //    P.y -= TankBody.velocity.y;

        //    transform.Translate(P);
        //}
    }
	
    private void MoveTank()
    {
        var Vel = TankBody.velocity;

        var rot = TankBody.rotation;

        if (Input.GetKey(TurnRight))
        {
            rot -= RotSpeed;
        }

        if (Input.GetKey(TurnLeft))
        {
            rot += RotSpeed;
        }

        if(Input.GetKey(Foward)||Input.GetKey(Backward))
        {
            float VelMag = (Input.GetKey(Foward) ? MaxSpeed : -1.0f * MaxSpeed/2.0f);
            float VelAngle = transform.eulerAngles.z * ((float)Math.PI / 180f);
            Vel = new Vector2((float)Math.Cos(VelAngle), (float)Math.Sin(VelAngle));
            Vel *= VelMag;



            
        }
        else
        {            
            Vel = Vector2.zero;
        }

        TankBody.rotation = rot;
        TankBody.velocity = Vel;
    }

	// Update is called once per frame
	void Update () {
        MoveTank();
	}
}
