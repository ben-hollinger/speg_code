using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteZone : MonoBehaviour, IDropHandler
{


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Destroy(eventData.pointerDrag);
        }
    }
}
