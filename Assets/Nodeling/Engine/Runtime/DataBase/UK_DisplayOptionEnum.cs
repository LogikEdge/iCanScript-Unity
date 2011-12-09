using UnityEngine;
using System.Collections;

[System.Serializable]
public enum UK_DisplayOptionEnum { Normal, Minimized, Folded }

public static partial class WD {
    public static bool IsDisplayedNormally(UK_EditorObject obj) { return obj.DisplayOption == UK_DisplayOptionEnum.Normal; }
    public static bool IsMinimized        (UK_EditorObject obj) { return obj.DisplayOption == UK_DisplayOptionEnum.Minimized; }
    public static bool IsFolded           (UK_EditorObject obj) { return obj.DisplayOption == UK_DisplayOptionEnum.Folded; }
    public static void DisplayNormally    (UK_EditorObject obj) { obj.DisplayOption= UK_DisplayOptionEnum.Normal; }
    public static void Fold               (UK_EditorObject obj) { obj.DisplayOption= UK_DisplayOptionEnum.Folded; }
    public static void Minimize           (UK_EditorObject obj) { obj.DisplayOption= UK_DisplayOptionEnum.Minimized; }
}
