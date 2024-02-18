using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace WoogiWorld.UI
{
    public class BtnBundleImageEvent : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler
    {
        public Image image;

        private Sprite targetGraphic;
        public Sprite highligtedSprite;
        public Sprite pressedSprite;
        public Sprite DisabledSprite;

        private Button btn;

        void Awake()
        {
            if (Btn != null)
                interactable = Btn.interactable;
            if (image != null)
                targetGraphic = image.sprite;
        }
        void OnDestroy()
        {
            btn = null;
            image = null;
            targetGraphic = pressedSprite = highligtedSprite = DisabledSprite = null;
        }
        private Button Btn
        {
            get
            {
                if (btn == null)
                {
                    btn = GetComponent<Button>();
                }
                return btn;
            }
        }
        private bool interactable;
        private bool isModify = false;

void Update()
        {
            if (CheckInteractableChange(ref isModify))
            {
                if (isModify)
                {
                    if (interactable)
                    {
                        if (image != null)
                            image.sprite = highligtedSprite;
                    }
                    else
                    {
                        if (image != null)
                            image.sprite = DisabledSprite;
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Check())
            {
                SetImageSprite(pressedSprite, targetGraphic);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Check())
            {
                SetImageSprite(highligtedSprite, targetGraphic);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Check())
            {
                SetImageSprite(targetGraphic, targetGraphic);
            }

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Check())
            {
                SetImageSprite(highligtedSprite, targetGraphic);
            }
        }

        private bool Check()
        {
            if (Btn != null && Btn.interactable && image != null)
                return true;
            return false;
        }

        private bool CheckInteractableChange(ref bool isModify)
        {
            if (Btn)
            {
                if (Btn.interactable != interactable)
                {
                    isModify = true;
                    interactable = Btn.interactable;
                }
                else
                    isModify = false;
                return true;
            }
            return false;
        }

        private void SetImageSprite(Sprite targetS, Sprite defaultSprite)
        {
            if (image != null)
            {
                if (targetS == null)
                {
                    if (image.sprite != defaultSprite)
                    {
                        image.sprite = defaultSprite;
                    }
                }
                else
                {
                    if (image.sprite != targetS)
                    {
                        image.sprite = targetS;
                    }
                }
            }
        }
    }
}
