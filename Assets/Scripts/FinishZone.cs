using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishZone : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [HideInInspector] public float finishTime;
    [SerializeField] string menu;
    [SerializeField] Image fade;
    [SerializeField] Image bigFade;
    [SerializeField] RectTransform finishText;
    [SerializeField] TextMeshProUGUI finishTimeText;
    [SerializeField] TextMeshProUGUI pointsText;
    bool fadeIn = false;
    bool finished = false;
    bool pointsAdded = false;
    float timer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!finished)
            {
                // Disables controls
                player.controls.Disable();
                Time.timeScale = 0.5f;

                // Saves time
                finishTime = player.time;

                // Shows the final text and fade
                finished = true;
                fade.CrossFadeAlpha(0.85f, 2.5f, true);
            }
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        //Make the alpha 1
        Color fixedColor = fade.color;
        fixedColor.a = 1;
        fade.color = fixedColor;
        fixedColor = bigFade.color;
        fixedColor.a = 1;
        bigFade.color = fixedColor;

        //Set the 0 to zero then duration to 0
        fade.CrossFadeAlpha(0f, 0f, true);
        bigFade.CrossFadeAlpha(0f, 2.5f, true);
        fadeIn = true;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (fadeIn)
        {
            timer += Time.unscaledDeltaTime;
            
            if (timer > 2.5f)
            {
                timer = 0;
                fadeIn = false;
                Time.timeScale = 1f;
            }
        }

        if (finished == true)
        {
            // Starts the timer to leave
            timer += Time.unscaledDeltaTime;

            if (finishTime < Global.bestTime || Global.bestTime == 0) Global.bestTime = finishTime;
            if (!pointsAdded)
            {
                if (finishTime < 20) Global.points += 15;
                else if (finishTime < 35) Global.points += 10;
                else if (finishTime < 60) Global.points += 5;
                pointsAdded = true;
            }

            finishTimeText.text = "Time: " + TimeSpan.FromSeconds(finishTime).ToString(@"mm\:ss\.fff");
            pointsText.text = "Points: " + Global.points;

            if (timer > 2.5f)
            {
                finishText.localScale = Vector3.one;
            }

            if (timer > 7.5f)
            {
                bigFade.CrossFadeAlpha(1f, 2.5f, true);
            }

            // Sends you to the menu
            if (timer > 10) SceneManager.LoadScene(menu);
        }
    }
}
