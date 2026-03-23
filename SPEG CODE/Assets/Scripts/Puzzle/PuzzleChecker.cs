using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleChecker : MonoBehaviour
{
    private PuzzleSlot[] slots;
    private bool isSolved = false;
 
    void Start()
    {
        setColor();
        slots = FindObjectsByType<PuzzleSlot>(FindObjectsSortMode.None);
        slots[0].setNeedsFilled(true);
        slots[24].setNeedsFilled(true);
    }
    
    void Update()
    {
        if (slots.All(slot => slot.getIsFilled() == slot.getNeedsFilled()))
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
