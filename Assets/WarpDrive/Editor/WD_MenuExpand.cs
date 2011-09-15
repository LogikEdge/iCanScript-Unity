using UnityEngine;
using UnityEditor;
using System.Collections;

public class WD_MenuExpand {

    // ----------------------------------------------------------------------
    [MenuItem("CONTEXT/WarpDrive/Expand")]
    public static void ExpandObject(MenuCommand command) {    
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Object selectedObject= context.SelectedObject as WD_Object;
        selectedObject.ExecuteIf<WD_Node>(
            (node)=> {
                node.ForEachChild<WD_Node>((child)=> { child.IsVisible= true; });
            }
        );
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Expand",true)]
    public static bool ValidateExpandObject(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Object selectedObject= context.SelectedObject as WD_Object;
        bool expansionAllowed= false;
        selectedObject.ExecuteIf<WD_Node>(
            (node)=> {
                node.ForEachChild<WD_Node>(
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
    [MenuItem("CONTEXT/WarpDrive/Fold")]
    public static void FoldObject(MenuCommand command) {    
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Object selectedObject= context.SelectedObject as WD_Object;
        selectedObject.Case<WD_Port, WD_Node>(
            (port)=> {
            },
            (node)=> {
                node.ForEachChild<WD_Node>((child)=> { child.IsVisible= false; });
            }
        );
        WD_MenuContext.DestroyImmediate(context);
    }
    [MenuItem("CONTEXT/WarpDrive/Fold",true)]
    public static bool ValidateFoldObject(MenuCommand command) {
        WD_MenuContext context= command.context as WD_MenuContext;
        WD_Object selectedObject= context.SelectedObject as WD_Object;
        bool foldAllowed= false;
        selectedObject.Case<WD_Port, WD_Node>(
            (port)=> {},
            (node)=> {
                node.ForEachChild<WD_Node>(
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
