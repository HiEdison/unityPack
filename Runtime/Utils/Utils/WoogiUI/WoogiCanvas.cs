using UnityEngine;
using UnityEngine.UI;

namespace WoogiWorld.UI
{
    public sealed class WoogiCanvas : WoogiGameObject
    {
        private bool isRegist = false;
        public string SortLayerName;
        public bool isUseUICamera = true;
        public int sortingOrder = 0;
        [Header("canvs scaler: true=>使用组件设置")] public bool isCustomScaler = false;

        #region

        internal void Regist()
        {
            isRegist = true;
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            UIManager.Instance.RegisterCanvas(gameObject.name, this);
        }

        void OnDestroy()
        {
            UIManager.Instance.DestroyCanvas(gameObject.name, true);
        }

        #endregion

        void Awake()
        {
            CheckPlaneRegist();
            if (!isRegist)
                Regist();
            Canvas canvas = GetComponent<Canvas>();
            if (UICamera.Instance.IsOpenUICamera && isUseUICamera)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                Camera uiCamera = UICamera.Instance.UiCamera;
                canvas.worldCamera = uiCamera;
                canvas.planeDistance = 20f;
                CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                if (!isCustomScaler)
                {
                    canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                }

                if (!string.IsNullOrEmpty(SortLayerName))
                    canvas.sortingLayerName = SortLayerName;
            }
            else
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            canvas.sortingOrder = sortingOrder;
        }

        void Start()
        {
        }

        public int GetSortLayer
        {
            get { return GetComponent<Canvas>().sortingOrder; }
            set { GetComponent<Canvas>().sortingOrder = value; }
        }

        private void CheckPlaneRegist()
        {
            if (transform.childCount > 0)
            {
                foreach (Transform childTf in transform)
                {
                    WoogiPlane plane = childTf.GetComponent<WoogiPlane>();
                    if (plane != null)
                    {
                        plane.Regist();
                    }
                }
            }
        }
    }
}