﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WoogiWorld.UI
{
    public class LongPressOrClickEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
    {
        public float durationThreshold = 1.0f;

        public UnityEvent onLongPress = new UnityEvent();
        public UnityEvent onClick = new UnityEvent();

        private bool isPointerDown = false;
        private bool longPressTriggered = false;
        private float timePressStarted;

        private void Update()
        {
            if (isPointerDown && !longPressTriggered)
            {
                if (Time.time - timePressStarted > durationThreshold)
                {
                    longPressTriggered = true;
                    onLongPress.Invoke();
                }
            }
        }
        protected override void OnDestroy()
        {
            onLongPress = onClick = null;
            base.OnDestroy();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerDown = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!longPressTriggered)
            {
                onClick.Invoke();
            }
        }
    }
}