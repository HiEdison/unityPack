using System;

namespace WoogiWorld.Event
{
    public delegate void EventFunction(EventMessage info);

    public class EventListener
    {
        public object listener;
        public EventFunction function;

        ~EventListener()
        {
            function = null;
        }

        public void Dispose()
        {
            function = null;
            listener = null;
        }
    }

    public class EventMessage
    {
        private EventListener target;
        private string eventType;
        private object _data;

        ~EventMessage()
        {
            _data = null;
            target = null;
        }

        public void Dispose()
        {
            _data = null;
            target = null;
        }

        public EventMessage(EventListener target, string type, object data)
        {
            this.target = target;
            this.eventType = type;
            this._data = data;
        }

        public bool CheckData()
        {
            return _data != null;
        }

        public object Data
        {
            get { return _data; }
        }

        public T ReadData<T>(T w_t = default(T))
        {
            return Data != null && Data is T ? (T)Data : w_t;
        }

        public T ReadDataToJson<T>(T w_t = default(T))
        {
            if (Data != null && Data is string)
            {
                return Json.DeserializeJson<T>(Data as string);
            }

            return w_t;
        }

        public EventListener Target
        {
            get { return target; }
        }

        public string EventType
        {
            get { return eventType; }
        }

        // 传多参 ////////////////////////////////////////////////////////////////////////////////////////////////////////
        private object[] _newdata;

        public EventMessage(string type, EventListener target, object[] data)
        {
            this.target = target;
            this.eventType = type;
            this._newdata = data;
        }

        public bool CheckNewData()
        {
            return _newdata != null && _newdata.Length > 0;
        }

        public object[] NewData
        {
            get { return _newdata; }
        }

        public int Length => _newdata != null ? _newdata.Length : 0;

        public T ReadNewData<T>(int _idx = 0, T w_t = default(T))
        {
            if (_idx >= 0 && _idx < Length)
            {
                return (T)Convert.ChangeType(_newdata[_idx], typeof(T));
            }

            return w_t;
        }
    }
}