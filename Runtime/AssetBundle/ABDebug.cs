using UnityEngine;

namespace WoogiWorld.AssetBundles
{
    public enum ABLog
    {
        Log,
        Warning,
        Error,
        Excited,
    }

    public partial class ABManager : MonoBehaviour
    {
        #region local log

        public static ABLog logMask;

        public static void Log(ABLog logType, string text)
        {
            if ((1 << (int) logType & (int) logMask) != 0)
            {
                if (Application.isEditor)
                {
                    switch (logType)
                    {
                        case ABLog.Error:
                            text = "<color=#ff0000ff>" + text + "</color>";
                            break;
                        case ABLog.Warning:
                            text = "<color=#ffa500ff>" + text + "</color>";
                            break;
                        case ABLog.Excited:
                            text = "<color=#00ff00ff>" + text + "</color>";
                            break;
                    }
                }

                WDebug.Log(logType + ":" + text);
            }
        }

        #endregion
    }
}