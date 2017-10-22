//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CameraOwnerControl : MonoBehaviour {


//    public GameObject Player;
//    public float RotateSpeed;
//    public KeyCode Left;
//    public KeyCode Right;

//    void Turn() //rotate Left or Right
//    {
//        if (Input.GetKey(Left))
//        {
//            transform.Rotate(0.0f, RotateSpeed, 0.0f);
//        }
//        if (Input.GetKey(Right))
//        {
//            transform.Rotate(0.0f, -1.0f * RotateSpeed, 0.0f);
//        }
//    }

//    // Use this for initialization
//    void Start () {
//        Left = KeyCode.G;
//        Right = KeyCode.H;
//    }
	
//    // Update is called once per frame
//    void Update () {
//        Turn();
//    }
//}
