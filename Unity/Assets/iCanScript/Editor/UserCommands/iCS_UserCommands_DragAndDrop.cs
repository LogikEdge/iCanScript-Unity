//
// File: iCS_UserCommands_DragAndDrop
//
//#define DEBUG
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
	// ----------------------------------------------------------------------
    public static void PasteIntoGraph(iCS_Storage sourceStorage, iCS_EngineObject sourceRoot,
                                      iCS_IStorage iStorage, iCS_EditorObject parent, Vector2 globalPos) {
        iStorage.RegisterUndo("Add Prefab "+sourceRoot.Name);
        iStorage.AnimateGraph(null,
            _=> {
                iCS_IStorage srcIStorage= new iCS_IStorage(sourceStorage);
                iCS_EditorObject srcRoot= srcIStorage.EditorObjects[sourceRoot.InstanceId];
                iCS_EditorObject pasted= iStorage.Copy(srcRoot, srcIStorage, parent, globalPos, iStorage);
                if(pasted.IsUnfoldedInLayout) {
                    pasted.Fold();            
                }
                pasted.LayoutNodeAndParents();                
            }
        );
    }
	// ----------------------------------------------------------------------
    public static void DragAndDropPortValue(iCS_EditorObject port) {
        var iStorage= port.IStorage;
        iStorage.RegisterUndo("Set port "+port.Name);
    }

}
