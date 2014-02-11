//
// File: iCS_UserCommands_Create
//
using UnityEngine;
using System.Collections;
using P=Prelude;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Object creation
	// ----------------------------------------------------------------------
    public static iCS_EditorObject CreatePackage(iCS_EditorObject parent, Vector2 globalPos, string name, bool isNameEditable) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create Package");
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.CreatePackage(parent.InstanceId, globalPos, name);
                package.IsNameEditable= isNameEditable;
                package.LayoutNodeAndParents();
            }
        );
        return package;
    }
	// ----------------------------------------------------------------------
    public static iCS_EditorObject CreateStateChart(iCS_EditorObject parent, Vector2 globalPos, string name) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create StateChart");
        iCS_EditorObject stateChart= null;
        iStorage.AnimateGraph(null,
            _=> {
                stateChart= iStorage.CreateStateChart(parent.InstanceId, globalPos, name);
                stateChart.LayoutNodeAndParents();
            }
        );
        return stateChart;        
    }
	// ----------------------------------------------------------------------
    public static iCS_EditorObject CreateState(iCS_EditorObject parent, Vector2 globalPos, string name) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create State");
        iCS_EditorObject state= null;
        iStorage.AnimateGraph(null,
            _=> {
                state= iStorage.CreateState(parent.InstanceId, globalPos, name);
                state.LayoutNodeAndParents();                
            }
        );
        return state;        
    }

    // ======================================================================
    // Node Wrapping in package.
	// ----------------------------------------------------------------------
    public static iCS_EditorObject WrapInPackage(iCS_EditorObject obj) {
        if(obj == null || !obj.CanHavePackageAsParent()) return null;
        var iStorage= obj.IStorage;
        iStorage.RegisterUndo("Wrap : "+obj.Name);
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.WrapInPackage(obj);
                package.LayoutNodeAndParents();
            }
        );
        return package;
    }
	// ----------------------------------------------------------------------
    public static iCS_EditorObject WrapMultiSelectionInPackage(iCS_IStorage iStorage) {
        if(iStorage == null) return null;
        var selectedObjects= iStorage.FilterMultiSelectionForWrapInPackage();
        if(selectedObjects == null || selectedObjects.Length == 0) return null;
        iStorage.RegisterUndo("Wrap Selection");
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.WrapInPackage(selectedObjects);
                package.LayoutNodeAndParents();
                var r= Math3D.Union(P.map(n => n.LayoutRect, selectedObjects));
                package.myAnimatedRect.StartValue= BuildRect(Math3D.Middle(r), Vector2.zero);
            }
        );
        return package;
    }
}
