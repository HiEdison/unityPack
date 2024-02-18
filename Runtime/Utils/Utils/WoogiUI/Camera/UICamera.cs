using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WoogiWorld.Event;
using WoogiWorld.UI;

[ExecuteInEditMode()]
public class UICamera : MonoBehaviour
{
    public static bool HasInstance
    {
        get { return instance; }
    }

    private static UICamera instance;

    public static UICamera Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<UICamera>();
                if (instance == null)
                {
                    WDebug.LogError(" dont find UICamera!");
                }
            }

            return instance;
        }
    }

    void OnDestroy()
    {
        EventDispatcher.Instance.RemoveAllEventListener(this);
        instance = null;
    }

    public void Awake()
    {
        UiCamera.orthographic = true;
//#if UNITY_EDITOR
        transform.localPosition = Vector3.up * 5000f;
//#endif
    }

    private void OnAppQuit(EventMessage info)
    {
        SetCameraRect(0);
    }

    private void onOrientationChange(EventMessage info)
    {
    }

    public void SetCameraRect(float offset)
    {
        if (offset > 0)
        {
#if UNITY_IOS || UNITY_ANDROID
            if (Screen.orientation == ScreenOrientation.Portrait)
            {
                float v = offset / Screen.height;
                UiCamera.rect = new Rect(new Vector2(0, v), new Vector2(1f, 1f - 2 * v));
            }
            else
#endif
            {
                float v = offset / Screen.width;
                UiCamera.rect = new Rect(new Vector2(v, 0), new Vector2(1f - 2 * v, 1f));
            }
        }
        else
        {
            UiCamera.rect = new Rect(Vector2.zero, Vector2.one);
        }
    }

    public Camera UiCamera
    {
        get { return GetComponent<Camera>(); }
    }

    public void ResetUICamera()
    {
        if (UiCamera != null)
            UiCamera.clearFlags = CameraClearFlags.Depth;
        List<string> wcls = UIManager.Instance.GetAllCanvasKeys();
        for (int i = 0; i < wcls.Count; i++)
        {
            Canvas canvas = UIManager.Instance.GetCanvas<Canvas>(wcls[i]);
            if (canvas != null)
            {
                if (canvas.worldCamera == UiCamera)
                {
                    GraphicRaycaster c = canvas.GetComponent<GraphicRaycaster>();
                    if (c != null)
                        c.enabled = true;
                }
            }
        }

        UICameraAble = true;
    }

    public Camera SetUICameraToVideoMode()
    {
        Camera uiCam = UICamera.Instance.UiCamera;
        if (uiCam != null)
        {
            uiCam.clearFlags = CameraClearFlags.Color;
            uiCam.backgroundColor = Color.black;
        }

        return uiCam;
    }


    [SerializeField] private bool isOpenUICamera = false;

    public bool IsOpenUICamera
    {
        get
        {
            if (gameObject.activeSelf != isOpenUICamera)
            {
                gameObject.SetActive(isOpenUICamera);
            }

            return isOpenUICamera;
        }
    }

    public bool UICameraAble
    {
        set { UiCamera.enabled = value; }
    }

    Vector2 screenSize = new Vector2(1920f, 1080f);

    protected void Update()
    {
        if (Screen.width != screenSize.x || Screen.height != screenSize.y)
        {
            screenSize = new Vector2(Screen.width, Screen.height);
            float size = UIEffectSize();
            UiCamera.orthographicSize = (2 - size) * 5f;
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            UiCamera.enabled = !UiCamera.enabled;
        }
    }

    private float UIEffectSize()
    {
        float currentSize = (Screen.width / (float) Screen.height);
        if (currentSize != 16 / (float) 9)
        {
            currentSize = currentSize / (16 / (float) 9);
        }
        else
            currentSize = 1f;

        return currentSize;
    }

    public void Death(bool isDeath)
    {
        SetCamerEffect(GetComponent<CustomBrightnessSaturationAndContrast>(), isDeath);
    }

    void SetCamerEffect(CustomBrightnessSaturationAndContrast a, bool isDeath)
    {
        if (a != null)
        {
            a.enabled = isDeath;
            if (isDeath)
            {
                a.saturation = .2f;
                a.brightness = 0f;
            }
            else
            {
                a.saturation = 1f;
                a.brightness = 1f;
            }
        }
    }
}