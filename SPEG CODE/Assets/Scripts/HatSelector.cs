using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

public class HatSelector : MonoBehaviour
{
    [Header("Hat Panel UI")]
    public GameObject hatPanel;

    [Header("Hats (assign in Inspector)")]
    public GameObject somberoParent;
    public GameObject vikingHat;
    public GameObject pjHat;
    public GameObject crownParent;
    public GameObject pillboxHatParent;

    [Header("Ability Buttons (assign in Inspector)")]
    public Button dashingStrikeButton;
    public Button shieldButton;
    public Button doubleJumpButton;
    public Button grappleButton;
    public Button magicBlastButton;

    private GameObject[] allHats;

    void Start()
    {
        allHats = new GameObject[]
        {
            somberoParent,
            vikingHat,
            pjHat,
            crownParent,
            pillboxHatParent
        };

        hatPanel.SetActive(false);
        SetAllHatsInactive();

        dashingStrikeButton.onClick.AddListener(() => EquipHat(somberoParent));
        shieldButton.onClick.AddListener(() => EquipHat(vikingHat));
        doubleJumpButton.onClick.AddListener(() => EquipHat(pjHat));
        grappleButton.onClick.AddListener(() => EquipHat(crownParent));
        magicBlastButton.onClick.AddListener(() => EquipHat(pillboxHatParent));
    }

    void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            hatPanel.SetActive(!hatPanel.activeSelf);
        }
    }

    void EquipHat(GameObject selectedHat)
    {
        SetAllHatsInactive();
        selectedHat.SetActive(true);
        hatPanel.SetActive(false);
    }

    void SetAllHatsInactive()
    {
        foreach (GameObject hat in allHats)
        {
            hat.SetActive(false);
        }
    }
}