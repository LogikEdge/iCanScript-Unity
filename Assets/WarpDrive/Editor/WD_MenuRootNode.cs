using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuRootNode {
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add Update State Chart")]
    public static void AddUpdateStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;

        WD_EditorObject top= editorObjects.CreateInstance<WD_Top>("Update", rootNode.InstanceId, context.GraphPosition);
        WD_EditorObject stateChart= editorObjects.CreateInstance<WD_StateChart>("Update", top.InstanceId, context.GraphPosition);
        WD_Top rtTop= editorObjects.GetRuntimeObject(top) as WD_Top;
        WD_StateChart rtStateChart= editorObjects.GetRuntimeObject(stateChart) as WD_StateChart;
        rtTop.Action= rtStateChart;
        rootNode.UpdateTop= rtTop;
        // Add initial state.
        WD_EditorObject state= editorObjects.CreateInstance<WD_State>("", stateChart.InstanceId, context.GraphPosition);
        WD_State rtState= editorObjects.GetRuntimeObject(state) as WD_State;
        rtStateChart.EntryState= rtState;

        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add Update State Chart", true)]
    public static bool ValidateAddUpdateStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        if(rootNode == null || rootNode.UpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add LateUpdate State Chart")]
    public static void AddLateUpdateStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;

        WD_EditorObject top= editorObjects.CreateInstance<WD_Top>("LateUpdate", rootNode.InstanceId, context.GraphPosition);
        WD_EditorObject stateChart= editorObjects.CreateInstance<WD_StateChart>("LateUpdate", top.InstanceId, context.GraphPosition);
        WD_Top rtTop= editorObjects.GetRuntimeObject(top) as WD_Top;
        WD_StateChart rtStateChart= editorObjects.GetRuntimeObject(stateChart) as WD_StateChart;
        rtTop.Action= rtStateChart;
        rootNode.LateUpdateTop= rtTop;
        // Add initial state.
        WD_EditorObject state= editorObjects.CreateInstance<WD_State>("", stateChart.InstanceId, context.GraphPosition);
        WD_State rtState= editorObjects.GetRuntimeObject(state) as WD_State;
        rtStateChart.EntryState= rtState;

        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add LateUpdate State Chart", true)]
    public static bool ValidateAddLateUpdateStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        if(rootNode == null || rootNode.LateUpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add FixedUpdate State Chart")]
    public static void AddFixedUpdateStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;

        WD_EditorObject top= editorObjects.CreateInstance<WD_Top>("FixedUpdate", rootNode.InstanceId, context.GraphPosition);
        WD_EditorObject stateChart= editorObjects.CreateInstance<WD_StateChart>("FixedUpdate", top.InstanceId, context.GraphPosition);
        WD_Top rtTop= editorObjects.GetRuntimeObject(top) as WD_Top;
        WD_StateChart rtStateChart= editorObjects.GetRuntimeObject(stateChart) as WD_StateChart;
        rtTop.Action= rtStateChart;
        rootNode.FixedUpdateTop= rtTop;
        // Add initial state.
        WD_EditorObject state= editorObjects.CreateInstance<WD_State>("", stateChart.InstanceId, context.GraphPosition);
        WD_State rtState= editorObjects.GetRuntimeObject(state) as WD_State;
        rtStateChart.EntryState= rtState;

        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add FixedUpdate State Chart", true)]
    public static bool ValidateAddFixedUpdateStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        if(rootNode == null || rootNode.FixedUpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add Update Module")]
    public static void AddUpdateModule(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;

        WD_EditorObject top= editorObjects.CreateInstance<WD_Top>("Update", rootNode.InstanceId, context.GraphPosition);
        WD_EditorObject module= editorObjects.CreateInstance<WD_Module>("Update", top.InstanceId, context.GraphPosition);
        WD_Top rtTop= editorObjects.GetRuntimeObject(top) as WD_Top;
        WD_Module rtModule= editorObjects.GetRuntimeObject(module) as WD_Module;
        rtTop.Action= rtModule;
        rootNode.UpdateTop= rtTop;

        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add Update Module", true)]
    public static bool ValidateAddUpdatefunction(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        if(rootNode == null || rootNode.UpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add LateUpdate Module")]
    public static void AddLateUpdateModule(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;

        WD_EditorObject top= editorObjects.CreateInstance<WD_Top>("LateUpdate", rootNode.InstanceId, context.GraphPosition);
        WD_EditorObject module= editorObjects.CreateInstance<WD_Module>("LateUpdate", top.InstanceId, context.GraphPosition);
        WD_Top rtTop= editorObjects.GetRuntimeObject(top) as WD_Top;
        WD_Module rtModule= editorObjects.GetRuntimeObject(module) as WD_Module;
        rtTop.Action= rtModule;
        rootNode.LateUpdateTop= rtTop;

        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add LateUpdate Module", true)]
    public static bool ValidateAddLateUpdateModule(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        if(rootNode == null || rootNode.LateUpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add FixedUpdate Module")]
    public static void AddFixedUpdateModule(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        WD_EditorObjectMgr editorObjects= context.Graph.EditorObjects;

        WD_EditorObject top= editorObjects.CreateInstance<WD_Top>("LateUpdate", rootNode.InstanceId, context.GraphPosition);
        WD_EditorObject module= editorObjects.CreateInstance<WD_Module>("LateUpdate", top.InstanceId, context.GraphPosition);
        WD_Top rtTop= editorObjects.GetRuntimeObject(top) as WD_Top;
        WD_Module rtModule= editorObjects.GetRuntimeObject(module) as WD_Module;
        rtTop.Action= rtModule;
        rootNode.FixedUpdateTop= rtTop;

        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add FixedUpdate Module", true)]
    public static bool ValidateAddFixedUpdateModule(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        if(rootNode == null || rootNode.FixedUpdateTop != null) return false;
        return true;
    }

}
