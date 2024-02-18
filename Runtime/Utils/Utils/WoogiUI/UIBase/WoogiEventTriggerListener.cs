using UnityEngine;
using UnityEngine.EventSystems;

namespace WoogiWorld.UI
{
    public class WoogiEventTriggerListener : EventTrigger
    {
        public delegate void VoidDelegate(object data = null);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;

        void OnDestroy()
        {
            onSelect = onUp = onExit = onEnter = onDown = onClick = null;
        }
        static public WoogiEventTriggerListener Get(GameObject go)
        {
            WoogiEventTriggerListener listener = go.GetComponent<WoogiEventTriggerListener>();
            if (listener == null) listener = go.AddComponent<WoogiEventTriggerListener>();
            return listener;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null) onClick(eventData);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null) onDown(eventData);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null) onEnter(eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null) onExit(eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null) onUp(eventData);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(eventData);
        }
    }
}
