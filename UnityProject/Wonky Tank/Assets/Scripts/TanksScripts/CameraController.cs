using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    public KeyCode Left;
    public KeyCode Right;

    public float turnSpeed = 12.0f;

    private Vector3 offset;

    void Start() { offset = transform.position - player.transform.position; }

    void LateUpdate()
    {
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        transform.position = player.transform.position + offset;
        transform.LookAt(player.transform.position);
    }
}
