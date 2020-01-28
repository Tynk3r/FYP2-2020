using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    public bool shouldTimerRun = true;
    public Timer timer;
    public Camera mainCamera;
    public GameObject ball;
    public float ballEmergeSpeed = 1f;
    public GameObject platformPrefab;
    public GameObject platformBreakPrefab;
    public List<GameObject> platforms = new List<GameObject>();
    public float platformDropSpeed = 1f;
    public float platformEmergeSpeed = 1f;
    public float gapBetweenPlatforms = 1f;
    public float platformEmergePosY = 5f;
    public float topOfScreen = 12.5f;
    public float bottomOfScreen = -12.5f;

    [HideInInspector]
    public int blocksBroken = 0;

    private float highestPlatformPosition = -100000f;
    private List<GameObject> platformsToRemove = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        blocksBroken = 0;
        if (ball.GetComponent<Rigidbody>() != null && ball.GetComponent<Rigidbody>().useGravity)
            ball.GetComponent<Rigidbody>().useGravity = false;
        if (platforms.Count == 0)
            StartCoroutine(SpawnPlatform());
    }

    // Update is called once per frame
    void Update()
    {
        if (platforms.Count >= 5f)
            StartGame();
        if (shouldTimerRun)
            timer.UpdateScore();
    }

    private void FixedUpdate()
    {
        if (ball.transform.position.y <= bottomOfScreen)
            EndGame();
        UpdatePlatforms();
    }

    private void UpdatePlatforms()
    {
        highestPlatformPosition = -100000f;
        foreach (GameObject platform in platforms)
        {
            if (platform.transform.position.z > 2.5f)
            {
                if (platform.GetComponent<BoxCollider>() != null && platform.GetComponent<BoxCollider>().enabled)
                    platform.GetComponent<BoxCollider>().enabled = false;
                platform.transform.Translate(0f, 0f, -platformEmergeSpeed);
            }
            else
            {
                if (platform.GetComponent<BoxCollider>() != null && !platform.GetComponent<BoxCollider>().enabled)
                    platform.GetComponent<BoxCollider>().enabled = true;
                platform.transform.Translate(0f, -platformDropSpeed, 0f);

            }
            if (platform.transform.position.y < bottomOfScreen)
            {
                platformsToRemove.Add(platform);
                continue;
            }
            if (platform.transform.position.y >= highestPlatformPosition)
                highestPlatformPosition = platform.transform.position.y;
        }

        if (platformsToRemove.Count > 0f)
        {
            foreach (GameObject platformToBeRemoved in platformsToRemove)
            {
                platforms.Remove(platformToBeRemoved);
                Destroy(platformToBeRemoved);
            }
            platformsToRemove.Clear();
        }

        if (highestPlatformPosition <= platformEmergePosY - gapBetweenPlatforms && platforms.Count < 10f)
        {
            StartCoroutine(SpawnPlatform());
        }

    }

    IEnumerator SpawnPlatform()
    {
        float platformScaleX = Random.Range(2.5f, 5f);
        float platformEdge = Random.Range(-1f, 1f);
        float platformPosX;
        if (platformEdge < 0f)
            platformPosX = platformEdge + platformScaleX;
        else if (platformEdge > 0f)
            platformPosX = platformEdge - platformScaleX;
        else
            platformPosX = platformEdge;

        if (platforms.Count > 0f)
        {
            while (Mathf.Abs(platformPosX - platforms[platforms.Count - 1].transform.position.x) < 1f)
            {
                platformScaleX = Random.Range(2.5f, 5f);
                platformEdge = Random.Range(-1f, 1f);
                if (platformEdge < 0f)
                    platformPosX = platformEdge + platformScaleX;
                else if (platformEdge > 0f)
                    platformPosX = platformEdge - platformScaleX;
                else
                    platformPosX = platformEdge;
            }
        }

        GameObject newPlatform = Instantiate(platformPrefab);
        newPlatform.transform.position = new Vector3(platformPosX, platformEmergePosY, 5.5f);
        newPlatform.transform.localScale = new Vector3(platformScaleX, 1f, 2.5f);
        if (newPlatform.GetComponent<BoxCollider>() != null)
            newPlatform.GetComponent<BoxCollider>().enabled = false;
        platforms.Add(newPlatform);
        highestPlatformPosition = 5f;
        yield return 0;
    }

    public void ShatterPlatform(GameObject platform)
    {
        blocksBroken++;
        GameObject destroyEffect = Instantiate(platformBreakPrefab);
        destroyEffect.transform.position = platform.transform.position;
        platformsToRemove.Add(platform);
    }

    private void StartGame()
    {
        if (ball.transform.position.z > 2.5f)
        {
            if (ball.GetComponent<Rigidbody>() != null && ball.GetComponent<Rigidbody>().useGravity)
                ball.GetComponent<Rigidbody>().useGravity = false;
            ball.transform.Translate(-Vector3.forward * ballEmergeSpeed);
        }
        else
        {
            if (ball.GetComponent<Rigidbody>() != null && !ball.GetComponent<Rigidbody>().useGravity)
                ball.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
