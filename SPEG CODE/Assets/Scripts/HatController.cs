using UnityEngine;

public class HatController : MonoBehaviour
{
    public GameObject hatMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hatMenu.SetActive(!hatMenu.activeSelf);
        }
    }
}
