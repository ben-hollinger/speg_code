using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleSlot : MonoBehaviour, IDropHandler
{
    private bool isFilled = false;
    private bool needsFilled = false;
    public Color emptyColor = Color.black;
    public Color filledColor = Color.green;

    void Start() { }
    void Update() { }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        GameObject item = eventData.pointerDrag;
        RectTransform itemRect = item.GetComponent<RectTransform>();

        // Parent first so anchoredPosition is relative to this slot
        item.transform.SetParent(transform, true);

        // Now snap to centre using anchoredPosition, not localPosition
        itemRect.anchoredPosition = Vector2.zero;

        isFilled = true;
        Debug.Log($"Filled slot: {gameObject.name}, fill status: {isFilled}");
        UpdateColor();
    }

    public void ClearSlot()
    {
        isFilled = false;
        UpdateColor();
    }

    public void UpdateColor()
    {
        GetComponent<Image>().color = isFilled ? filledColor : emptyColor;
    }

    public void setNeedsFilled(bool value) => needsFilled = value;
    public bool getNeedsFilled() => needsFilled;
    public bool getIsFilled() => isFilled;
    public bool isSolved() => isFilled == needsFilled;
}