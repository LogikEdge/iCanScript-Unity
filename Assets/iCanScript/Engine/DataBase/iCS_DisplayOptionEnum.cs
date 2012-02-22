using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_DisplayOptionEnum { Maximized, Minimized, Folded }

public static class iCS_DisplayOption {
    public static bool IsMaximized        (iCS_EditorObject obj) { return obj.DisplayOption == iCS_DisplayOptionEnum.Maximized; }
    public static bool IsMinimized        (iCS_EditorObject obj) { return obj.DisplayOption == iCS_DisplayOptionEnum.Minimized; }
    public static bool IsFolded           (iCS_EditorObject obj) { return obj.DisplayOption == iCS_DisplayOptionEnum.Folded; }
    public static void Maximize           (iCS_EditorObject obj) { obj.DisplayOption= iCS_DisplayOptionEnum.Maximized; }
    public static void Fold               (iCS_EditorObject obj) { obj.DisplayOption= iCS_DisplayOptionEnum.Folded; }
    public static void Minimize           (iCS_EditorObject obj) { obj.DisplayOption= iCS_DisplayOptionEnum.Minimized; }
}
