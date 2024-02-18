using UnityEngine;
using System;
using System.Collections;

public class SingletonBehaviour<T> : MonoBehaviour where T : Component
{
    public static string SingletonObjectName="SingletonObject";
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                   GameObject obj= GameObject.Find(SingletonObjectName);
                   if(obj ==null) obj = new GameObject (SingletonObjectName);
                    //隐藏实例化的new game object，下同  
                   // obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
      protected set { _instance = value; }
    }
    [Obsolete("统一单利，有的人用的‘main’,请用'Instance'代替。")]
    public static T main { get { return Instance; }set { Instance = value; } }
    public static bool hasInstance { get {return _instance != null; } }
    protected virtual void OnDestroy() { _instance = null; }
    
    protected virtual void Awake()
    {
        _instance = GetComponent<T>();
    }
}
