using UnityEngine;
using UnityEditor;
using System.Collections;

public class AP_MenuExpand {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/Expand")]
    public static void ExpandObject(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Object selectedObject= context.SelectedObject as AP_Object;
        selectedObject.ExecuteIf<AP_Node>(
            (node)=> {
                node.ForEachChild<AP_Node>((child)=> { child.IsVisible= true; });
            }
        );
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/Expand",true)]
    public static bool ValidateExpandObject(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Object selectedObject= context.SelectedObject as AP_Object;
        bool expansionAllowed= false;
        selectedObject.ExecuteIf<AP_Node>(
            (node)=> {
                node.ForEachChild<AP_Node>(
                    (child)=> {
                        if(!child.IsVisible) {
                            expansionAllowed= true;
                        }
                    }
                );                                
            }
        );
        return expansionAllowed;
    }
    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/AnimationPro/Edit/Fold")]
    public static void FoldObject(MenuCommand command) {    
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Object selectedObject= context.SelectedObject as AP_Object;
        selectedObject.Case<AP_Port, AP_Node>(
            (port)=> {
            },
            (node)=> {
                node.ForEachChild<AP_Node>((child)=> { child.IsVisible= false; });
            }
        );
    }
    [MenuItem("CONTEXT/AnimationPro/Edit/Fold",true)]
    public static bool ValidateFoldObject(MenuCommand command) {
        AP_MenuContext context= command.context as AP_MenuContext;
        AP_Object selectedObject= context.SelectedObject as AP_Object;
        bool foldAllowed= false;
        selectedObject.Case<AP_Port, AP_Node>(
            (port)=> {},
            (node)=> {
                node.ForEachChild<AP_Node>(
                    (child)=> {
                        if(child.IsVisible) {
                            foldAllowed= true;
                        }
                    }
                );                
            }
        );
        return foldAllowed;
    }

}
