using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public enum GAME_STATE
    {
        PRE_START = 0,
        IN_PLAY,
        ENDED
    }

    public static GameController instance = null;

    public bool shouldTimerRun = true;
    public Timer timer;
    public Camera mainCamera;
    public BallHandler ball;
    public GameObject platformPrefab;
    public List<Platform> platforms = new List<Platform>();
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
        ball.SetBallState(BallHandler.STATE.PRE_START);
        if (platforms.Count == 0)
            StartCoroutine(SpawnPlatform());
    }

    // Update is called once per frame
    void Update()
    {
        if (platforms.Count >= 5f && ball.GetBallState() == BallHandler.STATE.PRE_START)
            StartGame();
    }

    private void FixedUpdate()
    {
        switch (gameState)
        {
            case GAME_STATE.PRE_START:
                if (platforms.Count > 0)
                    UpdatePlatforms();
                break;

            case GAME_STATE.IN_PLAY:
                ball.UpdateMovement();
                if (platforms.Count > 0)
                    UpdatePlatforms();
                if (ball.transform.position.y <= bottomOfScreen)
                    EndGame();
                timer.UpdateScore();
                break;

            case GAME_STATE.ENDED:
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

        GameObject newPlatform = Instantiate(platformPrefab);
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
    }

    private void EndGame()
    {
#if UNITY_EDITOR
        //UnityEditor.EditorApplication.isPlaying = false;
#else
         //Application.Quit();
#endif
        gameState = GAME_STATE.ENDED;
    }
}
