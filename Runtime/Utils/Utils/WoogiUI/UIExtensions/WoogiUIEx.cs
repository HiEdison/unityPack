using UnityEngine;
using System;
using UnityEngine.UI;

namespace WoogiWorld.UI
{
    public static class UIEx
    {
        public static void SetImage(this GameObject t, Sprite value)
        {
            if (t != null)
            {
                Image img = t.GetComponent<Image>();
                if (img != null)
                    img.sprite = value;
                else
                    Log(t.name, "SetImage");
            }
        }

        public static void SetText(this GameObject t, string value)
        {
            if (t != null)
            {
                Text tx = t.GetComponent<Text>();
                if (tx != null)
                {
                    tx.text = value;
                    tx.gameObject.SetActive(!String.IsNullOrEmpty(value));
                }

                else
                    Log(t.name, "SetText");
            }
        }

        public static string GetText(this GameObject t)
        {
            if (t != null)
            {
                Text tx = t.GetComponent<Text>();
                if (tx != null)
                    return tx.text;
                else
                    Log(t.name, "SetText");
            }

            return "";
        }

        #region slider

        public static void SetSliderValue(this GameObject t, float value)
        {
            if (t != null)
            {
                Slider s = t.GetComponent<Slider>();
                if (s != null)
                    s.value = value;
                else
                    Log(t.name, "SetSliderValue");
            }
        }

        public static void SetSliderMinAndMaxValue(this GameObject t, float minvalue, float maxvalue)
        {
            if (t != null)
            {
                Slider s = t.GetComponent<Slider>();
                if (s != null)
                {
                    s.minValue = minvalue;
                    s.maxValue = maxvalue;
                }
                else
                    Log(t.name, "SetSliderValue");
            }
        }

        public static bool TryGetSliderValue(this GameObject t, out float value)
        {
            value = 0;
            if (t != null)
            {
                Slider s = t.GetComponent<Slider>();
                if (s != null)
                {
                    value = s.value;
                    return true;
                }
                else
                    Log(t.name, "TryGetSliderValue");
            }

            return false;
        }


        public static void AddSliderEvent(this GameObject t, Action<float, GameObject> _event)
        {
            if (t != null)
            {
                WoogiSlider ws = t.GetComponent<WoogiSlider>();
                if (ws != null)
                    ws.SetEvent(_event);
                else
                    Log(t.name, "SetSliderEvent");
            }
        }

        public static void RemoveSliderEvent(this GameObject t, Action<float, GameObject> _event)
        {
            if (t != null)
            {
                WoogiSlider ws = t.GetComponent<WoogiSlider>();
                if (ws != null)
                    ws.RemoveEvent(_event);
                else
                    Log(t.name, "SetSliderEvent");
            }
        }

        #endregion

        #region Scrollbar

        public static void SetScrollbarValue(this GameObject t, float value)
        {
            if (t != null)
            {
                Scrollbar s = t.GetComponent<Scrollbar>();
                if (s != null)
                    s.value = value;
                else
                    Log(t.name, "SetSliderValue");
            }
        }

        public static void AddScrollbarEvent(this GameObject t, ScrollBarEvent _event)
        {
            if (t != null)
            {
                WoogiScrollbar ws = t.GetComponent<WoogiScrollbar>();
                if (ws != null)
                    ws.SetEvent(UIEventType.change, _event);
                else
                    Log(t.name, "AddSliderbarEvent");
            }
        }

        public static void RemoveScrollbarEvent(this GameObject t)
        {
            if (t != null)
            {
                WoogiScrollbar ws = t.GetComponent<WoogiScrollbar>();
                if (ws != null)
                    ws.SetEvent(UIEventType.change, null);
                else
                    Log(t.name, "RemoveSliderbarEvent");
            }
        }

        public static bool TryGetScrollbarValue(this GameObject t, out float value)
        {
            value = 0;
            if (t != null)
            {
                Scrollbar s = t.GetComponent<Scrollbar>();
                if (s != null)
                {
                    value = s.value;
                    return true;
                }
                else
                    Log(t.name, "TryGetSliderValue");
            }

            return false;
        }

        #endregion


        #region toggle.

        public static void AddToggleEvent(this GameObject t, Action<bool, GameObject> _event)
        {
            if (t != null)
            {
                WoogiToggle wt = t.GetComponent<WoogiToggle>();
                if (wt != null)
                    wt.SetEvent(_event);
                else
                    Log(t.name, "SetToggleEvent");
            }
        }

        public static bool TryGetIsOn(this GameObject t, out bool value)
        {
            value = false;
            if (t != null)
            {
                Toggle to = t.GetComponent<Toggle>();
                if (to != null)
                {
                    value = to.isOn;
                    return true;
                }
                else
                    Log(t.name, "TryGetIsOn");
            }

            return false;
        }

        public static void SetIsOn(this GameObject t, bool value)
        {
            if (t != null)
            {
                Toggle to = t.GetComponent<Toggle>();
                if (to != null)
                {
                    to.isOn = value;
                    if (to.graphic != null)
                    {
                        GameObject obj = to.graphic.gameObject;
                        if (obj != null && obj.activeSelf == false)
                            obj.SetActive(true);
                    }
                }
                else
                    Log(t.name, "SetIsOn");
            }
        }

        #endregion

        private static void Log(string goName, string funName)
        {
            WDebug.LogError(string.Format("{0} get UI component fail in '[WoogiWorld.UIEx.{1}]',please check the issue!",
                goName, funName));
        }
    }
}