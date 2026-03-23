using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform parent;

    private void Awake()
    {rectTransform=GetComponent<RectTransform>();
    canvasGroup=GetComponent<CanvasGroup>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    public void OnPointerDown(PointerEventData eventData){}

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        
        parent = transform.parent;
        
        PuzzleSlot slot = parent.GetComponent<PuzzleSlot>();
        if (slot != null)
        {
            slot.ClearSlot();
        }

        transform.SetParent(transform.root); // move to top canvas while dragging
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition+=eventData.delta;
    }
   
    
}
