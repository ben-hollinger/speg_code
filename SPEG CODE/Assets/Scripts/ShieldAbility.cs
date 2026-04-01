using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShieldAbility : MonoBehaviour
{
    [Header("Shield Settings")]
    [Tooltip("How long (seconds) the shield stays active per use.")]
    public float shieldDuration = 2f;

    [Tooltip("Cooldown (seconds) before the shield can be used again.")]
    public float shieldCooldown = 5f;

    [Header("Forcefield Visual")]
    [Tooltip("Drag the Shield FX GameObject here. Must be a direct child of the root player object, NOT parented to any mixamorig bone.")]
    public GameObject forcefieldVisual;

    [Tooltip("Optional: bone Transform the FX follows each frame, e.g. mixamorig:Spine. Leave empty to stay at the player root.")]
    public Transform shieldAnchor;

    private bool  _shieldActive  = false;
    private bool  _onCooldown    = false;
    private float _shieldTimer   = 0f;
    private float _cooldownTimer = 0f;
    private Renderer[] _fxRenderers;

    void Awake()
    {
        if (forcefieldVisual != null)
            _fxRenderers = forcefieldVisual.GetComponentsInChildren<Renderer>(true);
        HideFX();
    }

    void OnEnable()  { HideFX(); }

    void OnDisable()
    {
        StopAllCoroutines();
        _shieldActive  = false;
        _onCooldown    = false;
        _shieldTimer   = 0f;
        _cooldownTimer = 0f;
        HideFX();
    }

    void Update()
    {
        if (_shieldActive && shieldAnchor != null && forcefieldVisual != null)
            forcefieldVisual.transform.position = shieldAnchor.position;

        if (_onCooldown)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f) { _onCooldown = false; _cooldownTimer = 0f; }
        }

        if (_shieldActive)
        {
            _shieldTimer -= Time.deltaTime;
            if (_shieldTimer <= 0f) DeactivateShield();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
            TryActivateShield();
    }

    public bool TryTakeDamage(float amount)
    {
        if (_shieldActive) { Debug.Log($"[ShieldAbility] Blocked {amount} damage!"); return true; }
        return false;
    }

    public bool IsShieldActive => _shieldActive;
    public float CooldownFraction => _onCooldown ? _cooldownTimer / shieldCooldown : 0f;

    void TryActivateShield()
    {
        if (_shieldActive || _onCooldown) return;
        _shieldActive = true;
        _shieldTimer  = shieldDuration;
        ShowFX();
        Debug.Log("[ShieldAbility] Shield activated!");
    }

    void DeactivateShield()
    {
        _shieldActive  = false;
        _shieldTimer   = 0f;
        _onCooldown    = true;
        _cooldownTimer = shieldCooldown;
        HideFX();
        Debug.Log("[ShieldAbility] Shield expired.");
    }

    void ShowFX() { if (_fxRenderers != null) foreach (var r in _fxRenderers) r.enabled = true; }
    void HideFX() { if (_fxRenderers != null) foreach (var r in _fxRenderers) r.enabled = false; }
}
