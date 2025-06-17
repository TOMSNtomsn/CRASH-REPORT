using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverScaler : HoverDetector
{
    [SerializeField] float hoverScale = 1.2f;
    [SerializeField] float hoverSpeed = 1.2f;
    [SerializeField] Sprite notCleared;
    [SerializeField] Sprite cleared;

    float hoverStartTime;
    float startingScale;

    bool canHover = true;

    public void SetCanHover(bool canHover)
    {
        this.canHover = canHover;
        if (!canHover)
        {
            transform.localScale = Vector3.one * startingScale;
        }
    }

    void Start()
    {
        startingScale = transform.localScale.x;
    }

    protected override void HandleOnPointerEnter()
    {
        if (!canHover) return;
        transform.localScale = Vector3.one * startingScale;
        hoverStartTime = Time.time;
    }

    protected override void HandleHovering()
    {
        if (!canHover) return;
        float timeSinceHoverStart = Time.time - hoverStartTime;
        float t = Mathf.PingPong(timeSinceHoverStart * hoverSpeed, 1);
        float scale = Mathf.Lerp(startingScale, startingScale * hoverScale, t);
        transform.localScale = Vector3.one * scale;
    }

    protected override void HandleOnPointerExit()
    {
        if (!canHover) return;
        transform.localScale = Vector3.one * startingScale;
    }

    public void SetSprite(bool clear)
    {
        GetComponent<Image>().sprite = clear ? cleared : notCleared;
    }
}
