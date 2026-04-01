using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    [Header("Bar Fill Image")]
    [Tooltip("The Image component used as the fill — set its Image Type to 'Filled'.")]
    public Image fillImage;

    [Header("Optional Labels")]
    [Tooltip("Shows e.g. '30 / 80 XP'. Leave empty to hide.")]
    public TextMeshProUGUI xpLabel;

    [Tooltip("Shows e.g. '3 / 8 pieces collected'. Leave empty to hide.")]
    public TextMeshProUGUI piecesLabel;

    [Header("Chest Unlock Indicator")]
    [Tooltip("A UI element that appears when the bar is full.")]
    public GameObject chestReadyIndicator;

    void OnEnable()
    {
        XPManager.OnXPChanged += RefreshUI;
        XPManager.OnBarFull   += OnBarFull;
    }

    void OnDisable()
    {
        XPManager.OnXPChanged -= RefreshUI;
        XPManager.OnBarFull   -= OnBarFull;
    }

    void Start()
    {
        if (chestReadyIndicator != null) chestReadyIndicator.SetActive(false);
    }

    // Poll every frame as a fallback in case the event was missed
    void Update()
    {
        if (XPManager.Instance == null) return;
        RefreshUI(XPManager.Instance.CurrentXP, XPManager.Instance.XPFraction);
    }

    void RefreshUI(int currentXP, float fraction)
    {
        if (fillImage != null)
            fillImage.fillAmount = fraction;

        if (xpLabel != null && XPManager.Instance != null)
            xpLabel.text = $"{currentXP} / {XPManager.Instance.xpToFillBar} XP";

        if (piecesLabel != null && XPManager.Instance != null)
            piecesLabel.text = $"{XPManager.Instance.CollectedPieceIDs.Count} pieces collected";
    }

    void OnBarFull()
    {
        if (chestReadyIndicator != null) chestReadyIndicator.SetActive(true);
    }
}
