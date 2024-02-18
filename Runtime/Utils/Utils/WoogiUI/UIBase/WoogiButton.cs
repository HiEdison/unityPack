using UnityEngine;
using System.Collections;
using WoogiWorld.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
//using LuaFramework;

namespace WoogiWorld.UI
{
    [RequireComponent(typeof(Button))]
    //public class WoogiButton : WoogiImage, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
    public class WoogiButton : /*WoogiImage*/ WoogiGameObject, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {  //取消继承woogiImage 是为了更灵活的使用button，比如说使用无像素的button.
        public MonoBehaviour mReceiveObj;
        public string mFunction;
        public string mButtonPath;
        public bool isPlayClickSound = true;
        internal ButtonEvent onMouseDown;
        internal ButtonEvent onMouseUp;
        internal ButtonEvent onMouseEnter;
        internal ButtonEvent onMouseExit;
        internal ButtonEvent onMousePress;
        internal ButtonEvent onMouseLongPress;
        internal ButtonEvent onClick;
        private bool isPress = false;
        private bool isLongPress = false;
        private float pressDurationTime;
        private float pressIntervalTime = 1.0f;
        private Vector3 pressVe3;

        public Button button
        {
            get
            {
                Button b = GetComponent<Button>();
                if (b == null)
                {
                    b = gameObject.AddComponent<Button>();
                }
                return b;
            }
        }
        void OnDestroy()
        {
            mReceiveObj = null;
            onMouseLongPress = onMouseUp = onMousePress = onMouseExit = onMouseEnter = onMouseDown = onClick = null;
        }

        public void Awake()
        {
            button.onClick.AddListener(OnClick);
            mButtonPath = WoogiTools.GetObjectPath(gameObject);
        }

        void OnClick()
        {
            if (isLongPress)
            {
                return;
            }
            //if (LuaFramework.Util.luaBtnFuntion.ContainsKey(this.name))
            //{
            //    LuaFramework.Util.DoLuaString(LuaFramework.Util.luaBtnFuntion[this.name]);
            //    return;
            //}
            if (onClick != null)
            {
                onClick(gameObject);
            }
           // FunctionPopupProcess.CheckButtonPath(mButtonPath);
            if (isPlayClickSound)
            {
                try
                {
//                    WDebug.Log("close engine 76");
                  //  PlaySound.PlaySoundClickButton();
                }
                catch (Exception e)
                {
                    WDebug.Log(e.Message);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isLongPress)
            {
                return;
            }
            if (onClick != null)
            {
                onClick(gameObject);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onMouseDown != null)
                onMouseDown(gameObject);
            isPress = true;
            pressDurationTime = 0.0f;
            isLongPress = false;
            pressVe3 = Input.mousePosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (onMouseUp != null)
                onMouseUp(gameObject);
            isPress = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (onMouseExit != null)
                onMouseExit(gameObject);
            isPress = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (onMouseEnter != null)
                onMouseEnter(gameObject);
        }

        /// <summary>
        /// set button event
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="_ButtonEvent">remove event when callback method is null,or else add event</param>
        internal virtual void SetEvent(MonoBehaviour recveiObj, UIEventType type, ButtonEvent _ButtonEvent = null)
        {
            if (_ButtonEvent != null)
                mFunction = _ButtonEvent.Method.Name;
            else
                mFunction = "";
            mReceiveObj = recveiObj;

            switch (type)
            {
                case UIEventType.mouseUp: onMouseUp = _ButtonEvent; break;
                case UIEventType.mouseDown: onMouseDown = _ButtonEvent; break;
                case UIEventType.mouseEnter: onMouseEnter = _ButtonEvent; break;
                case UIEventType.mouseExit: onMouseExit = _ButtonEvent; break;
                case UIEventType.mouseClick: onClick = _ButtonEvent; break;
                case UIEventType.mousePress: onMousePress = _ButtonEvent; break;
                case UIEventType.mouseLongPress: onMouseLongPress = _ButtonEvent; break;
            }
        }

        public bool interactable
        {
            get
            {
                if (button != null)
                {
                    return button.interactable;
                }
                return false;
            }
            set
            {
                if (button != null)
                {
                    if (button.interactable != value)
                        button.interactable = value;
                }
            }
        }

        void Update()
        {
            if (isPress)
            {
                onMousePress?.Invoke(gameObject);
                if (!isLongPress && onMouseLongPress != null)
                {
                    if (pressVe3 == Input.mousePosition)
                    {
                        pressDurationTime += Time.deltaTime;
                        if (pressDurationTime >= pressIntervalTime)
                        {
                            onMouseLongPress?.Invoke(gameObject);
                            isLongPress = true;
                        }
                    }
                    else
                    {
                        isLongPress = true;
                    }
                }
            }
        }
    }
}
