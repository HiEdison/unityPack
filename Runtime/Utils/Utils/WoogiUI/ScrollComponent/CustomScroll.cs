using SG;
using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [DisallowMultipleComponent]
    public class CustomScroll : LoopScrollRect
    {
        public Action<Transform, int> UpdateCell;
        public Action<bool> DragHandler;
        public Action<Vector2> UpdateBoundsChanged;
        public GameObject parefab;
        public int poolSize = 5;
        //obj pool
        private SG.Pool poolDict = null;
        protected override void OnDestroy()
        {
            parefab = null;
            poolDict = null;
            base.OnDestroy();
        }
        public void InitPool(int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (poolDict != null)
            {
                return;
            }
            else
            {
                if (parefab == null)
                {
                    WDebug.LogError("[CustomScroll] Invalide prefab name for pooling : null");
                    return;
                }
                poolDict = new SG.Pool(parefab.name, parefab, gameObject, size, type);
            }
        }

        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public override GameObject GetObjectFromPool(bool autoActive = true)
        {
            GameObject result = null;
            if (poolDict == null && poolSize > 0)
            {
                InitPool(poolSize, PoolInflationType.INCREMENT);
            }

            if (poolDict != null)
            {
                SG.Pool pool = poolDict;
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    WDebug.LogWarning("[CustomScroll]:No object available.");
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                WDebug.LogError("[CustomScroll]:Invalid pool name specified: ");
            }
#endif
            return result;
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="go"></param>
        public void ReturnObjectToPool(GameObject go)
        {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
#if UNITY_EDITOR
                WDebug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
            }
            else
            {
                SG.Pool pool = poolDict;
                if (pool != null)
                {
                    pool.ReturnObjectToPool(po);
                }
#if UNITY_EDITOR
                else
                {
                    WDebug.LogWarning("No pool available with name: " + po.poolName);
                }
#endif
            }
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="t"></param>
        public void ReturnTransformToPool(Transform t)
        {
            if (t == null)
            {
#if UNITY_EDITOR
                WDebug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }

        public override void ProvideData(Transform transform, int idx)
        {
            transform.name = "cell_" + idx;
            if (UpdateCell != null) UpdateCell.Invoke(transform, idx);
        }

        public override void ReturnObject(Transform go)
        {
            ReturnObjectToPool(go.gameObject);
            go.SendMessage("RecycleCell", SendMessageOptions.DontRequireReceiver);
        }

        public override void UpdateBoundsSize(Vector3 size)
        {
            if (UpdateBoundsChanged != null)
                UpdateBoundsChanged.Invoke(size);
        }


        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (m_Dragging)
            {
                if (DragHandler != null)
                    DragHandler.Invoke(true);
            }
        }
        
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (m_Dragging)
            {
                if (DragHandler != null)
                    DragHandler.Invoke(false);
            }
        }

        protected override float GetSize(RectTransform item)
        {
            throw new NotImplementedException();
        }

        protected override float GetDimension(Vector2 vector)
        {
            throw new NotImplementedException();
        }

        protected override Vector2 GetVector(float value)
        {
            throw new NotImplementedException();
        }
    }
}
