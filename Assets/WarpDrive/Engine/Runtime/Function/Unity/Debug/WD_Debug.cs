using UnityEngine;
using System.Collections;

[WD_Class(Company="Unity", Package="Debug", Name="Debug")]
public class WD_Debug {
    [WD_Function] public static string Log(string message)        { Debug.Log(message); return message; }
    [WD_Function] public static string LogWarning(string message) { Debug.LogWarning(message); return message; }
    [WD_Function] public static string LogError(string message)   { Debug.LogError(message); return message; }
}
