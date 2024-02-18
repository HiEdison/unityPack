using UnityEngine;
using System.Collections;
namespace WoogiWorld.AssetBundles
{
    public class ABLoadAssetOperationSimulation : ABLoadAssetOperation
    {
        Object m_SimulatedObject;
        ~ABLoadAssetOperationSimulation()
        {
            m_SimulatedObject = null;
        }
        public ABLoadAssetOperationSimulation(Object simulatedObject)
        {
            m_SimulatedObject = simulatedObject;
        }

        public override T GetAsset<T>()
        {
            return m_SimulatedObject as T;
        }

        public override string GetAssetbundlePath()
        {
            return "";
        }

        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return true;
        }

        public override bool IsRequestNull()
        {
            return false;
        }
    }
}
