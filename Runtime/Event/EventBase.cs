using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WoogiWorld.Event
{
    /// <summary>
    /// this is custom c# event system
    /// </summary>
    public class EventBase : IEventBase
    {
        private static object obj = new object();
        private Dictionary<string, List<EventListener>> eventListenerDic;

        public EventBase()
        {
            eventListenerDic = new Dictionary<string, List<EventListener>>();
        }

        public virtual void Dispose()
        {
            if (eventListenerDic != null)
            {
                eventListenerDic.Clear();
            }
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="data"></param>
        public void DispatchNewEvent(object eventType, params object[] data)
        {
            string eType = null;
            if (GetEventProtocol(out eType, eventType))
            {
                if (!CheckIsHaveEvent(eType))
                {
                    return;
                }
                List<EventListener> targetList = eventListenerDic[eType];
                for (int i = 0; i < targetList.Count; i++)
                {
                    if (targetList[i] != null && targetList[i].listener != null) //避免多线程.
                    {
                        EventMessage info = new EventMessage(eType, targetList[i], data);
                        if (targetList[i].function != null)
                        {
                            targetList[i].function(info);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="data"></param>
        public void DispatchEvent(object eventType, object data = null)
        {
            string eType = null;
            if (GetEventProtocol(out eType, eventType))
            {
                if (!CheckIsHaveEvent(eType))
                {
                    return;
                }

                List<EventListener> targetList = new List<EventListener>(eventListenerDic[eType]);
                foreach (EventListener target in targetList)
                {
                    if (target != null && target.listener != null) //避免多线程.
                    {
                        EventMessage info = new EventMessage(target, eType, data);
                        try
                        {

                            if (target.function != null &&
                                target.function.Target != null &&
                                target.function.Target.ToString() != "null")
                            {
                                target.function(info);
                            }
                        }
                        catch (Exception e)
                        {
                            WDebug.LogError($"{info.EventType}:{e}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加事件监听
        /// (如果不使用了一定要记得remove掉，不然后果自负).
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="listener">监听对象</param>
        /// <param name="function">回调方法</param>
        /// <returns></returns>
        public bool AddEventListener(object eventType, object listener, EventFunction function)
        {
            string eType = null;
            if (GetEventProtocol(out eType, eventType))
            {
                lock (obj)
                {
                    if (eventType == null || function == null)
                        return false;
                    AddEvent(eType);
                    return AddListener(eType, listener, function);
                }
            }

            return false;
        }

        /// <summary>
        /// 移除某个对象的事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool RemoveEventListener(object eventType, object listener)
        {
            string eType = null;
            if (GetEventProtocol(out eType, eventType))
            {
                if (listener == null)
                {
                    WDebug.LogWarning("RemoveEventListener==> (eventtype=" + eventType + ")listener is null");
                    return false;
                }

                lock (obj)
                {
                    if (!CheckIsHaveEvent(eType))
                        return false;
                    List<EventListener> targetList = eventListenerDic[eType];
                    foreach (EventListener target in targetList)
                    {
                        if (target.listener == listener)
                        {
                            target.Dispose();
                            targetList.Remove(target);
                            if (targetList.Count == 0)
                            {
                                eventListenerDic.Remove(eType);
                            }

                            return true;
                        }
                    }

                    return false;
                }
            }

            return false;
        }

        public bool RemoveLuaEventListener(object eventType, object listener)
        {
            string eType = null;
            if (GetEventProtocol(out eType, eventType))
            {
                if (listener == null)
                {
                    WDebug.LogWarning("RemoveEventListener==> (eventtype=" + eventType + ")listener is null");
                    return false;
                }

                lock (obj)
                {
                    if (!CheckIsHaveEvent(eType))
                        return false;
                    List<EventListener> targetList = eventListenerDic[eType];
                    foreach (EventListener target in targetList)
                    {
                        if (target.listener == listener)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除同一监听者的所有事件
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveAllEventListener(object listener)
        {
            if (listener == null) return;
            lock (obj)
            {
                List<string> eventTypes = new List<string>(eventListenerDic.Keys);
                foreach (string eventType in eventTypes)
                {
                    var target = from eventListener in eventListenerDic[eventType]
                                 where eventListener.listener == listener
                                 select eventListener;

                    if (target.Count() > 0)
                    {
                        RemoveEventListener(eventType, listener);
                    }
                }
            }
        }

        public void RemoveAllLuaEventListener(object listener)
        {
            if (listener == null) return;
            lock (obj)
            {
                List<string> eventTypes = new List<string>(eventListenerDic.Keys);
                foreach (string eventType in eventTypes)
                {
                    var target = from eventListener in eventListenerDic[eventType]
                                 where eventListener.listener == listener
                                 select eventListener;

                    if (target.Count() > 0)
                    {
                        RemoveLuaEventListener(eventType, listener);
                    }
                }
            }
        }

        /// <summary>
        /// 添加事件类型
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private void AddEvent(string eventType)
        {
            if (!CheckIsHaveEvent(eventType))
            {
                eventListenerDic.Add(eventType, new List<EventListener>());
            }
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        private bool AddListener(string eventType, object listener, EventFunction function)
        {
            if (CheckForListener(eventType, listener, out var _target))
            {
                _target.function = function;
                return true;
            }
            else
            {
                EventListener target = new EventListener();
                target.listener = listener;
                target.function = function;
                eventListenerDic[eventType].Add(target);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测同一个对象 同一种事件类型
        /// </summary>
        /// <param name="eventType">监听类型</param>
        /// <param name="listener">监听者</param>
        /// <returns></returns>
        private bool CheckForListener(string eventType, object listener, out EventListener _target)
        {
            foreach (EventListener target in eventListenerDic[eventType])
            {
                if (target.listener == listener)
                {
                    _target = target;
                    return true;
                }
            }

            _target = null;
            return false;
        }

        /// <summary>
        /// 检测事件是否存在
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns></returns>
        private bool CheckIsHaveEvent(string eventType)
        {
            if (eventListenerDic.ContainsKey(eventType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查是否存事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool CheckIsHaveEvent<T>(string eventType)
        {
            if (eventListenerDic.ContainsKey(eventType) && eventListenerDic[eventType].Exists(m => m.listener.GetType().Equals(typeof(T))))
            {
                return true;
            }

            return false;
        }

        private bool GetEventProtocol(out string eType, object eventType)
        {
            eType = "";
            if (eventType != null)
            {
                Type t = eventType.GetType();
                if (t == typeof(string))
                {
                    eType = (string)eventType;
                    return true;
                }

                eType = t + "." + eventType;
                return true;
            }

            return false;
        }
        public bool isEventProtocol(object w_eventType, object w_listener)
        {
            if (eventListenerDic != null && eventListenerDic.Count > 0)
            {
                string eType = null;
                if (GetEventProtocol(out eType, w_eventType))
                {
                    List<EventListener> w_ls;
                    if (eventListenerDic.TryGetValue(eType, out w_ls) && w_ls != null && w_ls.Count > 0)
                    {
                        foreach (EventListener w_el in w_ls)
                        {
                            if (w_el == w_listener)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}