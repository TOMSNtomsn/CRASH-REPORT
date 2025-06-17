using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class HoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HandleOnPointerEnter();
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandleOnPointerExit();
        isHovering = false;

    }

    private void Update()
    {
        if (isHovering)
        {
            HandleHovering();
        }
    }

    protected virtual void HandleOnPointerEnter() { }
    protected virtual void HandleOnPointerExit() { }
    protected virtual void HandleHovering() { }
}
