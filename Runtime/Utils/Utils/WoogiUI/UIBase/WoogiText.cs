using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using WoogiWorld.UI;

namespace WoogiWorld.UI
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class WoogiText : WoogiGameObject
    {
        public Text woogiText
        {
            get { return GetComponent<Text>(); }
        }

        public string text
        {
            get { return woogiText.text; }
            set { woogiText.text = value; }
        }
    }
}

