using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    public float offset = 1f;

    private float initialMouseXPos;
    private float initialBallXPos;
    private bool mouseDown;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            initialMouseXPos = Input.mousePosition.x;
            initialBallXPos = transform.position.x;
        }

        if (mouseDown && Input.GetMouseButtonUp(0))
            mouseDown = false;
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && mouseDown)
        {
            transform.position = new Vector3(initialBallXPos + (Input.mousePosition.x - initialMouseXPos) * offset, transform.position.y, transform.position.z);
        }
    }

}