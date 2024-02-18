using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WoogiWorld.UI
{
    public static class WoogiButtonEx
    {
        /// <summary>
        /// set button event
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="_ButtonEvent">remove event when callback method is null,or else add event</param>
        public static void AddMouseEvent(this GameObject t, MonoBehaviour recveiObj, UIEventType type, ButtonEvent _ButtonEvent)
        {
            if (t != null)
            {
                WoogiButton wb = t.GetComponent<WoogiButton>();
                if (wb != null)
                {
                    wb.SetEvent(recveiObj, type, _ButtonEvent);
                }
            }
        }

        public static void RemoveMouseEvent(this GameObject t, MonoBehaviour recveiObj, UIEventType type)
        {
            AddMouseEvent(t, recveiObj, UIEventType.mouseClick, null);
        }

        public static void Interactable(this GameObject t, bool value)
        {
            if (t != null)
            {
                Button wb = t.GetComponent<Button>();
                if (wb != null)
                {
                    wb.interactable = value;
                }
            }
        }
        
        public static bool Interactable(this GameObject t)
        {
            if (t != null)
            {
                Button wb = t.GetComponent<Button>();
                if (wb != null)
                {
                    return wb.interactable;
                }
            }

            return false;
        }
    }
}
