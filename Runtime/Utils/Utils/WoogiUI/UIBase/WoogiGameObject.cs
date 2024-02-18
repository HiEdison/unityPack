using UnityEngine;
namespace WoogiWorld.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class WoogiGameObject : MonoBehaviour
    {
        public Transform Parent
        {
            get { return transform.parent; }
        }

        public RectTransform rectTransform
        {
            get { return GetComponent<RectTransform>(); }
        }
    }
}


