using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public enum MENU_STATE
    {
        SPLASH = 0,
        MAIN,
        STATS,
        OPTIONS,
        END_OF_GAME,
        IN_PLAY,
    }
    public static MenuController instance = null;

    [Header("Menu Panels")]
    public GameObject splashLogo;
    public GameObject mainMenuContainer;
    public GameObject statMenuContainer;
    public GameObject optionsMenuContainer;
    public GameObject endOfGameContainer;

    [Header("Splash Animation")]
    public float logoFadeInSpeed = .01f;
    public float logoFirstPauseLength = 1f;
    public float logoMoveSpeed = 5f;
    public float logoSecondPauseLength = 1f;
    public float panelFadeInSpeed = 0.005f;
    public float buttonFadeInSpeed = .01f;
    public Image mainMenuLogo;
    public Image mainMenuPanel;
    public TextMeshProUGUI playButtonText;
    public TextMeshProUGUI statsButtonText;
    public TextMeshProUGUI optionsButtonText;
    public TextMeshProUGUI quitButtonText;
    public TextMeshProUGUI versionText;

    [Header("End Of Game Screen")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI endMessage;

    [Header("Stats Screen")]
    public TextMeshProUGUI highscoreStatsText;
    public TextMeshProUGUI blocksBrokenStatsText;

    [Header("Options Screen")]
    public TextMeshProUGUI controlTypeChoice;

    private MENU_STATE currentState;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        if (!SceneManager.GetSceneByName("GameScene").isLoaded)
            SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }
    private void Start()
    {
        ChangeMenuState(0);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));

        // Splash Animation
        StartCoroutine("SplashAnimation");
    }
    private IEnumerator SplashAnimation()
    {
        GameController.instance.shouldPlatformsMove = false;

        // Fade logo in
        while (splashLogo.GetComponent<Image>().color.a < 1f)
        {
            splashLogo.GetComponent<Image>().color = new Color(splashLogo.GetComponent<Image>().color.r, splashLogo.GetComponent<Image>().color.g, splashLogo.GetComponent<Image>().color.b, splashLogo.GetComponent<Image>().color.a + logoFadeInSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(logoFirstPauseLength); // pause for a second

        GameController.instance.shouldPlatformsMove = true; // start moving platforms in background
        mainMenuPanel.gameObject.SetActive(true); // enable main menu UI but set to transparent
        mainMenuPanel.color = new Color(mainMenuPanel.color.r, mainMenuPanel.color.g, mainMenuPanel.color.b, 0f);
        mainMenuLogo.color = new Color(mainMenuLogo.color.r, mainMenuLogo.color.g, mainMenuLogo.color.b, 0f);
        playButtonText.color = new Color(playButtonText.color.r, playButtonText.color.g, playButtonText.color.b, 0f);
        statsButtonText.color = new Color(statsButtonText.color.r, statsButtonText.color.g, statsButtonText.color.b, 0f);
        optionsButtonText.color = new Color(optionsButtonText.color.r, optionsButtonText.color.g, optionsButtonText.color.b, 0f);
        quitButtonText.color = new Color(quitButtonText.color.r, quitButtonText.color.g, quitButtonText.color.b, 0f);
        versionText.color = new Color(versionText.color.r, versionText.color.g, versionText.color.b, 0f);
        while (splashLogo.GetComponent<RectTransform>().anchoredPosition != mainMenuLogo.GetComponent<RectTransform>().anchoredPosition)
        {
            // start moving logo up into main menu logo position
            splashLogo.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(splashLogo.GetComponent<RectTransform>().anchoredPosition, mainMenuLogo.GetComponent<RectTransform>().anchoredPosition, logoMoveSpeed * Time.deltaTime);
            // fade in main menu background
            if (mainMenuPanel.color.a < 0.4f)
                mainMenuPanel.color = new Color(mainMenuPanel.color.r, mainMenuPanel.color.g, mainMenuPanel.color.b, mainMenuPanel.color.a + panelFadeInSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(logoSecondPauseLength); // pause for a second
        // start fading in buttons (quit -> options -> stats -> wait for blocks then play + version)
        while (quitButtonText.color.a < 1f)
        {
            quitButtonText.color = new Color(quitButtonText.color.r, quitButtonText.color.g, quitButtonText.color.b, quitButtonText.color.a + buttonFadeInSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        while (optionsButtonText.color.a < 1f)
        {
            optionsButtonText.color = new Color(optionsButtonText.color.r, optionsButtonText.color.g, optionsButtonText.color.b, optionsButtonText.color.a + buttonFadeInSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        while (statsButtonText.color.a < 1f)
        {
            statsButtonText.color = new Color(statsButtonText.color.r, statsButtonText.color.g, statsButtonText.color.b, statsButtonText.color.a + buttonFadeInSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        while (GameController.instance.platforms.Count < 5f)
            yield return new WaitForEndOfFrame();
        while (playButtonText.color.a < 1f)
        {
            playButtonText.color = new Color(playButtonText.color.r, playButtonText.color.g, playButtonText.color.b, playButtonText.color.a + buttonFadeInSpeed * Time.deltaTime);
            versionText.color = new Color(versionText.color.r, versionText.color.g, versionText.color.b, playButtonText.color.a); // PURPOSELY SET TO PLAYBUTTONTEXT ALPHA TO MATCH
            yield return new WaitForEndOfFrame();
        }
        if (currentState == MENU_STATE.SPLASH)
            ChangeMenuState(1);
    }
    public void ChangeMenuState(int newStateID)
    {
        PlayerPrefs.SetString("Control Type", controlTypeChoice.text);

        // in case change to options or stats while loading
        mainMenuPanel.color = new Color(mainMenuPanel.color.r, mainMenuPanel.color.g, mainMenuPanel.color.b, 0.4f);
        mainMenuLogo.color = new Color(mainMenuLogo.color.r, mainMenuLogo.color.g, mainMenuLogo.color.b, 1f);
        playButtonText.color = new Color(playButtonText.color.r, playButtonText.color.g, playButtonText.color.b, 1f);
        statsButtonText.color = new Color(statsButtonText.color.r, statsButtonText.color.g, statsButtonText.color.b, 1f);
        optionsButtonText.color = new Color(optionsButtonText.color.r, optionsButtonText.color.g, optionsButtonText.color.b, 1f);
        quitButtonText.color = new Color(quitButtonText.color.r, quitButtonText.color.g, quitButtonText.color.b, 1f);
        versionText.color = new Color(versionText.color.r, versionText.color.g, versionText.color.b, 1f);

        splashLogo.SetActive(false);
        mainMenuContainer.SetActive(false);
        statMenuContainer.SetActive(false);
        optionsMenuContainer.SetActive(false);
        endOfGameContainer.SetActive(false);
        MENU_STATE newState = (MENU_STATE)newStateID;
        switch (newState)
        {
            case MENU_STATE.SPLASH:
                splashLogo.SetActive(true);
                break;

            case MENU_STATE.MAIN:
                mainMenuContainer.SetActive(true);
                break;

            case MENU_STATE.STATS:
                highscoreStatsText.SetText("Highscore: " + PlayerPrefs.GetInt("Highscore", 0).ToString());
                blocksBrokenStatsText.SetText("Most Blocks Broken in a Run: " + PlayerPrefs.GetInt("Most Blocks Broken in a Run", 0).ToString());
                statMenuContainer.SetActive(true);
                break;

            case MENU_STATE.OPTIONS:
                optionsMenuContainer.SetActive(true);
                break;

            case MENU_STATE.END_OF_GAME:
                scoreText.SetText("Score: " + PlayerPrefs.GetInt("Last Game Score", 0).ToString());
                highscoreText.SetText("Highscore: " + PlayerPrefs.GetInt("Highscore", 0).ToString());
                if (PlayerPrefs.GetInt("Last Game Score", 0) >= PlayerPrefs.GetInt("Highscore", 0))
                    endMessage.SetText("New highscore!");
                else 
                    endMessage.SetText("Try again!");

                endOfGameContainer.SetActive(true);
                break;

            default:
                break;
        }
        currentState = newState;
    }
    public void StartGame()
    {
        if (GameController.instance.platforms.Count >= 8f)
        {
            //SceneManager.UnloadSceneAsync("MenuScene");
            //SceneManager.LoadScene("GameScene");
            GameController.instance.shouldGameStart = true;
        }
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
