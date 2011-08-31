using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuState {
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Set As Entry State")]
    public static void SetEntryState(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State state= context.SelectedObject as AP_State;
        AP_State parentState= state.Parent as AP_State;
        if(parentState) { parentState.EntryState= state; return; }
        AP_StateChart stateChart= state.Parent as AP_StateChart;
        if(stateChart) { stateChart.EntryState= state; }
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Set As Entry State", true)]
    public static bool ValidateSetEntryState(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State state= context.SelectedObject as AP_State;
        if(state == null) return false;
        AP_State parentState= state.Parent as AP_State;
        if(parentState && parentState.EntryState == state) return false;
        AP_StateChart stateChart= state.Parent as AP_StateChart;
        if(stateChart && stateChart.EntryState == state) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add State")]
    public static void AddState(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State parent= context.SelectedObject as AP_State;
        AP_State instance= AP_State.CreateInstance("", parent);
        instance.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add State", true)]
    public static bool ValidateAddState(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        return context.SelectedObject is AP_State;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add Entry Function")]
    public static void AddEntryFunction(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Module instance= AP_Module.CreateInstance("OnEntry", parent);
        instance.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add Entry Function", true)]
    public static bool ValidateAddEntryFunction(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State parentState= context.SelectedObject as AP_State;
        if(parentState == null) return false;
        return parentState.OnEntryAction == null;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add Exit Function")]
    public static void AddExitFunction(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Node parent= context.SelectedObject as AP_Node;
        AP_Module instance= AP_Module.CreateInstance("OnExit", parent);
        instance.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add Exit Function", true)]
    public static bool ValidateAddExitFunction(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State parentState= context.SelectedObject as AP_State;
        if(parentState == null) return false;
        return parentState.OnExitAction == null;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add Update Function")]
    public static void AddUpdateFunction(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State parent= context.SelectedObject as AP_State;
        AP_Module instance= AP_Module.CreateInstance("OnUpdate", parent);
        instance.SetInitialPosition(context.GraphPosition);        
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add Update Function", true)]
    public static bool ValidateAddUpdateFunction(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State parentState= context.SelectedObject as AP_State;
        if(parentState == null) return false;
        return parentState. OnUpdateAction == null;
    }
    
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add Transition")]
    public static void AddTransition(MenuCommand command) {
//        AP_MenuContext context= command.context as AP_MenuContext;
//        AP_State state= context.SelectedObject as AP_State;
//        AP_Transition instance= AP_Transition.CreateInstance("Transition", state);
//        AP_Aggregate parent= state.Parent;
//        parent.ForEachChild<AP_State>(
//            (child)=> {
//                if(child != state) {
//                    instance.myEndState= child;
//                }
//            }
//        );
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Add Transition", true)]
    public static bool ValidateAddTransition(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State state= context.SelectedObject as AP_State;
        if(state == null) return false;
        AP_Aggregate parent= state.Parent;
        return parent.ChildCount<AP_State>() == 2;
    }

    // ======================================================================
    // COMMON AREA
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Delete")]
    public static void Delete(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_State state= context.SelectedObject as AP_State;
        if(EditorUtility.DisplayDialog("Deleting State", "Are you sure you want to delete state: "+state.NameOrTypeName+" and all of its children?", "Delete", "Cancel")) {
            state.Dealloc();
        }                                
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/State/Delete", true)]
    public static bool ValidateDelete(MenuCommand command) {
        return true;
    }

}
