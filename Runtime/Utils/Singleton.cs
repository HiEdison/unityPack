using System;
using System.Threading;

/// <summary>
/// 单例基类(线程安全)
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : new()
{
    /// <summary>
    /// 单例对象
    /// </summary>
    private static T m_Instance=default(T);
    private static object s_objectLock = new object();
    public static T Instance
    {
        get
        {
            object obj;
            //加锁防止多线程创建单例
            Monitor.Enter(obj =s_objectLock);
            try
            {
                if (m_Instance == null)
                {
                    m_Instance = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));//创建单例的实例
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
            return m_Instance;
        }
    }
    protected Singleton(){ }
    public static bool hasInstance { get {return m_Instance != null; } }
    public virtual void Dispose()
    {
        m_Instance = default(T);
    }
}
