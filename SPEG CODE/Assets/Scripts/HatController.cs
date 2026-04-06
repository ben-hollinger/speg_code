using UnityEngine;
<<<<<<< HEAD
=======
using UnityEngine.InputSystem;
>>>>>>> origin/main

public class HatController : MonoBehaviour
{
    public GameObject hatMenu;
<<<<<<< HEAD

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hatMenu.SetActive(!hatMenu.activeSelf);
        }
=======
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
>>>>>>> origin/main
    }
}
