using System;
using UnityEngine;
using UnityEngine.UI;
namespace WoogiWorld.UI
{
    public static class SystemUIEx
    {
        #region image
        public static void Set(this Image t, Sprite value)
        {
            if (t != null)
            {
                t.sprite = value;
            }
        }

        public static void Set(this Image t, Color value)
        {
            if (t != null)
            {
                if (t.color != value)
                    t.color = value;
            }
        }
        #endregion

        #region text
        public static Text Set(this Text t, Color value)
        {
            if (t != null)
            {
                if (t.color != value)
                    t.color = value;
                return t;
            }
            return null;
        }

        public static void Set(this Text t, string value)
        {
            if (t != null)
            {
                if (value != null)
                {
                    if (t.text != value)
                    {
                        t.text = value;
                    }
                }
                else
                {
                    WDebug.Log("Content data does not exist.Check the localConfig configuration table.Add corresponding data");
                }
            }
        }

        public static bool TryGet(this Text t, out string value)
        {
            value = "";
            if (t != null)
            {
                value = t.text;
                return true;
            }
            return false;
        }
        #endregion

        #region slider
        public static void Set(this Slider t, float value)
        {
            if (t != null)
            {
                t.value = value;
            }
        }

        public static bool TryGet(this Slider t, out float value)
        {
            value = 0;
            if (t != null)
            {
                value = t.value;
                return true;
            }
            return false;
        }
        #endregion

        #region toggle
        public static void Set(this Toggle t, bool value)
        {
            if (t != null)
            {
                t.isOn = value;
            }
        }
        public static bool TryGet(this Toggle t, out bool value)
        {
            value = false;
            if (t != null)
            {
                value = t.isOn;
                return true;
            }
            return false;
        }
        #endregion
    }
}
