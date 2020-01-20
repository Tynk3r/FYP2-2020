using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    public float xSpeed = 1f;
    public float mouseToBallOffsetMult = 0.01f;
    public float maxYSpeed = 5f;

    private Rigidbody rb;
    private float ballOffset;
    private bool mouseDown;
    private Vector3 intendedBallPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (rb.velocity.y >= maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);

        if (Input.GetMouseButton(0))
        {
            intendedBallPosition = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(-Camera.main.transform.position.z);
            ballOffset = intendedBallPosition.x - transform.position.x;
            if (ballOffset > 1f)
            {
                rb.velocity = new Vector3(xSpeed, rb.velocity.y, rb.velocity.z);
                Debug.Log("moving right!");
            }
            else if (ballOffset < 1f)
            {
                rb.velocity = new Vector3(-xSpeed, rb.velocity.y, rb.velocity.z);
                Debug.Log("moving left!");
            }
            else
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
                Debug.Log("not moving!");
            }
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        }

    }

}