using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
    }
<<<<<<< HEAD
    

    public void OnPointerDown(PointerEventData eventData)
    {
    }
=======

    public void OnPointerDown(PointerEventData eventData) { }
>>>>>>> origin/main

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

<<<<<<< HEAD
        transform.SetParent(transform.root); // move to top canvas while dragging
        
        
=======
        originalParent = transform.parent;

        PuzzleSlot slot = originalParent.GetComponent<PuzzleSlot>();
        if (slot != null)
            slot.ClearSlot();

        // Move to root canvas so it renders on top and stays in the right space
        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Divide by canvas scale so delta is correct in both Overlay and Camera modes
        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
>>>>>>> origin/main
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
<<<<<<< HEAD
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition+=eventData.delta;
        
=======

        // If it wasn't dropped on a slot, return it to where it came from
        if (transform.parent == rootCanvas.transform)
            transform.SetParent(originalParent, true);
>>>>>>> origin/main
    }
}