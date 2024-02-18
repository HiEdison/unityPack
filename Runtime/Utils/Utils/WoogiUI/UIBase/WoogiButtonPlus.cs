using UnityEngine;
using System.Collections;
using WoogiWorld.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace WoogiWorld.UI
{
    [RequireComponent(typeof(Button))]
    public class WoogiButtonPlus : WoogiButton, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        internal ButtonEvent onStartDrag;
        internal ButtonEvent onDrag;
        internal ButtonEvent onEndDrag;
        void OnDestroy()
        {
            onEndDrag = onDrag = onStartDrag = null;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onStartDrag != null)
                onStartDrag(gameObject);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null)
                onDrag(gameObject);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDrag != null)
                onEndDrag(gameObject);
        }

        internal override void SetEvent(MonoBehaviour recveiObj, UIEventType type, ButtonEvent _ButtonEvent = null)
        {
            base.SetEvent(recveiObj, type, _ButtonEvent);
            switch (type)
            {
                case UIEventType.mouseStartDrag: onStartDrag = _ButtonEvent; break;
                case UIEventType.mouseDrag: onDrag = _ButtonEvent; break;
                case UIEventType.mouseEndDrag: onEndDrag = _ButtonEvent; break;
            }
        }
    }
}
