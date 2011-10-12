using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Debug")]
public class WD_Log {
    [WD_Function] public static void Evaluate(string message) { Debug.Log(message);}
}
