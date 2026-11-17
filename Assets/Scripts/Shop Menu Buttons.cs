using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopMenuButtonManager : MonoBehaviour
{
    float timer = 0;
    bool startTimer = false;
    bool mainMenu = false;
    bool fadeIn = false;
    [SerializeField] Image fade;
    [SerializeField] TextMeshProUGUI dash;
    [SerializeField] TextMeshProUGUI jump;
    [SerializeField] TextMeshProUGUI points;
    [SerializeField] TextMeshProUGUI dashUpgrade;
    [SerializeField] TextMeshProUGUI jumpUpgrade;

    private void Start()
    {
        // Make the alpha 1
        Color fixedColor = fade.color;
        fixedColor.a = 1;
        fade.color = fixedColor;

        // Fade in
        fade.rectTransform.localScale = Vector3.one;
        fade.CrossFadeAlpha(0f, 2.5f, true);
        startTimer = true;
        fadeIn = true;
    }

    private void Update()
    {
        if (startTimer) timer += Time.deltaTime;

        if (timer > 2.5f)
        {
            if (fadeIn)
            {
                fade.rectTransform.localScale = Vector3.zero;
                fadeIn = false;
                startTimer = false;
                timer = 0;
            }
            
            if (mainMenu) SceneManager.LoadScene("Main Menu");
        }

        dash.text = "Cost: " + (15 * (Global.dashUpgrades + 1)) + " Dash Cooldown: " + Mathf.Clamp(1.5f - (Global.dashUpgrades * 0.25f), 0.36f, 1.5f) + "s";
        jump.text = Global.jumpUpgrade ? "Jump Upgraded" : "Cost: 50 Default Jump";
        points.text = "Points: " + Global.points;
        if (Global.dashUpgrades == 5) dashUpgrade.text = "Max";
        if (Global.jumpUpgrade) jumpUpgrade.text = "Max";
    }


    public void MainMenu()
    {
        fade.rectTransform.localScale = Vector3.one;
        fade.CrossFadeAlpha(1f, 2.5f, true);
        startTimer = true;
        mainMenu = true;
    }

    public void DashUpgrade()
    {
        if (Global.dashUpgrades == 5) return;

        if (Global.points > 15 * (Global.dashUpgrades + 1))
        {
            Global.points -= 15 * (Global.dashUpgrades + 1);
            Global.dashUpgrades += 1;
        }
    }

    public void JumpUpgrade()
    {
        if (Global.jumpUpgrade) return;

        if (Global.points > 50)
        {
            Global.points -= 50;
            Global.jumpUpgrade = true;
        }
    }
}
