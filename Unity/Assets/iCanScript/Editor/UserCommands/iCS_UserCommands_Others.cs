//
// iCS_UserCommands_Others
//
//#define DEBUG
using UnityEngine;
using System.Collections;
using Pref= iCS_PreferencesController;


public static partial class iCS_UserCommands {
    // ======================================================================
    // Miscellanious User Commands.
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject SetAsStateEntry(iCS_EditorObject state) {
#if DEBUG
        Debug.Log("iCanScript: Set As Entry State => "+state.Name);
#endif
        if(state == null) return null;
        var iStorage= state.IStorage;
        iStorage.ForEachChild(state.Parent,
            child=>{
                if(child.IsEntryState) {
                    child.IsEntryState= false;
                }
            }
        );
        state.IsEntryState= true;
        var name= state.Name;
        iStorage.SaveStorage("Set As Entry "+name);
        return state;
    }
	// ----------------------------------------------------------------------
    public static void ShowInHierarchy(iCS_EditorObject obj) {
        var editor= iCS_EditorMgr.FindTreeViewEditor();
        if(editor != null) {
            editor.ShowElement(obj);
        }
    }
    // ----------------------------------------------------------------------
    // Change the display root to the selected object.
    public static void SetAsDisplayRoot(iCS_EditorObject obj) {
        if(obj == null || !obj.IsNode) return;
        var iStorage= obj.IStorage;
        if(iStorage.DisplayRoot == obj) return;
        iStorage.SaveNavigationState();
        iStorage.DisplayRoot= obj;
        iStorage.ForcedRelayoutOfTree(obj);
        var visualEditor= iCS_EditorMgr.FindVisualEditor();
        if(visualEditor != null) {
            visualEditor.OnDisplayRootChange();
        }
    }
    // ----------------------------------------------------------------------
    // Change the display root to the parent of the selected object.
    public static void ResetDisplayRoot(iCS_IStorage iStorage) {
        if(iStorage == null) return;
        iStorage.ClearNavigationHistory();
        var visualEditor= iCS_EditorMgr.FindVisualEditor();
        if(visualEditor != null) {
            visualEditor.OnDisplayRootChange();
        }
    }
    // ----------------------------------------------------------------------
	public static void ToggleShowDisplayRootNode(iCS_IStorage iStorage) {
		iStorage.ShowDisplayRootNode= !iStorage.ShowDisplayRootNode;
        iStorage.SaveStorage("Toggle Show Display Root");
	}
    // ----------------------------------------------------------------------
    public static void ChangeName(iCS_EditorObject obj, string name) {
        if(string.Compare(obj.RawName, name) == 0) return;
        var iStorage= obj.IStorage;
        iStorage.AnimateGraph(null,
            _=> {
                obj.Name= name;
                if(obj.IsNode) {
                    obj.LayoutNodeAndParents();
                }
                else if(obj.IsDataOrControlPort) {
                    obj.ParentNode.LayoutNodeAndParents();
                }
            }
        );
        obj.IStorage.SaveStorage("Change name => "+name);
    }
    // ----------------------------------------------------------------------
    public static void AutoLayoutPort(iCS_EditorObject port) {
        if(port == null) return;
        var iStorage= port.IStorage;
        var portPos= port.LayoutPosition;
        // First layout from port to provider
        var providerPort= iStorage.GetPointToPointProviderPortForConsumerPort(port);
        if(providerPort != null && providerPort != port) {
            var providerLayoutEndPoint= iStorage.GetProviderLineSegmentPosition(providerPort);
            iStorage.AutoLayoutPort(providerPort, portPos, providerLayoutEndPoint);
            iStorage.AutoLayoutOfPointToPointBindingExclusive(providerPort, port);
        }
        // Secondly, layout from port to consumer.
        var consumerPorts= iStorage.GetPointToPointConsumerPortsForProviderPort(port);
        if(consumerPorts != null) {
            foreach(var consumerPort in consumerPorts) {
                var consumerLayoutEndPoint= iStorage.GetConsumerLineSegmentPosition(consumerPort);
                iStorage.AutoLayoutPort(consumerPort, portPos, consumerLayoutEndPoint);
                iStorage.AutoLayoutOfPointToPointBindingExclusive(port, consumerPort);                
            }
        }
        // Save result.
        iStorage.SaveStorage("AutoLayout Port => "+port.Name);
    }
}
