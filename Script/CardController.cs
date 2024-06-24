using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Card card;
    public BoxCollider2D thisCard;
    public bool isMouseOver;

    private void OnMouseOver()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        isMouseOver = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        isMouseOver = false;
    }
}