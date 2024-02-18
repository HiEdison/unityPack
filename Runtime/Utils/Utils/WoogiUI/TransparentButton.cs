using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//穿透 按钮 
public class TransparentButton :/*Image*/MaskableGraphic, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler, IMoveHandler
{

    public delegate void ButtonEvent(GameObject self, GameObject second);//需要做一个绑定处理

    public GraphicRaycaster graphicRaycaster = null;//
    public ButtonEvent btEvent = null;

    private Button fbtn = null;
    public bool interactable = true;
    protected override void OnDestroy()
    {
        fbtn = null;
        btEvent = null;
        graphicRaycaster = null;
        base.OnDestroy();
    }

    protected TransparentButton()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        if (fbtn != null)
        {
            fbtn.OnPointerClick(eventData);
        }
        if (btEvent != null && btEvent.Target.ToString() != "null")
        {
            btEvent(gameObject, fbtn == null ? null : fbtn.gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;
        fbtn = GetButton();
        if (fbtn != null)
        {
            fbtn.OnPointerDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) return;
        if (fbtn != null)
        {
            fbtn.OnPointerUp(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable) return;
        if (fbtn != null)
        {
            fbtn.OnPointerExit(eventData);
            fbtn = null;
        }
    }

    void Update()
    {
        if (!interactable) return;
        OnMove(null);
    }

    public void OnMove(AxisEventData eventData)
    {
        if (!interactable) return;
        if (fbtn != null)
        {
            Button btn = GetButton();
            if (fbtn != btn)
            {
                PointerEventData PointeventData = new PointerEventData(EventSystem.current);
                PointeventData.pressPosition = Input.mousePosition;
                PointeventData.position = Input.mousePosition;
                OnPointerExit(PointeventData);
            }
        }
    }

    Button GetButton()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;


        List<RaycastResult> list = new List<RaycastResult>();
        if (graphicRaycaster == null)
        {
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        if (graphicRaycaster == null) return null;
        graphicRaycaster.Raycast(eventData, list);

        //射线 射到 两个以上对的对象，则要对 最近的第二个对象进行响应执行
        int secondDepth = 0;
        if (graphicRaycaster == transform.GetComponentInParent<GraphicRaycaster>())
        {
            secondDepth = 2;
        }
        if (list.Count > secondDepth)
        {
            for (int i = secondDepth; i < list.Count; i++)
            {
                Button btn = list[i].gameObject.GetComponent<Button>();
                if (btn != null)
                {
                    return btn;
                }
            }
        }
        return null;
    }
}
