using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class XPBar : MonoBehaviour
{
    public static XPBar Instance { get; private set; }

    [Header("UI References")]
    public Slider xpSlider;
    public Text   levelText;
    public Text   xpText;
    public Image  fillImage;

    [Header("XP Settings")]
    public int   currentLevel     = 1;
    public float currentXP        = 0f;
    public float xpToNextLevel    = 100f;
    public float xpScalingFactor  = 1.25f;
    public float fillAnimationSpeed = 2f;

    [Header("Puzzle Piece Bridge")]
    [Tooltip("If true, this bar listens to XPManager (puzzle pieces) instead of " +
             "only AddXP() calls. The chest unlock still fires via XPManager.OnBarFull.")]
    public bool usePuzzlePieceXP = true;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

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
        RefreshUI(instant: true);
    }

    // ── Puzzle piece bridge ───────────────────────────────────────────────────

    private void OnPuzzleXPChanged(int xp, float fraction)
    {
        if (!usePuzzlePieceXP || XPManager.Instance == null) return;

        // Mirror XPManager values into this bar's fields so StatsUI reads correctly
        currentXP      = xp;
        xpToNextLevel  = XPManager.Instance.xpToFillBar;

        StopAllCoroutines();
        StartCoroutine(AnimateBar());
    }

    // ── Direct XP (non-puzzle, e.g. enemy kills) ──────────────────────────────

    public void AddXP(float amount)
    {
        if (usePuzzlePieceXP) return;  // puzzle mode — ignore direct calls
        if (amount <= 0) return;

        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP     -= xpToNextLevel;
            xpToNextLevel  = Mathf.Round(xpToNextLevel * xpScalingFactor);
            currentLevel++;
            UpdateLevelText();
        }

        StopAllCoroutines();
        StartCoroutine(AnimateBar());
    }

    // ── Animation ─────────────────────────────────────────────────────────────

    private IEnumerator AnimateBar()
    {
        float targetFill = xpToNextLevel > 0 ? Mathf.Clamp01(currentXP / xpToNextLevel) : 0f;

        while (!Mathf.Approximately(xpSlider.value, targetFill))
        {
            xpSlider.value = Mathf.MoveTowards(xpSlider.value, targetFill,
                                                fillAnimationSpeed * Time.deltaTime);
            UpdateFillColour(xpSlider.value);
            UpdateXPText();
            yield return null;
        }

        xpSlider.value = targetFill;
        UpdateFillColour(targetFill);
        UpdateXPText();
    }

    // ── UI helpers ────────────────────────────────────────────────────────────

    private void RefreshUI(bool instant = false)
    {
        if (instant && xpSlider != null)
        {
            float fill = xpToNextLevel > 0 ? currentXP / xpToNextLevel : 0f;
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
            xpText.text = Mathf.FloorToInt(currentXP) + " / " +
                          Mathf.FloorToInt(xpToNextLevel) + " XP";
    }

    private void UpdateFillColour(float t)
    {
        if (fillImage != null)
            fillImage.color = Color.Lerp(new Color(0.2f, 0.6f, 1f),
                                         new Color(0.8f, 0.3f, 1f), t);
    }

    // ── Context menu tests ────────────────────────────────────────────────────
    [ContextMenu("Test: Add 25 XP")]  public void Test_Add25XP()  => AddXP(25f);
    [ContextMenu("Test: Add 100 XP")] public void Test_Add100XP() => AddXP(100f);
    [ContextMenu("Test: Reset")]
    public void Test_Reset()
    {
        currentLevel  = 1; currentXP = 0f; xpToNextLevel = 100f;
        StopAllCoroutines();
        RefreshUI(instant: true);
    }
}
