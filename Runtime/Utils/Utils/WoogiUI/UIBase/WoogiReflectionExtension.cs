
using System.Reflection;
/// summary
/// 反射扩展
/// /summary
public static class WoogiReflectionExtension
{
    public static object GetPrivateField(this object instance, string fieldName)
    {
        return instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
    }

    public static object GetPrivateProperty(this object instance, string propertyName)
    {
        return instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance, null);
    }

    public static void SetPrivateField(this object instance, string fieldName, object value)
    {
        instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
    }

    public static void SetPrivateProperty(this object instance, string propertyName, object value)
    {
        instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value, null);
    }

    public static object InvokePrivateMethod(this object instance, string methodName, params object[] param)
    {
        return instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(instance, param);
    }
}