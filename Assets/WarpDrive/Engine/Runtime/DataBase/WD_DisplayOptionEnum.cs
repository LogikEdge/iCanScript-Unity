using UnityEngine;
using System.Collections;

[System.Serializable]
public enum WD_DisplayOptionEnum { Normal, Minimized, Folded, Hidden }

public partial class WD {
    public static bool IsDisplayedNormally(WD_EditorObject obj) { return obj.DisplayOption == WD_DisplayOptionEnum.Normal; }
    public static bool IsMinimized        (WD_EditorObject obj) { return obj.DisplayOption == WD_DisplayOptionEnum.Minimized; }
    public static bool IsFolded           (WD_EditorObject obj) { return obj.DisplayOption == WD_DisplayOptionEnum.Folded; }
    public static bool IsHidden           (WD_EditorObject obj) { return obj.DisplayOption == WD_DisplayOptionEnum.Hidden; }
    public static void DisplayNormally    (WD_EditorObject obj) { obj.DisplayOption= WD_DisplayOptionEnum.Normal; }
    public static void Fold               (WD_EditorObject obj) { obj.DisplayOption= WD_DisplayOptionEnum.Folded; }
    public static void Minimize           (WD_EditorObject obj) { obj.DisplayOption= WD_DisplayOptionEnum.Minimized; }
    public static void Hide               (WD_EditorObject obj) { obj.DisplayOption= WD_DisplayOptionEnum.Hidden; }
}
