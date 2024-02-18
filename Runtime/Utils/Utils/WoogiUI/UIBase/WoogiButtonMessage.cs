using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class WoogiButtonMessage : MonoBehaviour {
    public delegate void ButtonEvent( GameObject go );
    public MonoBehaviour    mono         = null;//接受的对象
    public string           functionName = "";  //方法名
    public string           mButtonPath   = null;
    public ButtonEvent      onclickEvent = null;//委托

    void OnDestroy()
    {
        mono = null;
        onclickEvent = null;
    }
    void Awake()
    {
        mButtonPath = WoogiTools.GetObjectPath(gameObject);
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetEvent(ButtonEvent onclikEvent_ )
    {
        functionName = onclikEvent_.Method.Name;
        mono = onclikEvent_.Target as MonoBehaviour;
        onclickEvent = onclikEvent_;
       // GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if( onclickEvent != null )
        {
            onclickEvent(gameObject);
        }
    }
}
