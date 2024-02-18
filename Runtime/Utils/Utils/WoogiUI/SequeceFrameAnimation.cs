using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SequeceFrameAnimation : MonoBehaviour
{
    public string movieName;
    public List<Sprite> lSprites;
    public float fSep = 0.05f;
    public bool playOnAwake = false;
    public bool loop = false;

    public delegate void PlayOverCallback();

    private PlayOverCallback callback;
    private Image shower;
    int curFrame = 0;
    bool isPlay = false;

    void OnDestroy()
    {
        shower = null;
        callback = null;
        if (lSprites != null)
        {
            lSprites.Clear();
        }

        if (dMovieEvents != null)
        {
            dMovieEvents.Clear();
        }
    }

    public float showerWidth
    {
        get
        {
            if (shower == null)
            {
                return 0;
            }

            return shower.rectTransform.rect.width;
        }
    }

    public float showerHeight
    {
        get
        {
            if (shower == null)
            {
                return 0;
            }

            return shower.rectTransform.rect.height;
        }
    }

    void AddShowImage()
    {
        shower = GetComponent<Image>();
        if (shower == null)
            shower = gameObject.AddComponent<Image>();
        if (string.IsNullOrEmpty(movieName))
        {
            movieName = "movieName";
        }
    }

    protected  void OnEnable()
    {
        if (playOnAwake)
            Play(0);
    }

    public void Play(int startFrame, PlayOverCallback _callback = null)
    {
        AddShowImage();
        isPlay = true;
        callback = _callback;
        _Play(startFrame);
    }

    void _Play(int iFrame)
    {
        if (iFrame >= FrameCount && loop)
        {
            iFrame = 0;
        }
        else if (iFrame >= FrameCount && !loop)
        {
            iFrame = FrameCount - 1;
            isPlay = false;
            if (callback != null)
                callback();
            return;
        }

        shower.sprite = lSprites[iFrame];
        curFrame = iFrame;
        shower.SetNativeSize();
//		if (dMovieEvents.Keys.Count > 0)
//						WDebug.Log ("key  key :"+dMovieEvents.Keys);
        if (dMovieEvents.ContainsKey(iFrame))
        {
            foreach (delegateMovieEvent del in dMovieEvents[iFrame])
            {
                del();
            }
        }
    }

    public int FrameCount
    {
        get { return lSprites.Count; }
    }

    float fDelta = 0;

    void update()
    {
        if (isPlay)
        {
            fDelta += Time.deltaTime;
            if (fDelta > fSep)
            {
                fDelta = 0;
                curFrame++;
                _Play(curFrame);
            }
        }
    }

    public delegate void delegateMovieEvent();

    private Dictionary<int, List<delegateMovieEvent>> dMovieEvents = new Dictionary<int, List<delegateMovieEvent>>();

    public void RegistMovieEvent(int frame, delegateMovieEvent delEvent)
    {
        if (!dMovieEvents.ContainsKey(frame))
        {
            dMovieEvents.Add(frame, new List<delegateMovieEvent>());
        }

        dMovieEvents[frame].Add(delEvent);
    }

    public void UnregistMovieEvent(int frame, delegateMovieEvent delEvent)
    {
        if (!dMovieEvents.ContainsKey(frame))
        {
            return;
        }

        if (dMovieEvents[frame].Contains(delEvent))
        {
            dMovieEvents[frame].Remove(delEvent);
        }
    }
}