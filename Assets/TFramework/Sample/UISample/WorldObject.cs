using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObject : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("WorldObject enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("WorldObject exit");
    }
}
