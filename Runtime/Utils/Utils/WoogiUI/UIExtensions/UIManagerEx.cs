using System;
using UnityEngine;
using WoogiWorld.UI;

/** Copyright(C) 2020 by 	WoogiWorld
 *All rights reserved.
 *ProductName:  		WoogiCreate
 *FileName:     		UIManagerEx.cs
 *Author:       		None
 *Version:      		2.6.7
 *UnityVersion：		2018.4.21f1
 *Date:         		2020-07-07
 *Description:          No description
 *History:
*/
public static class UIManagerEx
{
    #region Event

    /// <summary>
    /// add mouse event
    /// </summary>
    /// <param name="t">uimanager</param>
    /// <param name="objName">object name（plane.image）</param>
    /// <param name="recveiObj">current mono</param>
    /// <param name="type">event type</param>
    /// <param name="_ButtonEvent">callback</param>
    public static void AddMEvt<TPlane>(this UIManager t, TPlane pName, string objName, MonoBehaviour recveiObj,
        UIEventType type,
        ButtonEvent _ButtonEvent)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).AddMouseEvent(recveiObj, type, _ButtonEvent);
        }
    }

    public static void RemoveMEvt<TPlane>(this UIManager t, TPlane pName, string objName, MonoBehaviour recveiObj,
        UIEventType type)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).RemoveMouseEvent(recveiObj, type);
        }
    }

    /// <summary>
    /// Add Toggle Event
    /// </summary>
    /// <param name="t"></param>
    /// <param name="objName">object name（plane.image）</param>
    /// <param name="_event"></param>
    public static void AddTogEvt<TPlane>(this UIManager t, TPlane pName, string objName,
        Action<bool, GameObject> _event)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).AddToggleEvent(_event);
        }
    }

    
    public static void LoadAsyncPlane(this UIManager t, string path, Action<GameObject> callback,
        string parentCanvas = "CentreLayer")
    {
        Transform parent = t.GetCanvas<Transform>(parentCanvas);
        t.LoadAsyncPlane(path, parent, callback);
    }
    /// <summary>
    /// Add Slider Event
    /// </summary>
    /// <param name="t"></param>
    /// <param name="objName">object name（plane.image）</param>
    /// <param name="_event"></param>
    public static void AddSliderEvt<TPlane>(this UIManager t, TPlane pName, string objName,
        Action<float, GameObject> _event)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).AddSliderEvent(_event);
        }
    }

    public static void RemoveSliderEvt<TPlane>(this UIManager t, TPlane pName, string objName,
        Action<float, GameObject> _event)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).RemoveSliderEvent(_event);
        }
    }

    #endregion

    #region 设置plane激活状态

    /// <summary>
    /// set panle active 
    /// </summary>
    /// <param name="planeName">plane name</param>
    /// <param name="value">bool value</param>
    public static void SetActivePanel<TPlane>(this UIManager t, TPlane pName, bool value)
    {
        WoogiPlane plane;
        if (t.GetTPlane<TPlane>(out plane, pName))
        {
            plane.gameObject.SetActive(value);
        }
    }

    public static bool ActiveSelfPanel<TPlane>(this UIManager t, TPlane pName)
    {
        WoogiPlane plane;
        if (t.GetTPlane<TPlane>(out plane, pName))
        {
            return plane.gameObject.activeSelf;
        }

        return false;
    }

    #endregion

    #region UI

    public static void SetImage<TPlane>(this UIManager t, TPlane pName, string objName, Sprite value)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).SetImage(value);
        }
    }

    public static void Txt<TPlane>(this UIManager t, TPlane pName, string objName, string value)
    {
        t.GetGameObject(pName, objName).SetText(value);
    }

    #endregion

    #region interface, 目的是为了减少gc.

    public static void SetActive<TPlane>(this UIManager t, TPlane pName, string objName, bool value)
    {
        WoogiPlane plane;
        if (t.GetTPlane<TPlane>(out plane, pName))
        {
            plane.SetActive(objName, value);
        }
    }

    public static bool ActiveSelf<TPlane>(this UIManager t, TPlane pName, string objName)
    {
        WoogiPlane plane;
        if (t.GetTPlane<TPlane>(out plane, pName))
        {
            return plane.ActiveSelf(objName);
        }

        return false;
    }

    public static GameObject GetGameObject<TPlane>(this UIManager t, TPlane pName, string objName)
    {
        WoogiPlane plane;
        if (t.GetTPlane<TPlane>(out plane, pName))
        {
            return plane.GetGameObject(objName);
        }

        return null;
    }

    public static RectTransform GetRectTransform<TPlane>(this UIManager t, TPlane pName, string objName)
    {
        WoogiPlane plane;
        if (t.GetTPlane<TPlane>(out plane, pName))
        {
            return plane.GetRectTransform(objName);
        }

        return null;
    }

    public static Transform GetTransform<TPlane>(this UIManager t, TPlane pName, string objName)
    {
        WoogiPlane plane;
        if (t.GetTPlane<TPlane>(out plane, pName))
        {
            return plane.GetTransform(objName);
        }

        return null;
    }

    public static Vector3 LocalPosition<TPlane>(this UIManager t, TPlane pName, string objName, Vector3 defaultV)
    {
        RectTransform tf = GetRectTransform(t, pName, objName);
        if (tf != null)
            return tf.localPosition;
        return defaultV;
    }

    public static Vector3 Position<TPlane>(this UIManager t, TPlane pName, string objName, Vector3 defaultV)
    {
        RectTransform tf = GetRectTransform(t, pName, objName);
        if (tf != null)
            return tf.position;
        return defaultV;
    }

    #endregion
}