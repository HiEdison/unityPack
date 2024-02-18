using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoogiWorld.UI
{
    [RequireComponent(typeof(Toggle))]
    public class WoogiToggle : WoogiGameObject
    {
        Action<bool, GameObject> click;
        void OnDestroy()
        {
            click = null;
        }
        public void Awake()
        {
            toggle.onValueChanged.AddListener(OnClick);
        }

        private void OnClick(bool arg0)
        {
            if (click != null)
            {
                click(arg0, gameObject);
                //PlaySound.PlaySoundClickButton();
            }
        }

        public Toggle toggle
        {
            get { return GetComponent<Toggle>(); }
        }

        public bool isOn
        {
            get
            {
                if (toggle != null)
                    return toggle.isOn;
                return false;
            }
            set
            {
                if (toggle != null)
                    toggle.isOn = value;
            }
        }

        public void SetEvent(Action<bool, GameObject> callbackEvent)
        {
            click = callbackEvent;
        }
    }
}
