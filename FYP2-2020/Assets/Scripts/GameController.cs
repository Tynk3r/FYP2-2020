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
    public GameObject platformPrefab;
    public List<GameObject> platforms = new List<GameObject>();
    public float platformDropSpeed = 1f;
    public float platformEmergeSpeed = 1f;
    public float gapBetweenPlatforms = 1f;
    public float platformEmergePosY = 5f;

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
        ball.SetActive(false);
        if (platforms.Count == 0)
            StartCoroutine(SpawnPlatform());
    }

    // Update is called once per frame
    void Update()
    {
        if (platforms.Count >= 5f)
            StartGame();
        if (shouldTimerRun)
            timer.UpdateTimerText();
    }

    private void FixedUpdate()
    {
        if (ball.transform.position.y <= -10f)
            EndGame();
        UpdatePlatforms();
    }

    private void UpdatePlatforms()
    {
        highestPlatformPosition = -100000f;
        foreach (GameObject platform in platforms)
        {
            if (platform.transform.position.z > 2.5f)
                platform.transform.Translate(0f, 0f, -platformEmergeSpeed);
            else
                platform.transform.Translate(0f, -platformDropSpeed, 0f);
            if (platform.transform.position.y < -10f)
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
            while (Mathf.Abs(platformPosX - platforms[platforms.Count - 1].transform.position.x) < 2.5f)
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
        platforms.Add(newPlatform);
        highestPlatformPosition = 5f;
        yield return 0;
    }

    public void RemovePlatform(GameObject platform)
    {
        platformsToRemove.Add(platform);
    }

    private void StartGame()
    {
        if (!ball.activeSelf)
            ball.SetActive(true);
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
