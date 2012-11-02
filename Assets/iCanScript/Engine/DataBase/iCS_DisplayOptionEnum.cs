using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_DisplayOptionEnum { Unfolded, Folded, Iconized }

public static class iCS_DisplayOption {
    public static bool IsUnfolded         (iCS_DisplayOptionEnum opt)     { return opt == iCS_DisplayOptionEnum.Unfolded; }
    public static bool IsFolded           (iCS_DisplayOptionEnum opt)     { return opt == iCS_DisplayOptionEnum.Folded; }
    public static bool IsIconized         (iCS_DisplayOptionEnum opt)     { return opt == iCS_DisplayOptionEnum.Iconized; }
    public static void Unfold             (ref iCS_DisplayOptionEnum opt) { opt= iCS_DisplayOptionEnum.Unfolded; }
    public static void Fold               (ref iCS_DisplayOptionEnum opt) { opt= iCS_DisplayOptionEnum.Folded; }
    public static void Iconize            (ref iCS_DisplayOptionEnum opt) { opt= iCS_DisplayOptionEnum.Iconized; }
}
