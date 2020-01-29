using System.Collections;
using System.Collections.Generic;
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
    public float splashAnimationSpeed = .1f;
    public GameObject mainMenuLogo;

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
        while (splashLogo.GetComponent<Image>().color.a < 1f)
        {
            splashLogo.GetComponent<Image>().color = new Color(splashLogo.GetComponent<Image>().color.r, splashLogo.GetComponent<Image>().color.g, splashLogo.GetComponent<Image>().color.b, splashLogo.GetComponent<Image>().color.a + splashAnimationSpeed);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(2.5f);
        GameController.instance.shouldPlatformsMove = true;

        while (splashLogo.GetComponent<RectTransform>().anchoredPosition != mainMenuLogo.GetComponent<RectTransform>().anchoredPosition)
        {
            splashLogo.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(splashLogo.GetComponent<RectTransform>().anchoredPosition, mainMenuLogo.GetComponent<RectTransform>().anchoredPosition, 5f);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(2.5f);

        while (GameController.instance.platforms.Count < 8f)
            yield return new WaitForEndOfFrame();
        ChangeMenuState(1);
    }
    public void ChangeMenuState(int newStateID)
    {
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
                statMenuContainer.SetActive(true);
                break;

            case MENU_STATE.OPTIONS:
                optionsMenuContainer.SetActive(true);
                break;

            case MENU_STATE.END_OF_GAME:
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
