using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
/// <summary>
/// facing the screen 3d gameobject
/// 3d 对象始终面向屏幕.
/// </summary>
public class FaceScreen3DObject : Woogi3DLable
{
    public GameObject content = null;
    public bool isVirtual2D = false;
    protected bool isVisible = false;
    private Vector3 cameraPosition = Vector3.zero;
    private Quaternion cameraRotation = new Quaternion();

    private Vector3 contentPosition = Vector3.zero;
    private Quaternion contentRotation = new Quaternion();
    private Camera mainCamera;
    protected virtual void OnDestroy()
    {
        content = null;
        mainCamera = null;
    }
void Update()
    {

        if (isVisible)
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            if (mainCamera != null)
            {
                if (content.gameObject.activeSelf)
                {
                    Vector3 currentCameraPosition = mainCamera.transform.position;
                    Vector3 currentContentPosition = content.transform.position;
                    Quaternion currentCameraRotation = mainCamera.transform.rotation;
                    Quaternion currentContentRotation = content.transform.rotation;
                    if (CheckVector(cameraPosition, currentCameraPosition) ||
                        CheckQuaternion(cameraRotation, currentCameraRotation) ||
                        CheckVector(contentPosition, currentContentPosition) ||
                        CheckQuaternion(contentRotation, currentContentRotation))
                    {
                        //content.transform.LookAt(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z), Vector3.up);
                        if (isVirtual2D)
                        {
                            content.transform.eulerAngles = (mainCamera.transform.eulerAngles);
                            content.transform.forward = -mainCamera.transform.forward;
                        }
                        else
                        {
                            LookAt(content.transform, currentCameraPosition, currentCameraRotation, currentContentRotation, currentContentPosition);
                        }
                    }

                    UpdateLabelState(mainCamera.transform);
                }
            }
        }


    }
    protected virtual void LookAt(Transform tf, Vector3 currentCameraPosition, Quaternion currentCameraRotation, Quaternion currentContentRotation, Vector3 currentContentPosition)
    {
        cameraPosition = currentCameraPosition;
        cameraRotation = currentCameraRotation;
        contentRotation = currentContentRotation;
        contentPosition = currentContentPosition;
        tf.LookAt(new Vector3(currentCameraPosition.x, content.transform.position.y, currentCameraPosition.z), Vector3.up);
    }
    protected virtual void OnBecameInvisible()
    {
        isVisible = false;
    }

    protected virtual void OnBecameVisible()
    {
        isVisible = true;
    }

    protected virtual bool CheckVector(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2) > 0.1f ? true : false;
    }

    protected virtual bool CheckQuaternion(Quaternion q1, Quaternion q2)
    {
        return Quaternion.Angle(q1, q2) > 0.1f ? true : false;
    }
}
