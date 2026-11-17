using NUnit.Framework;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static string username = null;
    public static int points = 0;
    public static float bestTime = 0;
    public static bool jumpUpgrade = false;
    public static int dashUpgrades = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
