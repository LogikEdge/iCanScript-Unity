using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuState {
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/State/Set As Entry State")]
    public static void SetEntryState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_State state= context.SelectedObject as WD_State;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        
        state.Parent.Case<WD_State, WD_StateChart>(
            (parentState)      => { parentState.EntryState= state; },
            (parentStateChart) => { parentStateChart.EntryState= state; }
        );
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/State/Set As Entry State", true)]
    public static bool ValidateSetEntryState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_State state= context.SelectedObject as WD_State;
        if(state == null) return false;
        WD_State parentState= state.Parent as WD_State;
        if(parentState != null && parentState.EntryState == state) return false;
        WD_StateChart stateChart= state.Parent as WD_StateChart;
        if(stateChart != null && stateChart.EntryState == state) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/State/Add State")]
    public static void AddState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_State parent= context.SelectedObject as WD_State;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_State>("", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/State/Add State", true)]
    public static bool ValidateAddState(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        return context.SelectedObject is WD_State;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/State/Add Entry Function")]
    public static void AddEntryFunction(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Module>("OnEntry", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/State/Add Entry Function", true)]
    public static bool ValidateAddEntryFunction(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_State parentState= context.SelectedObject as WD_State;
        if(parentState == null) return false;
        return parentState.OnEntryAction == null;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/State/Add Exit Function")]
    public static void AddExitFunction(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Node parent= context.SelectedObject as WD_Node;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Module>("OnExit", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/State/Add Exit Function", true)]
    public static bool ValidateAddExitFunction(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_State parentState= context.SelectedObject as WD_State;
        if(parentState == null) return false;
        return parentState.OnExitAction == null;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/State/Add Update Function")]
    public static void AddUpdateFunction(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_State parent= context.SelectedObject as WD_State;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        editorObjects.CreateInstance<WD_Module>("OnUpdate", parent.InstanceId, context.GraphPosition);
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/State/Add Update Function", true)]
    public static bool ValidateAddUpdateFunction(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_State parentState= context.SelectedObject as WD_State;
        if(parentState == null) return false;
        return parentState. OnUpdateAction == null;
    }
    
    // ======================================================================
    // COMMON AREA
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/State/Delete")]
    public static void Delete(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_State state= context.SelectedObject as WD_State;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;
        if(EditorUtility.DisplayDialog("Deleting State", "Are you sure you want to delete state: "+state.NameOrTypeName+" and all of its children?", "Delete", "Cancel")) {
            editorObjects.DestroyInstance(state.InstanceId);
        }                                
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/State/Delete", true)]
    public static bool ValidateDelete(MenuCommand command) {
        return true;
    }

}
