using System.Collections;
using UnityEngine;

/** Copyright(C) 2019 by 	WoogiWorld
 *All rights reserved.
 *ProductName:  		WoogiCreate
 *FileName:     		Woogi3DLableAuto.cs
 *Author:       		zhangchengang
 *Version:      		2.5.4
 *UnityVersion：		2018.3.0f2
 *Date:         		2019-07-11
 *Description:          Alpha gradients and zooms are automatically done
 *History:
*/

public class Woogi3DLableAuto : Woogi3DLable
{
    IEnumerator Start()
    {
        while (target == null)
        {
            yield return null;
            if (Camera.main != null)
                target = Camera.main.transform;
            yield return null;
        }
    }

    // Update is called once per frame
void Update()
    {
        UpdateLabelState(target);
    }
}
