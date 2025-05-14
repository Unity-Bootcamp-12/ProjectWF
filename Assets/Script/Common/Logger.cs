using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

using Debug = UnityEngine.Debug;

/// <summary>
/// Debug.Log 래퍼 클래스
/// 형식화된 로그를 남기기 위해 사용한다.
/// </summary>
public static class Logger
{
    [Conditional("DEV_VER")]
    public static void Info(string message)
    {
        Debug.LogFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }

    [Conditional("DEV_VER")]
    public static void Warning(string message)
    {
        Debug.LogWarningFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }

    public static void Error(string message)
    {
        Debug.LogErrorFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }
}