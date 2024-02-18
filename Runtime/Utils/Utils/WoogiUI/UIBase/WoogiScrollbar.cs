using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using WoogiWorld.UI;
using UnityEngine.EventSystems;

namespace WoogiWorld.UI
{
    [RequireComponent(typeof(Scrollbar))]
    public class WoogiScrollbar : WoogiImage
    {
        public Scrollbar scrollbar
        {
            get { return GetComponent<Scrollbar>(); }
        }

        private ScrollBarEvent onChange;

        void OnDestroy()
        {
            onChange = null;
        }

        void Awake()
        {
            scrollbar.onValueChanged.AddListener(OnChange);
        }

        /// <summary>
        /// set button event
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="_ButtonEvent">remove event when callback method is null,or else add event</param>
        public void SetEvent(UIEventType type, ScrollBarEvent _Event = null)
        {
            switch (type)
            {
                case UIEventType.change:
                    onChange = _Event;
                    break;
            }
        }

        void OnChange(float value)
        {
            if (onChange != null)
                onChange.Invoke(gameObject, value);
        }
    }
}
