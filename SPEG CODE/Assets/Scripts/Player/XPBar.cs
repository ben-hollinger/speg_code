using UnityEngine;
using UnityEngine.UI;
using System.Collections;
<<<<<<< HEAD
=======
using UnityEngine.Serialization;
using TMPro;
>>>>>>> origin/main

public class XPBar : MonoBehaviour
{
    public static XPBar Instance { get; private set; }

    public Slider xpSlider;
    public Text levelText;
    public Text xpText;
<<<<<<< HEAD
=======
    [Tooltip("Optional. Use when XP text is TextMeshPro instead of legacy UI Text.")]
    public TextMeshProUGUI xpTextMeshPro;
    [Tooltip("Optional. Use when level text is TextMeshPro instead of legacy UI Text.")]
    public TextMeshProUGUI levelTextMeshPro;
    [Tooltip("Fill image (Image Type: Filled). Used when xpSlider is not assigned.")]
>>>>>>> origin/main
    public Image fillImage;

    [Header("XP Settings")] public int currentLevel = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;
    [SerializeField] private float xpPerEnemyKill = 25f;

    public float xpScalingFactor = 1.25f;
    public float fillAnimationSpeed = 2f;

<<<<<<< HEAD
=======
    [Header("Puzzle Piece Bridge")]
    [FormerlySerializedAs("usePuzzlePieceXP")]
    public bool usePuzzlePieceXP = false;

>>>>>>> origin/main
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

<<<<<<< HEAD
    void Start()
    {
=======
    void OnEnable()
    {
        if (usePuzzlePieceXP)
            XPManager.OnXPChanged += OnPuzzleXPChanged;
    }

    void OnDisable()
    {
        XPManager.OnXPChanged -= OnPuzzleXPChanged;
    }

    void Start()
    {
        SyncPuzzlePieceXP();
>>>>>>> origin/main
        RefreshUI(instant: true);
    }

    public void AddXP(float amount)
    {
<<<<<<< HEAD
=======
        if (usePuzzlePieceXP) return;
>>>>>>> origin/main
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
        ApplyProgressionToPlayer();
    }

    public void ApplyProgressionToPlayer()
    {
        if (PlayerController.Instance == null) return;
        var stats = PlayerController.Instance.GetComponent<PlayerStats>();
        if (stats == null) return;

        int level = currentLevel;
        PlayerController.Instance.SetMeleeDamage(PlayerProgression.MeleeDamageForLevel(level));
        stats.ApplyMaxHealthFromProgression(PlayerProgression.MaxHealthForLevel(level));
    }

<<<<<<< HEAD
    private IEnumerator AnimateBar()
    {
        float targetFill = currentXP / xpToNextLevel;
=======
    private void OnPuzzleXPChanged(int xp, float fraction)
    {
        if (!usePuzzlePieceXP || XPManager.Instance == null) return;

        currentXP = xp;
        xpToNextLevel = XPManager.Instance.xpToFillBar;

        StopAllCoroutines();
        StartCoroutine(AnimateBar());
    }

    private void SyncPuzzlePieceXP()
    {
        if (!usePuzzlePieceXP || XPManager.Instance == null) return;

        currentXP = XPManager.Instance.CurrentXP;
        xpToNextLevel = XPManager.Instance.xpToFillBar;
    }

    private IEnumerator AnimateBar()
    {
        float targetFill = xpToNextLevel > 0f ? currentXP / xpToNextLevel : 0f;
>>>>>>> origin/main

        if (xpSlider != null)
        {
            while (!Mathf.Approximately(xpSlider.value, targetFill))
            {
                xpSlider.value = Mathf.MoveTowards(xpSlider.value, targetFill, fillAnimationSpeed * Time.deltaTime);
                UpdateFillColour(xpSlider.value);
                UpdateXPText();
                yield return null;
            }

            xpSlider.value = targetFill;
            UpdateFillColour(targetFill);
        }
<<<<<<< HEAD
=======
        else if (fillImage != null)
        {
            while (!Mathf.Approximately(fillImage.fillAmount, targetFill))
            {
                fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, targetFill, fillAnimationSpeed * Time.deltaTime);
                UpdateFillColour(fillImage.fillAmount);
                UpdateXPText();
                yield return null;
            }

            fillImage.fillAmount = targetFill;
            UpdateFillColour(targetFill);
        }
>>>>>>> origin/main

        UpdateXPText();
    }
    
    private void RefreshUI(bool instant = false)
    {
<<<<<<< HEAD
        if (instant && xpSlider != null)
        {
            float fill = currentXP / xpToNextLevel;
            xpSlider.value = fill;
            UpdateFillColour(fill);
=======
        if (instant)
        {
            float fill = xpToNextLevel > 0f ? currentXP / xpToNextLevel : 0f;
            if (xpSlider != null)
            {
                xpSlider.value = fill;
                UpdateFillColour(fill);
            }
            else if (fillImage != null)
            {
                fillImage.fillAmount = fill;
                UpdateFillColour(fill);
            }
>>>>>>> origin/main
        }

        UpdateLevelText();
        UpdateXPText();
    }
    private void UpdateLevelText()
    {
<<<<<<< HEAD
        if (levelText != null)
            levelText.text = "Level " + currentLevel;
=======
        string s = "Level " + currentLevel;
        if (levelText != null)
            levelText.text = s;
        if (levelTextMeshPro != null)
            levelTextMeshPro.text = s;
>>>>>>> origin/main
    }
 
    private void UpdateXPText()
    {
<<<<<<< HEAD
        if (xpText != null)
            xpText.text = Mathf.FloorToInt(currentXP) + " / " + Mathf.FloorToInt(xpToNextLevel) + " XP";
=======
        string s = Mathf.FloorToInt(currentXP) + " / " + Mathf.FloorToInt(xpToNextLevel) + " XP";
        if (xpText != null)
            xpText.text = s;
        if (xpTextMeshPro != null)
            xpTextMeshPro.text = s;
>>>>>>> origin/main
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
        ApplyProgressionToPlayer();
        Debug.Log("[XPBar] Reset to Level 1.");
    }
}



