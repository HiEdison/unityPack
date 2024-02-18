using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class WoogiButtonScale :MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    public float PointDownSize = 0.9f;

    void IPointerDownHandler.OnPointerDown( PointerEventData eventData )
    {
        transform.localScale = Vector3.one * PointDownSize;
    }

    void IPointerUpHandler.OnPointerUp( PointerEventData eventData )
    {
        transform.localScale = Vector3.one;
    }
}
