using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_DisplayOptionEnum { Maximized, Minimized, Folded }

public static class iCS_DisplayOption {
    public static bool IsMaximized        (iCS_DisplayOptionEnum opt)     { return opt == iCS_DisplayOptionEnum.Maximized; }
    public static bool IsMinimized        (iCS_DisplayOptionEnum opt)     { return opt == iCS_DisplayOptionEnum.Minimized; }
    public static bool IsFolded           (iCS_DisplayOptionEnum opt)     { return opt == iCS_DisplayOptionEnum.Folded; }
    public static void SetMaximized       (ref iCS_DisplayOptionEnum opt) { opt= iCS_DisplayOptionEnum.Maximized; }
    public static void SetMinimized       (ref iCS_DisplayOptionEnum opt) { opt= iCS_DisplayOptionEnum.Minimized; }
    public static void SetFolded          (ref iCS_DisplayOptionEnum opt) { opt= iCS_DisplayOptionEnum.Folded; }
}
