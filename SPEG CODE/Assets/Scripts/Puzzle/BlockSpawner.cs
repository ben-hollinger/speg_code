using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSpawner : MonoBehaviour, IPointerDownHandler ,IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private GameObject block;
    private PuzzleSlot[] slots;
    [SerializeField] private Transform canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slots = FindObjectsByType<PuzzleSlot>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("slot 1: "+slots[0].isSolved());
        Debug.Log("slot 1 need: "+slots[0].getNeedsFilled());
        Debug.Log("slot 1 is: "+slots[0].getIsFilled());
        Debug.Log("slot 2: "+slots[24].isSolved());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject newItem = Instantiate(block, canvas, false);

        RectTransform rt = newItem.GetComponent<RectTransform>();

        Vector2 localPoint;
        RectTransform canvasRect = canvas as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        rt.anchoredPosition = localPoint;
        rt.localScale = Vector3.one;

        newItem.transform.SetAsLastSibling();

        ExecuteEvents.Execute(newItem, eventData, ExecuteEvents.beginDragHandler);
        eventData.pointerDrag = newItem;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
    
}
