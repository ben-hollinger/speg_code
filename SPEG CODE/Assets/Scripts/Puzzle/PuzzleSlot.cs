using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleSlot : MonoBehaviour, IDropHandler
{
    private bool isFilled = false;
    private bool needsFilled = false;
    public Color emptyColor = Color.black;
    public Color filledColor = Color.green;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().localPosition;
            
            GameObject item = eventData.pointerDrag;
            item.transform.SetParent(transform);
            
            isFilled = true;
            UpdateColor();
            
            
        }
    }

    public void ClearSlot()
    {
        isFilled = false;
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (isFilled){
            GetComponent<Image>().color = filledColor;}
        else
        {
            GetComponent<Image>().color = emptyColor;
        }
    }
    
    public void setNeedsFilled(bool value)
    {
        needsFilled = value;
    }
    
    public bool getNeedsFilled(){
        return needsFilled;
    }
    
    public void setIsFilled(bool value)
    {
        isFilled = value;
    }
    
    public bool getIsFilled()
    {
        return isFilled;
    }
}
