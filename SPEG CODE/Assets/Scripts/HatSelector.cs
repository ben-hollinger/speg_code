using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HatSelector : MonoBehaviour
{
    // ═════════════════════════════════════════════════════════════════════════
    // LEVEL SETTING — change this one number for each level's scene.
    //
    //   1 = Magic Blast only
    //   2 = Magic Blast + Double Jump
    //   3 = Magic Blast + Double Jump + Dashing Strike
    //   4 = Magic Blast + Double Jump + Dashing Strike + Shield
    //   5 = All abilities (+ Grapple)
    // ═════════════════════════════════════════════════════════════════════════
    [SerializeField] private int CURRENT_LEVEL = 4; // edited so now you can change level based on scene

    [Header("Hat Panel UI")]
    public GameObject hatPanel;

    [Header("Hats (assign in Inspector)")]
    public GameObject somberoParent;
    public GameObject vikingHat;
    public GameObject pjHat;
    public GameObject crownParent;
    public GameObject pillboxHatParent;

    [Header("Ability Buttons (assign in Inspector)")]
    public Button magicBlastButton;     // unlocked at Level 1
    public Button doubleJumpButton;     // unlocked at Level 2
    public Button dashingStrikeButton;  // unlocked at Level 3
    public Button shieldButton;         // unlocked at Level 4
    public Button grappleButton;        // unlocked at Level 5

    [Header("Ability Components (assign in Inspector)")]
    public ShieldAbility shieldAbility;
    public GrappleController grappleController;
    // Uncomment as other abilities are implemented:
    public MagicBlastShooter magicBlastShooter;
    // public DoubleJump doubleJumpAbility;
    public DashStrikeController dashingStrikeAbility;

    private GameObject[] allHats;
    private Button[] _abilityButtons;

    // ── Awake: hide locked buttons before the first frame ever renders ────────

    void Awake()
    {
        _abilityButtons = new Button[]
        {
            magicBlastButton,       // Level 1
            doubleJumpButton,       // Level 2
            dashingStrikeButton,    // Level 3
            shieldButton,           // Level 4
            grappleButton           // Level 5
        };

        for (int i = 0; i < _abilityButtons.Length; i++)
        {
            if (_abilityButtons[i] != null)
                _abilityButtons[i].gameObject.SetActive(i + 1 <= CURRENT_LEVEL);
        }
    }

    // ── Start: wire everything else up ───────────────────────────────────────

    void Start()
    {
        allHats = new GameObject[]
        {
            pillboxHatParent,   // Magic Blast hat
            pjHat,              // Double Jump hat
            somberoParent,      // Dashing Strike hat
            vikingHat,          // Shield hat
            crownParent         // Grapple hat
        };

        hatPanel.SetActive(false);
        SetAllHatsInactive();
        DisableAllAbilities();

        // Wire up button clicks
        magicBlastButton.onClick.AddListener(() =>
        {
            EquipHat(pillboxHatParent);
            ActivateAbility(AbilityType.MagicBlast);
        });

        doubleJumpButton.onClick.AddListener(() =>
        {
            EquipHat(pjHat);
            ActivateAbility(AbilityType.DoubleJump);
        });

        dashingStrikeButton.onClick.AddListener(() =>
        {
            EquipHat(somberoParent);
            ActivateAbility(AbilityType.DashingStrike);
        });

        shieldButton.onClick.AddListener(() =>
        {
            EquipHat(vikingHat);
            ActivateAbility(AbilityType.Shield);
        });

        grappleButton.onClick.AddListener(() =>
        {
            EquipHat(crownParent);
            ActivateAbility(AbilityType.Grapple);
        });
    }

    void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
            hatPanel.SetActive(!hatPanel.activeSelf);
    }

    // ── Ability switching ─────────────────────────────────────────────────────

    private enum AbilityType { MagicBlast, DoubleJump, DashingStrike, Shield, Grapple }

    void DisableAllAbilities()
    {
        if (shieldAbility != null)     shieldAbility.enabled     = false;
        if (grappleController != null) grappleController.enabled = false;
        if (magicBlastShooter != null)  magicBlastShooter.enabled  = false;
        // if (doubleJumpAbility    != null) doubleJumpAbility.enabled    = false;
        if (dashingStrikeAbility != null) dashingStrikeAbility.enabled = false;
    }

    void ActivateAbility(AbilityType type)
    {
        DisableAllAbilities();

        switch (type)
        {
            case AbilityType.MagicBlast:
                if (magicBlastShooter != null)
                    magicBlastShooter.enabled = true;
                else
                    Debug.LogWarning("[HatSelector] MagicBlastShooter not assigned!");
                break;

            case AbilityType.DoubleJump:
                Debug.Log("[HatSelector] Double Jump selected (not yet implemented)");
                break;

            case AbilityType.DashingStrike:
                if (dashingStrikeAbility != null)
                    dashingStrikeAbility.enabled = true;
                else
                    Debug.Log("[HatSelector] Dashing Strike selected (not yet implemented)");
                break;

            case AbilityType.Shield:
                if (shieldAbility != null)
                    shieldAbility.enabled = true;
                else
                    Debug.LogWarning("[HatSelector] ShieldAbility not assigned!");
                break;

            case AbilityType.Grapple:
                if (grappleController != null)
                    grappleController.enabled = true;
                else
                    Debug.LogWarning("[HatSelector] GrappleController not assigned!");
                break;
        }
    }

    // ── Hat switching ─────────────────────────────────────────────────────────

    void EquipHat(GameObject selectedHat)
    {
        SetAllHatsInactive();
        selectedHat.SetActive(true);
        hatPanel.SetActive(false);
    }

    void SetAllHatsInactive()
    {
        foreach (GameObject hat in allHats)
            hat.SetActive(false);
    }
}
