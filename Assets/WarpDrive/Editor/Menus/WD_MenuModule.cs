//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//
//public class WD_MenuModule {
//    // ---------------------------------------------------------------------
//    [MenuItem("CONTEXT/WarpDrive/Module/Add State Chart")]
//    public static void AddStateChart(MenuCommand command) {
//        WD_MenuContext context= command.context as WD_MenuContext;
//        WD_EditorObject module= context.SelectedObject;
//        WD_EditorObjectMgr editorObjects= context.EditorObjects;
//        WD_EditorObject stateChart= editorObjects.CreateInstance<WD_StateChart>("", module.InstanceId, context.GraphPosition);
//        // Add initial state.
//        WD_EditorObject state= editorObjects.CreateInstance<WD_State>("", stateChart.InstanceId, context.GraphPosition);
//        WD_StateChart rtStateChart= editorObjects.GetRuntimeObject(stateChart) as WD_StateChart;
//        WD_State rtState= editorObjects.GetRuntimeObject(state) as WD_State;
//        rtStateChart.EntryState= rtState;        
//        WD_MenuContext.DestroyImmediate(context);
//    }
//    [MenuItem("CONTEXT/WarpDrive/Module/Add State Chart", true)]
//    public static bool ValidateAddStateChart(MenuCommand command) {
//        WD_MenuContext context= command.context as WD_MenuContext;
//        return context.SelectedObject.IsRuntimeA<WD_Module>();
//    }
//    // ----------------------------------------------------------------------
//    [MenuItem("CONTEXT/WarpDrive/Module/Delete")]
//    public static void DeleteObject(MenuCommand command) {    
//        WD_MenuContext context= command.context as WD_MenuContext;
//        WD_EditorObject module= context.SelectedObject;
//        if(EditorUtility.DisplayDialog("Deleting Module", "Are you sure you want to delete module: "+module.NameOrTypeName+"?", "Delete", "Cancel")) {
//            context.EditorObjects.DestroyInstance(module);
//        }                
//        WD_MenuContext.DestroyImmediate(context);
//    }
//    [MenuItem("CONTEXT/WarpDrive/Module/Delete",true)]
//    public static bool ValidateDeleteObject(MenuCommand command) {
//        WD_MenuContext context= command.context as WD_MenuContext;
//        return context.SelectedObject.IsRuntimeA<WD_Module>();
//    }
//
//}
//