using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class XPBar : MonoBehaviour
{
    public Slider xpSlider;
    public Text levelText;
    public Text xpText;
    public Image fillImage;

    [Header("XP Settings")] public int currentLevel = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;

    public float xpScalingFactor = 1.25f;

    public float fillAnimationSpeed = 2f;

    void Start()
    {
        RefreshUI(instant: true);
    }

    public void AddXP(float amount)
    {
        if (amount <= 0) return;

        currentXP += amount;
        Debug.Log("[XPBar] +" + amount + " XP. Total: " + currentXP + "/" + xpToNextLevel);


        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        StopAllCoroutines();
        StartCoroutine(AnimateBar());
    }

    public void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = Mathf.Round(xpToNextLevel * xpScalingFactor);
        Debug.Log("[XPBar] LEVEL UP! Now Level " + currentLevel + ". Next level needs " + xpToNextLevel + " XP.");
        UpdateLevelText();
    }

    private IEnumerator AnimateBar()
    {
        float targetFill = currentXP / xpToNextLevel;

        while (!Mathf.Approximately(xpSlider.value, targetFill))
        {
            xpSlider.value = Mathf.MoveTowards(xpSlider.value, targetFill, fillAnimationSpeed * Time.deltaTime);
            UpdateFillColour(xpSlider.value);
            UpdateXPText();
            yield return null;
        }
        
        xpSlider.value = targetFill;
        UpdateFillColour(targetFill);
        UpdateXPText();
    }
    
    private void RefreshUI(bool instant = false)
    {
        if (instant)
        {
            float fill = currentXP / xpToNextLevel;
            xpSlider.value = fill;
            UpdateFillColour(fill);
        }

        UpdateLevelText();
        UpdateXPText();
    }
    private void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "Level " + currentLevel;
    }
 
    private void UpdateXPText()
    {
        if (xpText != null)
            xpText.text = Mathf.FloorToInt(currentXP) + " / " + Mathf.FloorToInt(xpToNextLevel) + " XP";
    }
 
    private void UpdateFillColour(float t)
    {
        if (fillImage != null)
            fillImage.color = Color.Lerp(new Color(0.2f, 0.6f, 1f), new Color(0.8f, 0.3f, 1f), t);
    }
 
    [ContextMenu("Test: Add 25 XP")]
    public void Test_Add25XP() => AddXP(25f);
 
    [ContextMenu("Test: Add 100 XP (level up)")]
    public void Test_Add100XP() => AddXP(100f);
 
    [ContextMenu("Test: Reset")]
    public void Test_Reset()
    {
        currentLevel = 1;
        currentXP = 0f;
        xpToNextLevel = 100f;
        StopAllCoroutines();
        RefreshUI(instant: true);
        Debug.Log("[XPBar] Reset to Level 1.");
    }
}



