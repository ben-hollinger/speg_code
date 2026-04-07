using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    private PuzzleSlot[] slots;
    private bool isSolved = false;
    public int[] fillRequirements; 

    void Awake()
    {
       
    }

    void Start()
    {
        setColor();
        slots = FindObjectsByType<PuzzleSlot>(FindObjectsSortMode.InstanceID);
        foreach (var requirement in fillRequirements)
        {
            slots[requirement].setNeedsFilled(true);
            //slots[requirement].GetComponent<Image>().color = Color.red; debug
        }
        
    }
    
    void Update()
    {
        CheckPuzzle();
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

    public void CheckPuzzle()
    {
        if(isSolved){return;}

        if (slots.All(slot => slot.isSolved()))
        {
            isSolved = true;
            //if (KeyManager.Instance != null)
//                KeyManager.Instance.AddKey();
                
            //}
            KeyManager.Instance.AddKey();
            
        }
    }
    
    private void DisableUI()
    {
        var uiElements = GetComponentsInChildren<CanvasGroup>();

        foreach (var ui in uiElements)
        {
            ui.alpha = 0;
            ui.interactable = false; 
            ui.blocksRaycasts = false;
        }
    }
    
}
