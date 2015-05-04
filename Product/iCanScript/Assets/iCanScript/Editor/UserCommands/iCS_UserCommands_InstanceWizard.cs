//
// File: iCS_UserCommands_InstanceWizard
//
//#define DEBUG
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using iCanScript;
using iCanScript.Editor;

public static partial class iCS_UserCommands {
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateInstanceWizardElement(iCS_EditorObject parent, iCS_FunctionPrototype desc) {
#if DEBUG
        Debug.Log("iCanScript: Create Instance Element => "+desc.DisplayName);
#endif
        if(parent == null) return null;
        var iStorage= parent.IStorage;

        iCS_EditorObject instance= null;
        OpenTransaction(iStorage);
        try {
            SendStartRelayoutOfTree(iStorage);
            iStorage.AnimateGraph(null,
                _=> {
                    instance= iStorage.InstanceWizardCreate(parent, desc);
                    instance.SetInitialPosition(parent.GlobalPosition);
                    instance.Iconize();
                    iStorage.ForcedRelayoutOfTree();
                }
            );                
            SendEndRelayoutOfTree(iStorage);            
        }
        catch(System.Exception) {
            instance= null;
        }
        if(instance == null) {
            CancelTransaction(iStorage);
            return null;
        }
        CloseTransaction(iStorage, "Create "+desc.DisplayName);            
        return instance;
    }
	// ----------------------------------------------------------------------
    public static void DeleteInstanceWizardElement(iCS_EditorObject parent, iCS_FunctionPrototype desc) {
#if DEBUG
        Debug.Log("iCanScript: Delete Instance Element => "+desc.DisplayName);
#endif
        if(parent == null || desc == null) return;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        try {
            SendStartRelayoutOfTree(iStorage);
            iStorage.AnimateGraph(null,
                _=> {
                    iStorage.InstanceWizardDestroy(parent, desc);
                    iStorage.ForcedRelayoutOfTree();
                }
            );                
            SendEndRelayoutOfTree(iStorage);            
        }
        catch(System.Exception) {
            CancelTransaction(iStorage);
            return;
        }
        CloseTransaction(iStorage, "Delete "+desc.DisplayName);            
    }
 	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateInstanceObjectAndElement(iCS_EditorObject parent, Vector2 globalPos, Type instanceType, iCS_FunctionPrototype desc) {
        if(instanceType == null) return null;
        instanceType= iCS_Types.RemoveRefOrPointer(instanceType);
#if DEBUG
        Debug.Log("iCanScript: Create Instance Object & Element => "+desc.DisplayName);
#endif
        if(parent == null || instanceType == null || desc == null) return null;
        var iStorage= parent.IStorage;

        iCS_EditorObject element= null;
        OpenTransaction(iStorage);
        try {
            SendStartRelayoutOfTree(iStorage);
            iStorage.AnimateGraph(null,
                _=> {
                    var instance= iStorage.CreateObjectInstance(parent.InstanceId, instanceType.Name, instanceType);
                    instance.SetInitialPosition(globalPos);
                    element= iStorage.InstanceWizardCreate(instance, desc);
                    element.SetInitialPosition(globalPos);
                    element.Iconize();
                    iStorage.ForcedRelayoutOfTree();
                }
            );
            SendEndRelayoutOfTree(iStorage);            
        }
        catch(System.Exception) {
            element= null;
        }
        if(element == null) {
            CancelTransaction(iStorage);
            return null;
        }
        CloseTransaction(iStorage, "Create "+desc.DisplayName);            
        return element;
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateInstanceBuilderAndObjectAndElement(iCS_EditorObject parent, Vector2 globalPos, Type instanceType, iCS_FunctionPrototype desc) {
        if(instanceType == null) return null;
        instanceType= iCS_Types.RemoveRefOrPointer(instanceType);
#if DEBUG
        Debug.Log("iCanScript: Create Builder, Instance Object & Element => "+desc.DisplayName);
#endif
        if(parent == null || instanceType == null || desc == null) return null;
        var iStorage= parent.IStorage;

        iCS_EditorObject element= null;
        OpenTransaction(iStorage);
        try {
            iStorage.AnimateGraph(null,
                _=> {
                    // Object Instance
                    var instance= iStorage.CreateObjectInstance(parent.InstanceId, instanceType.Name, instanceType);
                    instance.SetInitialPosition(globalPos);
                    // Internal element
                    element= iStorage.InstanceWizardCreate(instance, desc);
                    element.SetInitialPosition(globalPos);
                    element.Iconize();
                    // Object Builder
    		        iCS_ConstructorInfo[] myConstructors= iCS_LibraryDatabase.GetConstructors(desc.ClassType);
    		    	Array.Sort(myConstructors, (x,y)=> x.FunctionSignatureNoThis.CompareTo(y.FunctionSignatureNoThis));
    				if(myConstructors.Length != 0) {
    					iCS_EditorObject builder= null;
    					if(myConstructors.Length == 1) {
    						builder= iStorage.InstanceWizardCreateConstructor(instance, myConstructors[0]);								
    						builder.SetInitialPosition(new Vector2(globalPos.x-75f, globalPos.y));
    					}
    					else {
    						/*
    							TODO : Support Multiple Instance Builders on drag port quick menu. 
    						*/
                            var visualEditor= iCS_EditorController.FindVisualEditor();
                            if(visualEditor != null) {
                                visualEditor.ShowNotification(new GUIContent("Multiple Builders exists.  Please create the builder manually."));
                            }
    						Debug.LogWarning("iCanScript: Multiple Builders exists.  Please create the builder manually.");								
    					}
					
    				}         
                    // Layout
                    iStorage.ForcedRelayoutOfTree();
                }
            );            
        }
        catch(System.Exception) {
            element= null;
        }
        if(element == null) {
            CancelTransaction(iStorage);
            return null;
        }
        CloseTransaction(iStorage, "Create "+desc.DisplayName);            
        return element;
    }
}
