using UnityEngine;
using UnityEngine.UI;
namespace WoogiWorld.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(Image))]
    public class WoogiImage : WoogiGameObject
    {
        public Image image
        {
            get { return GetComponent<Image>(); }
        }

        public Sprite sprite
        {
            get { return image.sprite; }
            set { image.sprite = value; }
        }
    }
}
