using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using WoogiWorld.UI;
using System;

namespace WoogiWorld.UI
{
    [RequireComponent(typeof(Slider))]
    public class WoogiSlider : WoogiGameObject
    {
        private Action<float, GameObject> onChange;
        public Slider slider
        {
            get { return GetComponent<Slider>(); }
        }
        void OnDestroy()
        {
            onChange = null;
        }
        public float value
        {
            get
            {
                if (slider != null)
                {
                    return slider.value;
                }
                return 0;
            }
            set
            {
                if (slider != null)
                    slider.value = value;
            }
        }

        public void SetEvent(Action<float, GameObject> _event)
        {
            onChange = _event;
            slider.onValueChanged.AddListener(OnChange);
        }
        
        public void RemoveEvent(Action<float, GameObject> _event)
        {
            onChange = _event;
            slider.onValueChanged.RemoveListener(OnChange);
        }

        private void OnChange(float value)
        {
            if (onChange != null)
            {
                onChange(value, gameObject);
            }
        }
    }
}
