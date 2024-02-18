using UnityEngine;
using System.Collections;
using WoogiWorld.Event;
using System;

public class EventDispatcher : EventBase
{
    private static EventDispatcher instance;
    public static EventDispatcher Instance
    {
        get
        {
            if (instance == null)
                instance = new EventDispatcher();
            return instance;
        }
    }
    public override void Dispose()
    {
        base.Dispose();
        instance = null;
    }
}
