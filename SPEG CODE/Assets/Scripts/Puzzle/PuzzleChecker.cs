using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleChecker : MonoBehaviour
{
    private PuzzleSlot[] slots;
    private bool isSolved = false;
    private void Awake()
    {
      
    }
    void Start()
    {
        StartCoroutine(StartAfterDelay());

    }

    IEnumerator StartAfterDelay(){
        yield return new WaitForSeconds(1f); // wait 1 second

        Debug.Log("Started after delay");
        slots = FindObjectsOfType<PuzzleSlot>();  
    }
    void Update()
    {
        if (slots[24].getIsFilled() && slots[0].getIsFilled())
        {
            isSolved = true;
        }
        else
        {
            isSolved = false;
        }
        setColor();
    }

    private void setColor()
    {
        if (isSolved)
        {
            GetComponent<TextMeshProUGUI>().color = Color.green;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().color = Color.red;
        }
    }
}
