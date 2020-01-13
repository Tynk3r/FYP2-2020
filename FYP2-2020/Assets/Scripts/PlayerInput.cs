using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Rigidbody rb;
    public float jumpForce = 2.5f;
    public float walkForce = 1f;
    private float distToGround = 0f;
    private bool jumping = false;

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(walkForce * Input.GetAxis("Horizontal"), rb.velocity.y, rb.velocity.z);
        
        if (Input.GetAxis("Jump1") > 0f && IsGrounded())
        {
            rb.velocity += Vector3.up * jumpForce;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
}
