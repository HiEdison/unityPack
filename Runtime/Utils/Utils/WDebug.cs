using System.Diagnostics;

namespace UnityEngine
{
    public class WDebug
    {
        //test add 2
        //test add 1
        //test add something in china dev
        public static bool isLog = true;

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.Log(LogType.Log, message);
        }
        
        public static void LogTime(object message)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.Log(LogType.Log, $"[{Time.realtimeSinceStartup}] {message}");
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Log(object message, bool isRed)
        {
            if (!isLog)
            {
                return;
            }
#if UNITY_EDITOR
            if (isRed)
            {
                Debug.unityLogger.Log(LogType.Log, $"Rich:<color=#ff0000ff>{message}</color>");
            }
            else
#endif

            {
                Log($"Rich:{message}");
            }
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Log(object message, Object context)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.Log(LogType.Log, message, context);
        }

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogFormat(string format, params object[] args)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.LogFormat(LogType.Log, format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogFormat(Object context, string format, params object[] args)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.LogFormat(LogType.Log, context, format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogFormat(
            LogType logType,
            LogOption logOptions,
            Object context,
            string format,
            params object[] args)
        {
            if (!isLog)
            {
                return;
            }

            Debug.LogFormat(logType, logOptions, context, format, args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.Log(LogType.Warning, message);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message, Object context)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.Log(LogType.Warning, message, context);
        }

        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.LogFormat(LogType.Warning, format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.LogFormat(LogType.Warning, context, format, args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.Log(LogType.Error, message);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogError(object message, Object context)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.Log(LogType.Error, message, context);
        }

        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(string format, params object[] args)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.LogFormat(LogType.Error, format, args);
        }

        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            if (!isLog)
            {
                return;
            }

            Debug.unityLogger.LogFormat(LogType.Error, context, format, args);
        }
    }
}