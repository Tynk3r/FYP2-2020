using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public enum GAME_STATE
    {
        PRE_START = 0,
        IN_PLAY,
    }

    public static GameController instance = null;

    public bool shouldPlatformsMove = true;
    public bool shouldGameStart = false;
    public bool shouldTimerRun = true;
    public Timer timer;
    public Camera mainCamera;
    public BallHandler ball;
    public GameObject platformPrefab;
    public List<Platform> platforms = new List<Platform>();
    public Vector3 ballStartingPosition = Vector3.zero;
    public float gapBetweenPlatforms;
    public float platformEmergePosY;
    public float platformRecedePosY;
    public float bottomOfScreen;

    [HideInInspector]
    public int blocksBroken = 0;

    private GAME_STATE gameState = GAME_STATE.PRE_START;
    private float highestPlatformPosition = -100000f;
    private List<Platform> platformsToRemove = new List<Platform>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        blocksBroken = 0;
        gameState = GAME_STATE.PRE_START;
        timer.gameObject.SetActive(false);
        ball.SetBallState(BallHandler.STATE.PRE_START);
        if (platforms.Count == 0)
            StartCoroutine(SpawnPlatform());
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GAME_STATE.PRE_START:
                if (platforms.Count >= 8f && ball.GetBallState() == BallHandler.STATE.PRE_START && shouldGameStart)
                    StartGame();
                break;

            case GAME_STATE.IN_PLAY:
                break;

            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (platforms.Count > 0 && shouldPlatformsMove)
            UpdatePlatforms();
        switch (gameState)
        {
            case GAME_STATE.PRE_START:
                break;

            case GAME_STATE.IN_PLAY:
                ball.UpdateMovement();
                if (ball.transform.position.y <= bottomOfScreen)
                    EndGame();
                timer.UpdateScore();
                break;

            default:
                break;
        }
    }

    private void UpdatePlatforms()
    {
        highestPlatformPosition = -100000f;
        foreach (Platform platform in platforms)
        {
            if (platform.toBeRemoved)
            {
                platformsToRemove.Add(platform);
                continue;
            }

            platform.UpdateMovement();
            if (platform.transform.position.y >= highestPlatformPosition)
                highestPlatformPosition = platform.transform.position.y;
            if (platform.transform.position.y <= platformRecedePosY)
                platform.Recede();
        }

        if (platformsToRemove.Count > 0f)
        {
            foreach (Platform platformToBeRemoved in platformsToRemove)
            {
                platforms.Remove(platformToBeRemoved);
                Destroy(platformToBeRemoved.gameObject);
            }
            platformsToRemove.Clear();
        }

        if (platforms.Count < 10f && highestPlatformPosition <= platformEmergePosY - gapBetweenPlatforms)
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

        GameObject newPlatform = Instantiate(platformPrefab, transform);
        newPlatform.transform.position = new Vector3(platformPosX, platformEmergePosY, 5.5f);
        newPlatform.transform.localScale = new Vector3(platformScaleX, 1f, 2.5f);
        if (newPlatform.GetComponent<Platform>() != null)
            platforms.Add(newPlatform.GetComponent<Platform>());
        if (newPlatform.GetComponent<BoxCollider>() != null)
            newPlatform.GetComponent<BoxCollider>().enabled = false;
        highestPlatformPosition = platformEmergePosY;
        yield return 0;
    }

    private void StartGame()
    {
        gameState = GAME_STATE.IN_PLAY;
        ball.SetBallState(BallHandler.STATE.EMERGING);
        ball.transform.position = ballStartingPosition;
        timer.gameObject.SetActive(true);
        shouldGameStart = false;
        MenuController.instance.ChangeMenuState(5);
    }

    private void EndGame()
    {
        gameState = GAME_STATE.PRE_START;
        ball.SetBallState(BallHandler.STATE.PRE_START);
        blocksBroken = 0;
        timer.ResetScore();
        timer.gameObject.SetActive(false);
        MenuController.instance.ChangeMenuState(4);
    }
}
