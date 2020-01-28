using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public enum STATE
    {
        EMERGING = 0,
        DROPPING,
        RECEDING
    };

    public GameObject platformBreakPrefab;
    public float emergeSpeed = 0f;
    public float emergeEndZ = 0f;
    public float dropSpeed = 0f;
    public float recedeSpeed = 0f;
    public float recedeEndZ = 0f;

    [HideInInspector]
    public bool toBeRemoved = false;

    private STATE platformState = STATE.EMERGING;

    // Start is called before the first frame update
    void Start()
    {
        if (platformState == STATE.EMERGING && recedeEndZ != transform.position.z)
            recedeEndZ = transform.position.z;
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
        switch (platformState)
        {
            case STATE.EMERGING:
                if (GetComponent<BoxCollider>() != null && GetComponent<BoxCollider>().enabled)
                    GetComponent<BoxCollider>().enabled = false;
                transform.Translate(0f, 0f, -emergeSpeed);
                if (transform.position.z <= emergeEndZ)
                    platformState = STATE.DROPPING;
                break;

            case STATE.DROPPING:
                if (GetComponent<BoxCollider>() != null && !GetComponent<BoxCollider>().enabled)
                    GetComponent<BoxCollider>().enabled = true;
                transform.Translate(0f, -dropSpeed, 0f);
                break;

            case STATE.RECEDING:
                if (GetComponent<BoxCollider>() != null && GetComponent<BoxCollider>().enabled)
                    GetComponent<BoxCollider>().enabled = false;
                transform.Translate(0f, 0f, recedeSpeed);
                if (transform.position.z >= recedeEndZ)
                    toBeRemoved = true;
                break;

            default:
                break;
        }

    }
    public void ShatterPlatform()
    {
        GameController.instance.blocksBroken++;

        GameObject destroyEffect = Instantiate(platformBreakPrefab);
        destroyEffect.transform.position = transform.position;

        toBeRemoved = true;
    }

    public bool IsEmerging()
    {
        return platformState == STATE.EMERGING;
    }

    public void Recede()
    {
        platformState = STATE.RECEDING;
    }

}
