using System;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    float timer = 0;
    bool startTimer = false;
    [SerializeField] Image fade;
    [SerializeField] TextMeshProUGUI user;
    [SerializeField] TextMeshProUGUI points;
    [SerializeField] TextMeshProUGUI bestTime;
    [SerializeField] TextMeshProUGUI loginUser;
    [SerializeField] RectTransform loginMenu;
    [SerializeField] RectTransform mainMenu;
    bool nameChanged = false;
    bool gameStart = false;
    bool shop = false;
    bool highscores = false;
    bool login = false;
    bool logout = false;
    bool fadeIn;

    private void Awake()
    {
        if (Global.username != null)
        {
            loginMenu.localScale = Vector3.zero;
            mainMenu.localScale = Vector3.one;
        }
    }

    private void Start()
    {
        //Make the alpha 1
        Color fixedColor = fade.color;
        fixedColor.a = 1;
        fade.color = fixedColor;

        //Set the 0 to zero then duration to 0
        fade.CrossFadeAlpha(0f, 2.5f, true);
        fade.rectTransform.localScale = Vector3.one;
        fadeIn = true;
        startTimer = true;
    }

    private void Update()
    {
        if (startTimer) timer += Time.deltaTime;

        if (timer > 2.5f)
        {
            if (gameStart) { SceneManager.LoadScene("Game"); }

            if (shop) { SceneManager.LoadScene("Shop"); }

            if (highscores) { SceneManager.LoadScene("HighScores"); }

            if (login)
            {
                loginMenu.localScale = Vector3.zero;
                mainMenu.localScale = Vector3.one;
                fade.CrossFadeAlpha(0f, 2.5f, true);
                login = false;
            }

            if (logout)
            {
                loginMenu.localScale = Vector3.one;
                mainMenu.localScale = Vector3.zero;
                fade.CrossFadeAlpha(0f, 2.5f, true);
                logout = false;
            }

            if (fadeIn)
            {
                fadeIn = false;
                startTimer = false;
                timer = 0;
                fade.rectTransform.localScale = Vector3.zero;
            }
        }

        if (timer > 5f)
        {
            fade.rectTransform.localScale = Vector3.zero;
            startTimer = false;
            timer = 0;
        }

        user.text = "User: " + Global.username;
        points.text = "Points: " + Global.points;
        if (Global.bestTime > 0)
        {
            bestTime.text = "Best Time: " + TimeSpan.FromSeconds(Global.bestTime).ToString(@"mm\:ss\.fff");
        }
        else
        {
            bestTime.text = "Best time not set";
        }
    }

    public void StartGame()
    {
        fade.rectTransform.localScale = Vector3.one;
        fade.CrossFadeAlpha(1f, 2.5f, true);
        startTimer = true;
        gameStart = true;
    }

    public void Shop()
    {
        fade.rectTransform.localScale = Vector3.one;
        fade.CrossFadeAlpha(1f, 2.5f, true);
        startTimer = true;
        shop = true;
    }

    public void Highscores()
    {
        fade.rectTransform.localScale = Vector3.one;
        fade.CrossFadeAlpha(1f, 2.5f, true);
        startTimer = true;
        highscores = true;
    }

    public void NameChanged()
    {
        nameChanged = true;
    }

    public void Login()
    {
        if (!nameChanged) return;

        fade.rectTransform.localScale = Vector3.one;
        fade.CrossFadeAlpha(1f, 2.5f, true);
        login = true;
        startTimer = true;
        Global.username = loginUser.text;
        Global.points = 0;
        Global.dashUpgrades = 0;
        Global.bestTime = 0;
    }

    public void Logout()
    {
        fade.rectTransform.localScale = Vector3.one;
        fade.CrossFadeAlpha(1f, 2.5f, true);
        logout = true;
        startTimer = true;
    }
}
