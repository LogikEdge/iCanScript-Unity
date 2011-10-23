using UnityEngine;
using System.Collections;

[WD_Class(Company="Unity", Package="Debug", Name="Debug")]
public class WD_Debug {
    [WD_Function] public static void Log(string message) { Debug.Log(message); }
    [WD_Function] public static void LogWarning(string message) { Debug.LogWarning(message); }
    [WD_Function] public static void LogError(string message) { Debug.LogError(message); }
}
