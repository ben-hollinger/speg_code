using UnityEngine;
using UnityEngine.InputSystem;

public class HatController : MonoBehaviour
{
    public GameObject hatMenu;
    private HatSelector _hatSelector;

    void Awake()
    {
        _hatSelector = GetComponent<HatSelector>();
    }

    void Update()
    {
        if (_hatSelector != null) return;
        if (hatMenu == null) return;
        if (Keyboard.current == null || !Keyboard.current.hKey.wasPressedThisFrame) return;

        hatMenu.SetActive(!hatMenu.activeSelf);
    }
}
