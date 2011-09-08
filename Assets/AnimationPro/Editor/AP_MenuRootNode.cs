using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuRootNode {
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add Update State Chart")]
    public static void AddUpdateStateChart(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        AP_Top top= AP_Top.CreateInstance("Update", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        AP_StateChart stateChart= AP_StateChart.CreateInstance<AP_StateChart>("Update", top);
        top.Action= stateChart;
        rootNode.UpdateTop= top;
        // Add initial state.
        AP_State state= AP_State.CreateInstance<AP_State>("", stateChart);
        stateChart.EntryState= state;        
    }
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add Update State Chart", true)]
    public static bool ValidateAddUpdateStateChart(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        if(rootNode == null || rootNode.UpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add LateUpdate State Chart")]
    public static void AddLateUpdateStateChart(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        AP_Top top= AP_Top.CreateInstance("LateUpdate", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        AP_StateChart stateChart= AP_StateChart.CreateInstance<AP_StateChart>("LateUpdate", top);
        top.Action= stateChart;
        rootNode.LateUpdateTop= top;
        // Add initial state.
        AP_State state= AP_State.CreateInstance<AP_State>("", stateChart);
        stateChart.EntryState= state;        
    }
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add LateUpdate State Chart", true)]
    public static bool ValidateAddLateUpdateStateChart(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        if(rootNode == null || rootNode.LateUpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add FixedUpdate State Chart")]
    public static void AddFixedUpdateStateChart(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        AP_Top top= AP_Top.CreateInstance("FixedUpdate", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        AP_StateChart stateChart= AP_StateChart.CreateInstance<AP_StateChart>("FixedUpdate", top);
        top.Action= stateChart;
        rootNode.FixedUpdateTop= top;
        // Add initial state.
        AP_State state= AP_State.CreateInstance<AP_State>("", stateChart);
        stateChart.EntryState= state;        
    }
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add FixedUpdate State Chart", true)]
    public static bool ValidateAddFixedUpdateStateChart(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        if(rootNode == null || rootNode.FixedUpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add Update Module")]
    public static void AddUpdateModule(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        AP_Top top= AP_Top.CreateInstance("Update", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        AP_Module module= AP_Module.CreateInstance<AP_Module>("Update", top);
        top.Action= module;
        rootNode.UpdateTop= top;
    }
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add Update Module", true)]
    public static bool ValidateAddUpdatefunction(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        if(rootNode == null || rootNode.UpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add LateUpdate Module")]
    public static void AddLateUpdateModule(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        AP_Top top= AP_Top.CreateInstance("LateUpdate", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        AP_Module module= AP_Module.CreateInstance<AP_Module>("LateUpdate", top);
        top.Action= module;
        rootNode.LateUpdateTop= top;
    }
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add LateUpdate Module", true)]
    public static bool ValidateAddLateUpdateModule(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        if(rootNode == null || rootNode.LateUpdateTop != null) return false;
        return true;
    }
    // ---------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add FixedUpdate Module")]
    public static void AddFixedUpdateModule(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        AP_Top top= AP_Top.CreateInstance("FixedUpdate", rootNode);
        top.SetInitialPosition(context.GraphPosition);
        AP_Module module= AP_Module.CreateInstance<AP_Module>("FixedUpdate", top);
        top.Action= module;
        rootNode.FixedUpdateTop= top;
    }
    [MenuItem("CONTEXT/AnimationPro/RootNode/Add FixedUpdate Module", true)]
    public static bool ValidateAddFixedUpdateModule(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_RootNode rootNode= context.SelectedObject as AP_RootNode;
        if(rootNode == null || rootNode.FixedUpdateTop != null) return false;
        return true;
    }

}
