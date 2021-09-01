using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MDebug
{
    public static void Log(string content)
    {
#if Debug
        Debug.Log(content);
#endif
    }

    public static void LogWarning(string content)
    {
#if Debug
        Debug.LogWarning(content);
#endif
    }

    public static void LogError(string content)
    {
#if Debug
        Debug.LogError(content);
#endif
    }
}
