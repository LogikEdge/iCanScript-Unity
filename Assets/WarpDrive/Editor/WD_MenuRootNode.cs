using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuRootNode {
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/RootNode/Add Update State Chart")]
    public static void AddUpdateStateChart(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_RootNode rootNode= context.SelectedObject as WD_RootNode;
        WD_Top top= WD_Top.CreateInstance("Update", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        WD_StateChart stateChart= WD_StateChart.CreateInstance<WD_StateChart>("Update", top);
        top.Action= stateChart;
        rootNode.UpdateTop= top;
        // Add initial state.
        WD_State state= WD_State.CreateInstance<WD_State>("", stateChart);
        stateChart.EntryState= state;
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
        WD_Top top= WD_Top.CreateInstance("LateUpdate", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        WD_StateChart stateChart= WD_StateChart.CreateInstance<WD_StateChart>("LateUpdate", top);
        top.Action= stateChart;
        rootNode.LateUpdateTop= top;
        // Add initial state.
        WD_State state= WD_State.CreateInstance<WD_State>("", stateChart);
        stateChart.EntryState= state;        
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
        WD_Top top= WD_Top.CreateInstance("FixedUpdate", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        WD_StateChart stateChart= WD_StateChart.CreateInstance<WD_StateChart>("FixedUpdate", top);
        top.Action= stateChart;
        rootNode.FixedUpdateTop= top;
        // Add initial state.
        WD_State state= WD_State.CreateInstance<WD_State>("", stateChart);
        stateChart.EntryState= state;        
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
        WD_Top top= WD_Top.CreateInstance("Update", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        WD_Module module= WD_Module.CreateInstance<WD_Module>("Update", top);
        top.Action= module;
        rootNode.UpdateTop= top;
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
        WD_Top top= WD_Top.CreateInstance("LateUpdate", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        WD_Module module= WD_Module.CreateInstance<WD_Module>("LateUpdate", top);
        top.Action= module;
        rootNode.LateUpdateTop= top;
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
        WD_Top top= WD_Top.CreateInstance("FixedUpdate", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        WD_Module module= WD_Module.CreateInstance<WD_Module>("FixedUpdate", top);
        top.Action= module;
        rootNode.FixedUpdateTop= top;
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
