using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCtrlBase<T> : MonoBehaviour
{
    public List<T> list = new List<T>();
    public CustomScroll srollrect;

    #region 数据拓展

    /// <summary>
    /// 在list.InsertRange(0,xxLs) or list.Insert(0,xx)后刷新.
    /// </summary>
    /// <param name="addCount">数量</param>
    public void InsertFirst(List<T> ls)
    {
        if (srollrect && ls != null && ls.Count > 0)
        {
            list.InsertRange(0, ls);
            srollrect.totalCount = list.Count;
            srollrect.itemTypeStart += ls.Count;
            srollrect.itemTypeEnd += ls.Count;
        }
    }

    public void Add(List<T> ls)
    {
        if (srollrect && ls != null && ls.Count > 0)
        {
            list.AddRange(ls);
            srollrect.totalCount = list.Count;
        }
    }

    #endregion

    #region 跳转、数据拓展并且跳转

    /// <summary>
    /// 跳转到首项
    /// </summary>
    public void GotoFirst()
    {
        if (srollrect)
        {
            if (srollrect.vertical)
                GotoFirstOrLast(srollrect.verticalScrollbar, true);
            if (srollrect.horizontal)
                GotoFirstOrLast(srollrect.horizontalScrollbar, true);
        }
        else
        {
            WDebug.LogError(this.name + "GotoFirst failed srollrect is null.");
        }
    }

    /// <summary>
    /// 在首位添加并跳转到首项
    /// </summary>
    public void GotoFirst(List<T> ls)
    {
        if (srollrect && ls != null && ls.Count > 0)
        {
            list.InsertRange(0, ls);
            srollrect.totalCount = list.Count;
            FineTuningSliderValue(0f, 0.01f);
            GotoFirst();
        }
    }

    /// <summary>
    /// 在末尾添加并跳转到末尾
    /// </summary>
    public void GotoEnd(List<T> ls)
    {
        if (srollrect && ls != null && ls.Count > 0)
        {
            list.AddRange(ls);
            srollrect.totalCount = list.Count;
            FineTuningSliderValue(1f, 0.99f);
            GotoEnd();
        }
    }

    /// <summary>
    /// 跳转到末尾
    /// </summary>
    public void GotoEnd()
    {
        if (srollrect)
        {
            if (srollrect.vertical)
                GotoFirstOrLast(srollrect.verticalScrollbar, false);
            if (srollrect.horizontal)
                GotoFirstOrLast(srollrect.horizontalScrollbar, false);
        }
        else
        {
            WDebug.LogError(this.name + "GotoEnd failed srollrect is null.");
        }
    }

    public void MoveEnd(List<T> ls, int speed = 1000)
    {
        if (srollrect && ls != null && ls.Count > 0)
        {
            list.AddRange(ls);
            srollrect.totalCount = list.Count;
            MoveEnd(speed);
        }
    }

    public void MoveEnd(int speed)
    {
        if (srollrect)
        {
            MoveFirstOrLast(false, speed);
        }
        else
        {
            WDebug.LogError(this.name + "GotoEnd failed srollrect is null.");
        }
    }

    #endregion

    void FineTuningSliderValue(float v0, float v1)
    {
        if (srollrect != null)
        {
            if (srollrect.verticalScrollbar != null)
            {
                if (srollrect.verticalScrollbar.value == v0)
                    srollrect.verticalScrollbar.value = v1;
                return;
            }
            else if (srollrect.horizontalScrollbar != null)
            {
                if (srollrect.horizontalScrollbar.value == v0)
                    srollrect.horizontalScrollbar.value = v1;
                return;
            }
        }

        WDebug.LogError(this.name + ":scrollbar or srollrect is null.");
    }

    void GotoFirstOrLast(Scrollbar scrollbar, bool isFirst)
    {
        if (scrollbar)
        {
            if (isFirst)
            {
                if (scrollbar.value > 0f)
                {
                    srollrect.reverseDirection = isFirst;
                    srollrect.RefillCellsFromEnd();
                    scrollbar.value = 0f;

                    Invoke("_RestDir", 0.01f);
                }
            }
            else
            {
                if (scrollbar.value < 1f)
                {
                    srollrect.reverseDirection = isFirst;
                    srollrect.RefillCellsFromEnd();
                    scrollbar.value = 1f;
                }
            }
        }
        else
        {
            WDebug.LogError(this.name + ":scrollbar is null.");
        }
    }

    void MoveFirstOrLast(bool isFirst, int speed = 1000)
    {
        if (isFirst)
        {
            if (srollrect.itemTypeStart != 0 || srollrect.verticalScrollbar.value != 0)
            {
                srollrect.SrollToCell(0, speed);
            }
        }
        else
        {
            if (srollrect.itemTypeEnd != srollrect.totalCount || srollrect.verticalScrollbar.value != 1)
            {
                srollrect.SrollToCell(srollrect.totalCount - 1, speed);
            }
        }
    }

    void _RestDir()
    {
        srollrect.reverseDirection = false;
    }
}