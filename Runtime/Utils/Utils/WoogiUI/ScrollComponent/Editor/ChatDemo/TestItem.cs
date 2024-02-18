using UnityEngine;
using UnityEngine.UI;
namespace Demo.ScrollRect
{
    public class TestItem : MonoBehaviour
    {
        public Text msg;
        public Image img;

        RectTransform rectTransform;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        public void Ref(DataModel data)
        {
            if (!string.IsNullOrEmpty(data.msg))
            {
                this.msg.gameObject.SetActive(true);
                img.gameObject.SetActive(false);
                this.msg.text = data.msg;
            }
            else
            {
                this.msg.gameObject.SetActive(false);
                img.gameObject.SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        }
    }
}
