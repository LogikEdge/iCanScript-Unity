//
// File: iCS_UserCommands_DragAndDrop
//
#define DEBUG
using UnityEngine;
using UnityEditor;
using System.Collections;

public static partial class iCS_UserCommands {
    // ======================================================================
    // 
	// ----------------------------------------------------------------------
    // OK
    public static bool ChangeIcon(iCS_EditorObject node, Texture newTexture) {
        if(node == null) return false;
#if DEBUG
        Debug.Log("iCanScript: Change Icon => "+node.Name);
#endif
        var iStorage= node.IStorage;
	    iStorage.RegisterUndo("Change Icon");			
        string iconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
        node.IconGUID= iconGUID;                    
        node.LayoutNode();
        return true;
    }
}
