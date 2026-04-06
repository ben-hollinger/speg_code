using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    private PuzzleSlot[] slots;
    private bool isSolved = false;
<<<<<<< HEAD
=======
    public int[] fillRequirements; 
>>>>>>> origin/main

    void Awake()
    {
       
    }

    void Start()
    {
        setColor();
<<<<<<< HEAD
        slots = FindObjectsByType<PuzzleSlot>(FindObjectsSortMode.InstanceID); 
        slots[0].setNeedsFilled(true);
        slots[0].GetComponent<Image>().color = Color.red;
        slots[24].setNeedsFilled(true);
        slots[24].GetComponent<Image>().color = Color.red;
=======
        slots = FindObjectsByType<PuzzleSlot>(FindObjectsSortMode.InstanceID);
        foreach (var requirement in fillRequirements)
        {
            slots[requirement].setNeedsFilled(true);
            //slots[requirement].GetComponent<Image>().color = Color.red; debug
        }
>>>>>>> origin/main
        
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
<<<<<<< HEAD
            //give key here
            DisableUI();
=======
            //if (KeyManager.Instance != null)
//                KeyManager.Instance.AddKey();
                
            //}
            KeyManager.Instance.AddKey();
            
>>>>>>> origin/main
        }
    }
    
    private void DisableUI()
    {
        var uiElements = GetComponentsInChildren<CanvasGroup>();

        foreach (var ui in uiElements)
        {
            ui.alpha = 0;
<<<<<<< HEAD
            ui.interactable = false;
=======
            ui.interactable = false; 
>>>>>>> origin/main
            ui.blocksRaycasts = false;
        }
    }
    
}
