using UnityEngine;

/** Copyright(C) 2019 by 	WoogiWorld
 *All rights reserved.
 *ProductName:  		WoogiCreate
 *FileName:     		Woogi3DLable.cs
 *Author:       		zhangchengang
 *Version:      		2.5.4
 *UnityVersion：		2018.3.0f2
 *Date:         		2019-07-11
 *Description:          No description
 *History:
*/

public class Woogi3DLable : MonoBehaviour
{
    [Header("Lable")]
    public Transform target;
    public float maxDistances = 20;
    public float minDistances = 10;
    public Vector3 originalLocalScale;
    public bool isDefault = true;
    private CanvasGroup alpha;
    protected virtual void Awake()
    {
        if (isDefault)
        {
            alpha = GetComponent<CanvasGroup>();
            if (alpha == null)
            {
                alpha = gameObject.AddComponent<CanvasGroup>();
            }
            alpha.alpha = 0f;
        }
    }
    protected virtual void Start()
    {
        originalLocalScale = transform.localScale;
    }


    // Update is called once per frame
    protected void UpdateLabelState(Transform target)
    {
        if (target == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, target.transform.position);

        float length = maxDistances - minDistances;
        if (alpha == null)
        {
            alpha = GetComponent<CanvasGroup>();
            if (alpha == null)
                alpha = gameObject.AddComponent<CanvasGroup>();
        }
        else
        {
            if (distance >= maxDistances) alpha.alpha = 0;
            else if (distance <= minDistances)
            {
                alpha.alpha = 1;
                transform.localScale = originalLocalScale * (distance / minDistances);
            }
            else
            {
                alpha.alpha = 1 - (distance - minDistances) / length;
                transform.localScale = originalLocalScale;
            }
        }
    }
}
