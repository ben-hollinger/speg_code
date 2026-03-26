using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    private PuzzleSlot[] slots;
    private bool isSolved = false;

    void Awake()
    {
       
    }

    void Start()
    {
        setColor();
        slots = FindObjectsByType<PuzzleSlot>(FindObjectsSortMode.InstanceID); 
        slots[0].setNeedsFilled(true);
        slots[0].GetComponent<Image>().color = Color.red;
        slots[24].setNeedsFilled(true);
        slots[24].GetComponent<Image>().color = Color.red;
        
        Debug.Log(slots[0].getNeedsFilled());
        Debug.Log(slots[24].getNeedsFilled());
        Debug.Log(slots[1].getNeedsFilled());
        Debug.Log(slots[1].isSolved());
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

        /*if (slots.All(slot => slot.isSolved()))
        {
            isSolved = true;
        }*/
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
