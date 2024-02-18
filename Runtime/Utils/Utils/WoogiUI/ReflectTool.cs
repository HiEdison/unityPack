using UnityEngine;
using System.Reflection;
using System;
using WoogiWorld.UI;

namespace WoogiWorld.UI
{
    public static class ReflectTool
    {
        public static T CallPrivateMethod<T>(this object instance, string name, params object[] param)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(name, flag);
            return (T)method.Invoke(instance, param);
        }
        public static void SetPrivateProperty(this object instance, string propertyname, object value)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo field = type.GetProperty(propertyname, flag);
            field.SetValue(instance, value, null);
        }
        public static void SetPrivateField(this object instance, string fieldname, object value)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo field = type.GetField(fieldname, flag);
            field.SetValue(instance, value);
        }
        public static T GetPrivateProperty<T>(this object instance, string propertyname)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo field = type.GetProperty(propertyname, flag);
            return (T)field.GetValue(instance, null);
        }
        public static T GetPrivateField<T>(this object instance, string fieldname)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo field = type.GetField(fieldname, flag);
            return (T)field.GetValue(instance);
        }
    }
}
#if UNITY_EDITOR
#region this is dome.
public class TestClass1
{
    public void t()
    {
        TestClass obj = new TestClass();
        WDebug.Log("Private field");
        WDebug.Log(obj.GetPrivateField<int>("privatefield1"));
        WDebug.Log(obj.GetPrivateField<int>("privatefield2"));
        WDebug.Log("Private Attributes");
        WDebug.Log(obj.GetPrivateProperty<string>("PrivateFieldA"));
        WDebug.Log(obj.GetPrivateProperty<string>("PrivateFieldB"));
        WDebug.Log("Private method");
        WDebug.Log(obj.CallPrivateMethod<int>("Add", null));
        WDebug.Log(obj.CallPrivateMethod<string>("Join", null));
        WDebug.Log("Modifying private attributes");
        obj.SetPrivateProperty("PrivateFieldA", "hello");
        obj.SetPrivateProperty("PrivateFieldB", "world");
        WDebug.Log(obj.CallPrivateMethod<string>("Join", null));

        WDebug.Log(obj.GetPrivateField<object>("state")); //重点.
        WDebug.Log(obj.GetPrivateField<int>("state")); //重点.

    }
}
public class TestClass
{
    public TestClass()
    {
        privatefield1 = 1; privatefield2 = 99;
        PrivateFieldA = "Lo"; PrivateFieldB = "ve";
        //state = SelectionState.Highlighted;
    }
    private int privatefield1;
    private int privatefield2;
    //private SelectionState state;
    private string PrivateFieldA { get; set; }
    private string PrivateFieldB { get; set; }
    private int Add() { return privatefield1 + privatefield2; }
    private string Join() { return PrivateFieldA + PrivateFieldB; }

    protected enum SelectionState
    {
        Normal = 0,
        Highlighted = 1,
        Pressed = 2,
        Disabled = 3
    }
}
#endregion
#endif

