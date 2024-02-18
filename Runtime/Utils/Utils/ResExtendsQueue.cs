using System;
public class ResExtendsBase
{
    public bool isLoadRes = false;
    public string path;
    public string abName;
    public string root;
    public ResExtendsBase(string _path, string _abName, string _root)
    {
        path = _path;
        abName = _abName;
        root = _root;
    }
    public virtual bool LoadAsyncRes() { return false; }
}
public class ResExtendsQueue<T> : ResExtendsBase
{
    public Action<T> callback;
    public Action<ResExtendsQueue<T>> resExtends;
    ~ResExtendsQueue()
    {
        callback = null;
        resExtends = null;
    }
    public ResExtendsQueue(string _path, string _abName, string _root, Action<T> _callback, Action<ResExtendsQueue<T>> _resExtends) : base(_path, _abName, _root)
    {
        callback = _callback;
        resExtends = _resExtends;
    }
    public override bool LoadAsyncRes()
    {
        if (!isLoadRes)
        {
            isLoadRes = true;
            resExtends?.Invoke(this);
            return true;
        }
        return false;
    }
}