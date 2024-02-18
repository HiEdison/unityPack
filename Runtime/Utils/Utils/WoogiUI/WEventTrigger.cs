using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WEventTrigger : MonoBehaviour
{
    EventTrigger eventTrigger;

    private Dictionary<EventTriggerType, EventTrigger.TriggerEvent> dic =
        new Dictionary<EventTriggerType, EventTrigger.TriggerEvent>();

    void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = gameObject.AddComponent<EventTrigger>();
    }

    private void OnDestroy()
    {
        foreach (var item in dic)
        {
            item.Value.RemoveAllListeners();
        }

        dic.Clear();
        dic = null;
        eventTrigger.triggers.Clear();
    }

    EventTrigger.TriggerEvent AddTrigger(EventTriggerType triigerType)
    {
        var onEvt = new EventTrigger.Entry();
        onEvt.eventID = triigerType;
        onEvt.callback = new EventTrigger.TriggerEvent();
        eventTrigger.triggers.Add(onEvt);
        return onEvt.callback;
    }

    public void AddListener(EventTriggerType _type, UnityAction<BaseEventData> callback)
    {
        if (!dic.ContainsKey(_type))
            dic[_type] = AddTrigger(_type);
        if (dic.TryGetValue(_type, out var evt))
        {
            evt.AddListener(callback);
        }
    }

    public void RemoveListener(EventTriggerType _type, UnityAction<BaseEventData> callback)
    {
        if (dic.ContainsKey(_type) && callback != null)
        {
            if (dic.TryGetValue(_type, out var evt))
            {
                evt.RemoveListener(callback);
            }
        }
    }
}