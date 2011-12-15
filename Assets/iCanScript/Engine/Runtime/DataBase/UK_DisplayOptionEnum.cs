using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_DisplayOptionEnum { Normal, Minimized, Folded }

public static partial class WD {
    public static bool IsDisplayedNormally(iCS_EditorObject obj) { return obj.DisplayOption == iCS_DisplayOptionEnum.Normal; }
    public static bool IsMinimized        (iCS_EditorObject obj) { return obj.DisplayOption == iCS_DisplayOptionEnum.Minimized; }
    public static bool IsFolded           (iCS_EditorObject obj) { return obj.DisplayOption == iCS_DisplayOptionEnum.Folded; }
    public static void DisplayNormally    (iCS_EditorObject obj) { obj.DisplayOption= iCS_DisplayOptionEnum.Normal; }
    public static void Fold               (iCS_EditorObject obj) { obj.DisplayOption= iCS_DisplayOptionEnum.Folded; }
    public static void Minimize           (iCS_EditorObject obj) { obj.DisplayOption= iCS_DisplayOptionEnum.Minimized; }
}
