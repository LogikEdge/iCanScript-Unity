using UnityEngine;
using System.Collections;

public partial class WD_IStorage {
    // ======================================================================
    // Interpretation
    // ----------------------------------------------------------------------
    public void GenerateDynamicCode() {
        // Only generate runtime code for behaviours.
        WD_Behaviour rtBehaviour= Storage as WD_Behaviour;
        if(rtBehaviour == null || EditorObjects.Count == 0) return;
        // Remove any previous runtime data.
        WD_EditorObject edBehaviour= EditorObjects[0];
        if(!edBehaviour.IsBehaviour) {
            Debug.LogError("Could not locate Behaviour object.  Aborting code generation.");
        }
        rtBehaviour.ClearGeneratedCode();
        GenerateDynamicNodeCode(edBehaviour, rtBehaviour);
    }
    void GenerateDynamicNodeCode(WD_EditorObject edNode, object rtNode) {
        ForEachChild(edNode,
            edChild=>{
                if(edChild.IsNode) {
                    object rtChild= null;
                    switch(edChild.ObjectType) {
                        case WD_ObjectTypeEnum.Module: {
                            rtChild= new WD_Module(edChild.Name);
                            break;
                        }
                        case WD_ObjectTypeEnum.StateChart: {
                            rtChild= new WD_StateChart(edChild.Name);
                            break;
                        }
                        case WD_ObjectTypeEnum.State: {
                            rtChild= new WD_State(edChild.Name);
                            break;
                        }
                        case WD_ObjectTypeEnum.Function: {
                            break;
                        }
                        case WD_ObjectTypeEnum.Conversion: {
                            break;
                        }
                        case WD_ObjectTypeEnum.Class: {
                            break;
                        }
                        default: {
                            Debug.LogWarning("Code could not be generated for "+edChild.ObjectType+" editor object type.");
                            break;
                        }
                    }
                    if(rtChild != null) {
                        WD_Reflection.InvokeAddChildIfExists(rtNode, rtChild);
                        GenerateDynamicNodeCode(edChild, rtChild);
                    }
                }
            }
        );
    }
}
