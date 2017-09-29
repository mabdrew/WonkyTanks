using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunControl : MonoBehaviour {

    public KeyCode ClockWise = KeyCode.DownArrow;
    public KeyCode CounterClockWise = KeyCode.UpArrow;

    public KeyCode fire = KeyCode.Space;

    private float RotSpeed = 5.0f;

    private void RotateGun()
    {
        if (Input.GetKey(ClockWise))
        {
            Vector3 RotVec = new Vector3(0.0f, 0.0f, 0.0f);
            RotVec.z = RotSpeed;
            transform.Rotate(RotVec);
        }
        if (Input.GetKey(CounterClockWise))
        {
            Vector3 RotVec = new Vector3(0.0f, 0.0f, 0.0f);
            RotVec.z = -1.0f*RotSpeed;
            transform.Rotate(RotVec);
        }
    }

    public Rigidbody2D fireableball;
    private DateTime timeFired;

    void FireBall()
    {
        if (DateTime.Now > timeFired.AddSeconds(1))
        {
            timeFired = DateTime.Now;
            float rotrad = transform.eulerAngles.z * ((float)Math.PI / 180f);
            var vel = new Vector2((float)Math.Cos(rotrad), (float)Math.Sin(rotrad))/1.1f;

            var balltrans = transform.position;
            balltrans.x += vel.x;
            balltrans.y += vel.y;

            Rigidbody2D ballclone = (Rigidbody2D)Instantiate(fireableball, balltrans, transform.rotation);
            vel *= 4.0f;
            ballclone.GetComponent<BallControl>().Vel = vel;
            ballclone.velocity = vel;


            ballclone.GetComponent<BallControl>().IsTemp = true;
            ballclone.GetComponent<BallControl>().MaxNoCollisions = 2;
            ballclone.GetComponent<BallControl>().NoCollisions = 0;
            ballclone.GetComponent<BallControl>().Started = true;
        }
    }

	// Use this for initialization
	void Start () {
        timeFired = DateTime.Now;
	}
	
	// Update is called once per frame
	void Update () {
        RotateGun();

        if(Input.GetKey(fire))
            FireBall();
	}
}
