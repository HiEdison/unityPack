using System;
using System.Collections;
using UnityEngine;

namespace WoogiWorld.AssetBundles
{
    public abstract class ABLoadOperation : IEnumerator
    {
        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset()
        {
        }

        abstract public bool Update();

        abstract public bool IsDone();
        abstract public bool IsRequestNull();
        
        public bool isError;
        public string error;
    }

    public abstract class ABLoadAssetOperation : ABLoadOperation
    {
        public abstract T GetAsset<T>() where T : UnityEngine.Object;
        public abstract string GetAssetbundlePath();
    }
}