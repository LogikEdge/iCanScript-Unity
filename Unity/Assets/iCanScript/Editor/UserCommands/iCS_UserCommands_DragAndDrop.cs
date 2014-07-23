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
        if(!IsCreationAllowed()) return false;
        var iStorage= node.IStorage;
        string iconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
        node.IconGUID= iconGUID;                    
        node.LayoutNode();
	    iStorage.SaveStorage("Change Icon");
        iCS_EditorController.RepaintEditorsWithLabels();			
        return true;
    }
	// ----------------------------------------------------------------------
    public static void PasteIntoGraph(iCS_MonoBehaviourImp sourceMonoBehaviour, iCS_EngineObject sourceRoot,
                                      iCS_IStorage iStorage, iCS_EditorObject parent, Vector2 globalPos) {
        if(!IsCreationAllowed()) return;
        iStorage.AnimateGraph(null,
            _=> {
                iCS_IStorage srcIStorage= new iCS_IStorage(sourceMonoBehaviour);
                iCS_EditorObject srcRoot= srcIStorage.EditorObjects[sourceRoot.InstanceId];
                iCS_EditorObject pasted= iStorage.Copy(srcRoot, srcIStorage, parent, globalPos, iStorage);
                if(pasted.IsUnfoldedInLayout) {
                    pasted.Fold();            
                }
                iStorage.ForcedRelayoutOfTree();
            }
        );
        iStorage.SaveStorage("Add Prefab "+sourceRoot.Name);
    }
	// ----------------------------------------------------------------------
    public static void DragAndDropSetPortValue(iCS_EditorObject port, UnityEngine.Object value) {
        var iStorage= port.IStorage;
        port.PortValue= value;
        iStorage.SaveStorage("Set port "+port.Name);
    }

}
