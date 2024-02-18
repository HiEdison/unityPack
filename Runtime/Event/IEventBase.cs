
namespace WoogiWorld.Event
{
    public interface IEventBase
    {
        void Dispose();
        void DispatchEvent(object eventType, object data = null);
        bool AddEventListener(object eventType, object listener, EventFunction function);
        bool RemoveEventListener(object eventType, object listener);
        bool RemoveLuaEventListener(object eventType, object listener);
        void RemoveAllEventListener(object listener);
        void RemoveAllLuaEventListener(object listener);
    }
}