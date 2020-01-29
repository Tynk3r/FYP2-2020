using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BallHandler : MonoBehaviour
{
    public enum STATE
    {
        PRE_START,
        EMERGING,
        IN_PLAY
    };

    public float xSpeed = 1f;
    public float maxYSpeed = 5f;
    public float ballEmergeSpeed = 1f;

    private Rigidbody rb;
    private float ballOffset;
    private Vector3 intendedBallPosition;
    private STATE ballState = STATE.PRE_START;

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null && GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }
    public void UpdateMovement()
    {
        if (rb.velocity.y >= maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);

        switch (ballState)
        {
            case STATE.PRE_START:
                break;

            case STATE.EMERGING:
                if (transform.position.z > 2.5f)
                    transform.Translate(-Vector3.forward * ballEmergeSpeed);
                else
                    SetBallState(STATE.IN_PLAY);
                break;

            case STATE.IN_PLAY:
                /*if (Input.GetMouseButton(0))
                {
                    intendedBallPosition = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(-Camera.main.transform.position.z + transform.position.z);
                    ballOffset = intendedBallPosition.x - transform.position.x;
                    if (ballOffset > 0.1f)
                        rb.velocity = new Vector3(xSpeed, rb.velocity.y, rb.velocity.z);
                    else if (ballOffset < -0.1f)
                        rb.velocity = new Vector3(-xSpeed, rb.velocity.y, rb.velocity.z);
                    else
                        rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
                }
                else
                {
                    rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
                }*/
                if (Input.GetAxis("Horizontal") > 0f)
                    rb.velocity = new Vector3(xSpeed, rb.velocity.y, rb.velocity.z);
                else if (Input.GetAxis("Horizontal") < 0f)
                    rb.velocity = new Vector3(-xSpeed, rb.velocity.y, rb.velocity.z);
                else
                    rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
                break;

            default:
                break;
        }
    }
    public void SetBallState(STATE newState)
    {
        switch (newState)
        {
            case STATE.PRE_START:
                if (GetComponent<Renderer>().enabled)
                    GetComponent<Renderer>().enabled = false;
                if (GetComponent<Rigidbody>() != null && !GetComponent<Rigidbody>().isKinematic)
                    GetComponent<Rigidbody>().isKinematic = true;
                if (GetComponent<Rigidbody>() != null && GetComponent<Rigidbody>().useGravity)
                    GetComponent<Rigidbody>().useGravity = false;
                if (GetComponent<TrailRenderer>() != null && GetComponent<TrailRenderer>().enabled)
                    GetComponent<TrailRenderer>().enabled = false;
                break;

            case STATE.EMERGING:
                if (!GetComponent<Renderer>().enabled)
                    GetComponent<Renderer>().enabled = true;
                if (GetComponent<Rigidbody>() != null && GetComponent<Rigidbody>().isKinematic)
                    GetComponent<Rigidbody>().isKinematic = false;
                if (GetComponent<Rigidbody>() != null && GetComponent<Rigidbody>().useGravity)
                    GetComponent<Rigidbody>().useGravity = false;
                if (GetComponent<TrailRenderer>() != null && GetComponent<TrailRenderer>().enabled)
                    GetComponent<TrailRenderer>().enabled = false;
                break;

            case STATE.IN_PLAY:
                if (!GetComponent<Renderer>().enabled)
                    GetComponent<Renderer>().enabled = true;
                if (GetComponent<Rigidbody>() != null && GetComponent<Rigidbody>().isKinematic)
                    GetComponent<Rigidbody>().isKinematic = false;
                if (GetComponent<Rigidbody>() != null && !GetComponent<Rigidbody>().useGravity)
                    GetComponent<Rigidbody>().useGravity = true;
                if (GetComponent<TrailRenderer>() != null && !GetComponent<TrailRenderer>().enabled)
                    GetComponent<TrailRenderer>().enabled = true;
                break;
            default:
                break;
        }
        ballState = newState;
    }
    public STATE GetBallState()
    {
        return ballState;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.collider.gameObject;
        if (go.GetComponent<Platform>() != null)
            if (GetComponent<SphereCollider>().bounds.min.y < go.transform.position.y)
                go.GetComponent<Platform>().ShatterPlatform();
    }

}