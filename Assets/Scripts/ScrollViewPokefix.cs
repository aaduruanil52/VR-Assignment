using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewPokefix : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private ScrollRect scrollRect;
    private bool isPoking = false;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPoking = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPoking = false;
        scrollRect.enabled = true;
    }
}
