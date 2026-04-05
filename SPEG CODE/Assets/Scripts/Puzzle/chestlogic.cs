using UnityEngine;

public class chestlogic : MonoBehaviour
{
    
    public GameObject canvasObject;
    public KeyCode interactKey = KeyCode.E;
    private bool playerInRange = false;

    void Start()
    {
        canvasObject.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            canvasObject.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

