//
// File: iCS_UserCommands_Create
//
//#define DEBUG
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using P=Prelude;

public static partial class iCS_UserCommands {
    // ======================================================================
    // Object creation
	// ----------------------------------------------------------------------
    public static iCS_EditorObject CreatePackage(iCS_EditorObject parent, Vector2 globalPos, string name, iCS_ObjectTypeEnum objectType= iCS_ObjectTypeEnum.Package, Type runtimeType= null) {
#if DEBUG
        Debug.Log("iCanScript: CreatePackage => "+name);
#endif
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create "+name);
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.CreatePackage(parent.InstanceId, name, objectType, runtimeType);
                package.SetInitialPosition(globalPos);
                package.LayoutNodeAndParents();
            }
        );
        return package;
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateStateChart(iCS_EditorObject parent, Vector2 globalPos, string name) {
#if DEBUG
        Debug.Log("iCanScript: Create State Chart => "+name);
#endif
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create StateChart");
        iCS_EditorObject stateChart= null;
        iStorage.AnimateGraph(null,
            _=> {
                stateChart= iStorage.CreateStateChart(parent.InstanceId, name);
                stateChart.SetInitialPosition(globalPos);
                // Automatically create entry state.
                var entryState= iStorage.CreateState(stateChart.InstanceId, "EntryState");
                entryState.IsEntryState= true;
                entryState.SetInitialPosition(globalPos);
                entryState.LayoutNodeAndParents();
            }
        );
        return stateChart;        
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateState(iCS_EditorObject parent, Vector2 globalPos, string name) {
#if DEBUG
        Debug.Log("iCanScript: Create State => "+name);
#endif
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create State");
        iCS_EditorObject state= null;
        iStorage.AnimateGraph(null,
            _=> {
                state= iStorage.CreateState(parent.InstanceId, name);
                state.SetInitialPosition(globalPos);
                state.LayoutNodeAndParents();                
            }
        );
        return state;        
    }
	// ----------------------------------------------------------------------
	// OK
    public static iCS_EditorObject CreateMessageHandler(iCS_EditorObject parent, Vector2 globalPos, iCS_MessageInfo desc) {
#if DEBUG
        Debug.Log("iCanScript: Create Message Handler => "+desc.DisplayName);
#endif
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        var name= desc.DisplayName;
        if(!iCS_AllowedChildren.CanAddChildNode(name, iCS_ObjectTypeEnum.InstanceMessage, parent, iStorage)) {
            return null;
        }
        iStorage.RegisterUndo("Create "+name);
        iCS_EditorObject msgHandler= null;
        iStorage.AnimateGraph(null,
            _=> {
                msgHandler= iStorage.CreateMessageHandler(parent.InstanceId, desc);
                msgHandler.SetInitialPosition(globalPos);
                msgHandler.ForEachChildPort(p=> {p.AnimationStart= BuildRect(globalPos, Vector2.zero);});
                msgHandler.LayoutNodeAndParents(); 
//                msgHandler.LayoutPorts();               
            }
        );
        return msgHandler;
    }
	// ----------------------------------------------------------------------
	// OK
    public static iCS_EditorObject CreateOnEntryPackage(iCS_EditorObject parent, Vector2 globalPos) {
        var package= CreatePackage(parent, globalPos, iCS_Strings.OnEntry, iCS_ObjectTypeEnum.OnStateEntry);
        if(package != null) {
            package.IsNameEditable= false;
            package.Tooltip= iCS_ObjectTooltips.OnEntry;            
        }
        return package;
    }
	// ----------------------------------------------------------------------
	// OK
	public static iCS_EditorObject CreateOnUpdatePackage(iCS_EditorObject parent, Vector2 globalPos) {
        var package= CreatePackage(parent, globalPos, iCS_Strings.OnUpdate, iCS_ObjectTypeEnum.OnStateUpdate);
        if(package != null) {
            package.IsNameEditable= false;            
            package.Tooltip= iCS_ObjectTooltips.OnUpdate;            
        }
        return package;
    }
	// ----------------------------------------------------------------------
    // OK
	public static iCS_EditorObject CreateOnExitPackage(iCS_EditorObject parent, Vector2 globalPos) {
        var package= CreatePackage(parent, globalPos, iCS_Strings.OnExit, iCS_ObjectTypeEnum.OnStateExit);
        if(package != null) {
            package.IsNameEditable= false;            
            package.Tooltip= iCS_ObjectTooltips.OnExit;            
        }
        return package;
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateFunction(iCS_EditorObject parent, Vector2 globalPos, iCS_MethodBaseInfo desc) {
#if DEBUG
        Debug.Log("iCanScript: Create Function => "+desc.DisplayName);
#endif
        if(parent == null || desc == null) return null;
		var name= desc.DisplayName;
		var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create "+name);
        iCS_EditorObject function= null;
        iStorage.AnimateGraph(null,
            _=> {
                function= iStorage.CreateFunction(parent.InstanceId, desc);
                function.SetInitialPosition(globalPos);
                function.LayoutNodeAndParents();                
            }
        );
        return function;        
    }

	// ----------------------------------------------------------------------
    public static iCS_EditorObject CreateTransition(iCS_EditorObject fromStatePort, iCS_EditorObject toState, Vector2 toStatePortPos) {
#if DEBUG
        Debug.Log("iCanScript: Create Transition Package");
#endif
        if(fromStatePort == null || toState == null) return null;
        var iStorage= toState.IStorage;
        iStorage.RegisterUndo("Create Transition");
        // Create toStatePort
        iCS_EditorObject toStatePort= iStorage.CreatePort("", toState.InstanceId, typeof(void), iCS_ObjectTypeEnum.InStatePort);
        // Update port positions
        toStatePort.SetInitialPosition(toStatePortPos);
        toStatePort.UpdatePortEdge();        
        fromStatePort.UpdatePortEdge();
        // Temporally connect state ports together.
        iStorage.SetSource(toStatePort, fromStatePort);
        // Create transition package
        var transitionPackage= iStorage.CreateTransition(fromStatePort, toStatePort);
        // Try to position the transition in the middle
        var fromStatePortPos= fromStatePort.LayoutPosition;
        var globalPos= 0.5f*(fromStatePortPos+toStatePortPos);
        transitionPackage.SetInitialPosition(globalPos);
        transitionPackage.Iconize();
        transitionPackage.LayoutNodeAndParents();
        return transitionPackage;
    }
    // -------------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateEnablePort(iCS_EditorObject parent) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create Enable Port");
		var port= iStorage.CreateEnablePort(parent.InstanceId);
        var pRect= parent.LayoutRect;
        port.SetInitialPosition(new Vector2(0.5f*(pRect.x+pRect.xMax), pRect.y));        
        parent.LayoutPorts();
        return port;
    }
    // -------------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateTriggerPort(iCS_EditorObject parent) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create Trigger Port");
		var port= iStorage.CreateTriggerPort(parent.InstanceId);        
        var pRect= parent.LayoutRect;
        port.SetInitialPosition(new Vector2(0.5f*(pRect.x+pRect.xMax), pRect.yMax));        
        parent.LayoutPorts();
        return port;
    }
    // -------------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateOutInstancePort(iCS_EditorObject parent) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create 'this' Port");
		var port= iStorage.CreateOutInstancePort(parent.InstanceId, parent.RuntimeType);        
        parent.LayoutPorts();
        return port;
    }
    

    // ======================================================================
    // Instance Object creation.
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateObjectInstance(iCS_EditorObject parent, Vector2 globalPos, Type instanceType) {
        if(instanceType == null) return null;
        instanceType= iCS_Types.RemoveRefOrPointer(instanceType);
#if DEBUG
        Debug.Log("iCanScript: Create Object Instance => "+instanceType.Name);
#endif
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        var name= instanceType.Name;
        iStorage.RegisterUndo("Create "+name);
        iCS_EditorObject instance= null;
        iStorage.AnimateGraph(null,
            _=> {
                instance= iStorage.CreateObjectInstance(parent.InstanceId, name, instanceType);
                instance.SetInitialPosition(globalPos);
                instance.LayoutNodeAndParents();                
            }
        );
        return instance;
    }

    // ======================================================================
    // Node Wrapping in package.
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject WrapInPackage(iCS_EditorObject obj) {
        if(obj == null || !obj.CanHavePackageAsParent()) return null;
        var iStorage= obj.IStorage;
        iStorage.RegisterUndo("Wrap : "+obj.Name);
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.WrapInPackage(obj);
                package.LayoutNodeAndParents();
            }
        );
        return package;
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject WrapMultiSelectionInPackage(iCS_IStorage iStorage) {
        if(iStorage == null) return null;
        var selectedObjects= iStorage.FilterMultiSelectionForWrapInPackage();
        if(selectedObjects == null || selectedObjects.Length == 0) return null;
        iStorage.RegisterUndo("Wrap Selection");
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.WrapInPackage(selectedObjects);
                package.LayoutNodeAndParents();
                var r= Math3D.Union(P.map(n => n.LayoutRect, selectedObjects));
                package.myAnimatedRect.StartValue= BuildRect(Math3D.Middle(r), Vector2.zero);
            }
        );
        return package;
    }

    // ======================================================================
    // Create from Drag & Drop
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateGameObject(GameObject go, iCS_EditorObject parent, Vector2 globalPos) {
#if DEBUG
        Debug.Log("iCanScript: Create Game Object => "+go.name);
#endif
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Create "+go.name);
        iCS_EditorObject instance= null;
        iStorage.AnimateGraph(null,
            _=> {
                instance= iStorage.CreatePackage(parent.InstanceId, go.name, iCS_ObjectTypeEnum.Package, go.GetType());
                var thisPort= iStorage.InstanceWizardGetInputThisPort(instance);
                if(thisPort != null) {
                    thisPort.PortValue= go;
                }
                instance.SetInitialPosition(globalPos);
                instance.LayoutNodeAndParents();    
            }
        );
        return instance;
    }
}
