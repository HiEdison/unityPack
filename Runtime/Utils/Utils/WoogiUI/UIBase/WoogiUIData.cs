using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace WoogiWorld.UI
{
    public delegate void ButtonEvent(GameObject obj);
    public delegate void ScrollBarEvent(GameObject obj, float value);
    public enum UIEventType
    {
        mouseDown,
        mouseUp,
        mouseEnter,
        mouseExit,
        mouseClick,
        mousePress,
        mouseLongPress,
        mouseStartDrag,
        mouseDrag,
        mouseEndDrag,
        mouseMove, // 鼠标移动（主要用于做提示使用）.
        change,
    };
}
