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
 *Description:         这个脚本适用于lua，请不要用泛型
 *History:
*/

public static class UIManagerLuaEx
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
    public static void AddMEvt(this UIManager t, string pName, string objName, MonoBehaviour recveiObj,
        UIEventType type,
        ButtonEvent _ButtonEvent)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).AddMouseEvent(recveiObj, type, _ButtonEvent);
        }
    }

    public static void RemoveMEvt(this UIManager t, string pName, string objName, MonoBehaviour recveiObj,
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
    public static void AddTogEvt(this UIManager t, string pName, string objName,
        Action<bool, GameObject> _event)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).AddToggleEvent(_event);
        }
    }

    /// <summary>
    /// Add Slider Event
    /// </summary>
    /// <param name="t"></param>
    /// <param name="objName">object name（plane.image）</param>
    /// <param name="_event"></param>
    public static void AddSliderEvt(this UIManager t, string pName, string objName,
        Action<float, GameObject> _event)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).AddSliderEvent(_event);
        }
    }

    public static void RemoveSliderEvt(this UIManager t, string pName, string objName,
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
    public static void SetActivePanel(this UIManager t, string pName, bool value)
    {
        WoogiPlane plane;
        if (t.GetTPlane(out plane, pName))
        {
            plane.gameObject.SetActive(value);
        }
    }

    public static bool ActiveSelfPanel(this UIManager t, string pName)
    {
        WoogiPlane plane;
        if (t.GetTPlane(out plane, pName))
        {
            return plane.gameObject.activeSelf;
        }

        return false;
    }

    #endregion

    #region UI

    public static void SetImage(this UIManager t, string pName, string objName, Sprite value)
    {
        if (t != null)
        {
            t.GetGameObject(pName, objName).SetImage(value);
        }
    }

    public static void Txt(this UIManager t, string pName, string objName, string value)
    {
        t.GetGameObject(pName, objName).SetText(value);
    }

    #endregion

    #region  interface, 目的是为了减少gc.

    public static void SetActive(this UIManager t, string pName, string objName, bool value)
    {
        WoogiPlane plane;
        if (t.GetTPlane(out plane, pName))
        {
            plane.SetActive(objName, value);
        }
    }

    public static bool ActiveSelf(this UIManager t, string pName, string objName)
    {
        WoogiPlane plane;
        if (t.GetTPlane(out plane, pName))
        {
            return plane.ActiveSelf(objName);
        }

        return false;
    }

    public static GameObject GetGameObject(this UIManager t, string pName, string objName)
    {
        WoogiPlane plane;
        if (t.GetTPlane(out plane, pName))
        {
            return plane.GetGameObject(objName);
        }

        return null;
    }

    public static RectTransform GetRectTransform(this UIManager t, string pName, string objName)
    {
        WoogiPlane plane;
        if (t.GetTPlane(out plane, pName))
        {
            return plane.GetRectTransform(objName);
        }

        return null;
    }

    public static Vector3 LocalPosition(this UIManager t, string pName, string objName, Vector3 defaultV)
    {
        RectTransform tf = GetRectTransform(t, pName, objName);
        if (tf != null)
            return tf.localPosition;
        return defaultV;
    }
    
    public static Vector3 Position(this UIManager t, string pName, string objName, Vector3 defaultV)
    {
        RectTransform tf = GetRectTransform(t, pName, objName);
        if (tf != null)
            return tf.position;
        return defaultV;
    }
    #endregion
}