//
// File: iCS_UserCommands
//
using UnityEngine;
using UnityEditor;
using System.Collections;
using P=Prelude;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Utilities
	// ----------------------------------------------------------------------
    static Rect BuildRect(Vector2 p, Vector2 s) {
        return new Rect(p.x, p.y, s.x, s.y);
    }
}
