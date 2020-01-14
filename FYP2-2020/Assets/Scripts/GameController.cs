using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public bool shouldTimerRun = true;
    public Timer timer;
    public Camera mainCamera;
    public GameObject platformPrefab;
    public List<GameObject> platforms;
    public float platformDropSpeed = 1f;

    private float highestPlatformPosition = -100000f;

    // Start is called before the first frame update
    void Start()
    {
        if (platforms.Count == 0)
        {
            StartCoroutine(SpawnPlatform());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldTimerRun)
            timer.UpdateTimerText();
    }

    private void FixedUpdate()
    {
        UpdatePlatforms();
    }

    private void UpdatePlatforms()
    {
        highestPlatformPosition = -100000f;

        foreach (GameObject platform in platforms)
        {
            platform.transform.Translate(0f, -platformDropSpeed, 0f);
            if (platform.transform.position.y < -10f)
            {
                Debug.Log("Destroying platform.");
                platforms.Remove(platform);
                Destroy(platform);
                continue;
            }
            if (platform.transform.position.y >= highestPlatformPosition)
            {
                highestPlatformPosition = platform.transform.position.y;
                Debug.Log("Highest platform now at " + highestPlatformPosition);
            }
        }

        if (highestPlatformPosition <= 4f && platforms.Count < 10f)
        {
            StartCoroutine(SpawnPlatform());
        }

    }

    IEnumerator SpawnPlatform()
    {
        Debug.Log("Spawning platform.");

        float platformScaleX = Random.Range(2.5f, 5f);
        float platformEdge = Random.Range(-1f, 1f);
        float platformPosX = 0f;
        if (platformEdge < 0f)
            platformPosX = platformEdge + platformScaleX;
        else if (platformEdge > 0f)
            platformPosX = platformEdge - platformScaleX;
        else
            platformPosX = platformEdge;

        GameObject newPlatform = Instantiate(platformPrefab);
        newPlatform.transform.position = new Vector3(platformPosX, 7.5f, 28.75f);
        newPlatform.transform.localScale = new Vector3(platformScaleX, 1f, 2.5f);
        platforms.Add(newPlatform);
        highestPlatformPosition = 5f;
        yield return 0;
    }
}
