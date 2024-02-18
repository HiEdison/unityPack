using UnityEngine;

namespace WoogiWorld.Event
{
    /// <summary>
    /// this is custom mono event system
    /// </summary>
    public class MonoEventBase : MonoBehaviour, IEventBase
    {
        private EventBase eventBase = new EventBase();
        public virtual void Dispose()
        {
            eventBase.Dispose();
        }
        
        protected virtual void OnDestroy()
        {
            if (eventBase != null) eventBase.Dispose();
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="data"></param>
        //public void DispatchEvent(string eventType, object data = null)
        //{
        //    eventBase.DispatchEvent(eventType, data);
        //}

        public void DispatchEvent(object eventType, object data = null)
        {
            eventBase.DispatchEvent(eventType, data);
        }
        public void DispatchEvent(object eventType, params object[] data)
        {
            eventBase.DispatchNewEvent(eventType, data);
        }
        /// <summary>
        /// 添加事件监听
        /// (如果不使用了一定要记得remove掉，不然后果自负).
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="listener">监听对象</param>
        /// <param name="function">回调方法</param>
        /// <returns></returns>
        //public bool AddEventListener(string eventType, object listener, EventFunction function)
        //{
        //    return eventBase.AddEventListener(eventType, listener, function);
        //}
        public bool AddEventListener(object eventType, object listener, EventFunction function)
        {
            return eventBase.AddEventListener(eventType, listener, function);
        }

        /// <summary>
        /// 移除某个对象的事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        //public bool RemoveEventListener(string eventType, object listener)
        //{
        //    return eventBase.RemoveEventListener(eventType, listener);
        //}

        public bool RemoveEventListener(object eventType, object listener)
        {
            return eventBase.RemoveEventListener(eventType, listener);
        }
        public bool RemoveLuaEventListener(object eventType, object listener)
        {
            return eventBase.RemoveLuaEventListener(eventType, listener);
        }
        /// <summary>
        /// 移除同一监听者的所有事件
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveAllEventListener(object listener)
        {
            eventBase.RemoveAllEventListener(listener);
        }
        
        public void RemoveAllLuaEventListener(object listener)
        {
            eventBase.RemoveAllLuaEventListener(listener);
        }
    }
}
