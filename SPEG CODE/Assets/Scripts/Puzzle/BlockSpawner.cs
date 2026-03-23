using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSpawner : MonoBehaviour, IPointerDownHandler ,IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private GameObject block;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Instantiate(block, transform);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
    
}
