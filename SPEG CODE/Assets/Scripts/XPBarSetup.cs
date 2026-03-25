using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class XPBarSetup : MonoBehaviour
{
    private XPBar xpBar;

    void Awake()
    {
        xpBar = BuildXPBarUI();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)  xpBar.AddXP(25f);
        if (Keyboard.current.enterKey.wasPressedThisFrame)  xpBar.AddXP(100f);
        if (Keyboard.current.rKey.wasPressedThisFrame)      xpBar.Test_Reset();
    }

    private XPBar BuildXPBarUI()
    {
        GameObject canvasGO = new GameObject("XP Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject panel = CreateUIObject("XP Panel", canvasGO.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.04f);
        panelRect.anchorMax = new Vector2(0.9f, 0.12f);
        panelRect.offsetMin = panelRect.offsetMax = Vector2.zero;
        Image panelBG = panel.AddComponent<Image>();
        panelBG.color = new Color(0.08f, 0.08f, 0.12f, 0.9f);

        GameObject sliderGO = CreateUIObject("XP Slider", panel.transform);
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.15f, 0.15f);
        sliderRect.anchorMax = new Vector2(0.82f, 0.85f);
        sliderRect.offsetMin = sliderRect.offsetMax = Vector2.zero;

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.interactable = false; // XP bar isn't clicked by the player

        GameObject bg = CreateUIObject("Background", sliderGO.transform);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.18f, 0.18f, 0.25f);

        GameObject fillArea = CreateUIObject("Fill Area", sliderGO.transform);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = fillAreaRect.offsetMax = Vector2.zero;

        GameObject fillGO = CreateUIObject("Fill", fillArea.transform);
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = fillRect.offsetMax = Vector2.zero;
        Image fillImage = fillGO.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.6f, 1f);
        slider.fillRect = fillRect;

        GameObject levelGO = CreateUIObject("Level Text", panel.transform);
        RectTransform levelRect = levelGO.GetComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0f, 0f);
        levelRect.anchorMax = new Vector2(0.14f, 1f);
        levelRect.offsetMin = levelRect.offsetMax = Vector2.zero;
        Text levelText = levelGO.AddComponent<Text>();
        levelText.text = "Level 1";
        levelText.fontSize = 18;
        levelText.color = Color.white;
        levelText.alignment = TextAnchor.MiddleCenter;
        levelText.fontStyle = FontStyle.Bold;
        levelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject xpTextGO = CreateUIObject("XP Text", panel.transform);
        RectTransform xpRect = xpTextGO.GetComponent<RectTransform>();
        xpRect.anchorMin = new Vector2(0.83f, 0f);
        xpRect.anchorMax = new Vector2(1f, 1f);
        xpRect.offsetMin = xpRect.offsetMax = Vector2.zero;
        Text xpText = xpTextGO.AddComponent<Text>();
        xpText.text = "0 / 100 XP";
        xpText.fontSize = 15;
        xpText.color = new Color(0.75f, 0.75f, 0.9f);
        xpText.alignment = TextAnchor.MiddleCenter;
        xpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject hintGO = CreateUIObject("Hint", canvasGO.transform);
        RectTransform hintRect = hintGO.GetComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.1f, 0.13f);
        hintRect.anchorMax = new Vector2(0.9f, 0.19f);
        hintRect.offsetMin = hintRect.offsetMax = Vector2.zero;
        Text hintText = hintGO.AddComponent<Text>();
        hintText.text = "SPACE = +25 XP     ENTER = +100 XP (level up)     R = Reset";
        hintText.fontSize = 14;
        hintText.color = new Color(0.45f, 0.45f, 0.55f);
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        XPBar xpBarScript = canvasGO.AddComponent<XPBar>();
        xpBarScript.xpSlider = slider;
        xpBarScript.levelText = levelText;
        xpBarScript.xpText = xpText;
        xpBarScript.fillImage = fillImage;

        Debug.Log("[XPBarSetup] Ready! Press Space / Enter / R to test.");
        return xpBarScript;
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }
}