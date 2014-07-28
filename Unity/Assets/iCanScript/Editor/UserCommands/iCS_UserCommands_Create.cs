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
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        iCS_EditorObject package= _CreatePackage(parent, globalPos, name, objectType, runtimeType);
        if(package == null) return null;
        CloseTransaction(iStorage, "Create "+name);
        return package;
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateStateChart(iCS_EditorObject parent, Vector2 globalPos, string name) {
#if DEBUG
        Debug.Log("iCanScript: Create State Chart => "+name);
#endif
        if(parent == null) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        iCS_EditorObject stateChart= null;
        iStorage.AnimateGraph(null,
            _=> {
                stateChart= iStorage.CreateStateChart(parent.InstanceId, name);
                stateChart.SetInitialPosition(globalPos);
                // Automatically create entry state.
                var entryState= iStorage.CreateState(stateChart.InstanceId, "EntryState");
                entryState.IsEntryState= true;
                entryState.SetInitialPosition(globalPos);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Create StateChart");
        return stateChart;        
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateState(iCS_EditorObject parent, Vector2 globalPos, string name) {
#if DEBUG
        Debug.Log("iCanScript: Create State => "+name);
#endif
        if(parent == null) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        iCS_EditorObject state= null;
        iStorage.AnimateGraph(null,
            _=> {
                state= iStorage.CreateState(parent.InstanceId, name);
                state.SetInitialPosition(globalPos);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Create State");
        return state;        
    }
	// ----------------------------------------------------------------------
	// OK
    public static iCS_EditorObject CreateMessageHandler(iCS_EditorObject parent, Vector2 globalPos, iCS_MessageInfo desc) {
#if DEBUG
        Debug.Log("iCanScript: Create Message Handler => "+desc.DisplayName);
#endif
        if(parent == null) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= parent.IStorage;
        var name= desc.DisplayName;
        if(!iCS_AllowedChildren.CanAddChildNode(name, iCS_ObjectTypeEnum.InstanceMessage, parent, iStorage)) {
            return null;
        }
        OpenTransaction(iStorage);
        iCS_EditorObject msgHandler= null;
        iStorage.AnimateGraph(null,
            _=> {
                msgHandler= iStorage.CreateMessageHandler(parent.InstanceId, desc);
                msgHandler.SetInitialPosition(globalPos);
                msgHandler.ForEachChildPort(p=> {p.AnimationStartRect= BuildRect(globalPos, Vector2.zero);});
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Create "+name);
        return msgHandler;
    }
    // ----------------------------------------------------------------------
    public static iCS_EditorObject CreateUserMessageHandler(iCS_EditorObject parent, Vector2 globalPos) {
        return CreatePackage(parent, globalPos, "UserFunction");    
    }
	// ----------------------------------------------------------------------
	// OK
    public static iCS_EditorObject CreateOnEntryPackage(iCS_EditorObject parent, Vector2 globalPos) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        var package= _CreatePackage(parent, globalPos, iCS_Strings.OnEntry, iCS_ObjectTypeEnum.OnStateEntry);
        if(package == null) {
            CloseTransaction(iStorage, "Create "+package.Name);
            return null;
        }
        package.IsNameEditable= false;
        package.Tooltip= iCS_ObjectTooltips.OnEntry;            
        CloseTransaction(iStorage, "Create "+package.Name);
        return package;
    }
	// ----------------------------------------------------------------------
	// OK
	public static iCS_EditorObject CreateOnUpdatePackage(iCS_EditorObject parent, Vector2 globalPos) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        var package= _CreatePackage(parent, globalPos, iCS_Strings.OnUpdate, iCS_ObjectTypeEnum.OnStateUpdate);
        if(package == null) {
            CloseTransaction(iStorage, "Create "+package.Name);
            return null;
        }
        package.IsNameEditable= false;            
        package.Tooltip= iCS_ObjectTooltips.OnUpdate;            
        CloseTransaction(iStorage, "Create "+package.Name);
        return package;
    }
	// ----------------------------------------------------------------------
    // OK
	public static iCS_EditorObject CreateOnExitPackage(iCS_EditorObject parent, Vector2 globalPos) {
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        var package= _CreatePackage(parent, globalPos, iCS_Strings.OnExit, iCS_ObjectTypeEnum.OnStateExit);
        if(package == null) {
            CloseTransaction(iStorage, "Create "+package.Name);
            return null;
        }
        package.IsNameEditable= false;            
        package.Tooltip= iCS_ObjectTooltips.OnExit;            
        CloseTransaction(iStorage, "Create "+package.Name);
        return package;
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateFunction(iCS_EditorObject parent, Vector2 globalPos, iCS_MethodBaseInfo desc) {
#if DEBUG
        Debug.Log("iCanScript: Create Function => "+desc.DisplayName);
#endif
        if(parent == null || desc == null) return null;
        if(!IsCreationAllowed()) return null;
		var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
		var name= desc.DisplayName;
        iCS_EditorObject function= null;
        iStorage.AnimateGraph(null,
            _=> {
                function= iStorage.CreateFunction(parent.InstanceId, desc);
                function.SetInitialPosition(globalPos);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Create "+name);
        return function;        
    }

	// ----------------------------------------------------------------------
    public static iCS_EditorObject CreateTransition(iCS_EditorObject fromStatePort, iCS_EditorObject toState, Vector2 toStatePortPos) {
#if DEBUG
        Debug.Log("iCanScript: Create Transition Package");
#endif
        if(fromStatePort == null || toState == null) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= toState.IStorage;
        OpenTransaction(iStorage);
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
        var fromStatePortPos= fromStatePort.GlobalPosition;
        var globalPos= 0.5f*(fromStatePortPos+toStatePortPos);
        transitionPackage.SetInitialPosition(globalPos);
        transitionPackage.Iconize();
        // Attempt to proper edge for transition ports.
        var outTransitionPort= toStatePort.ProviderPort;
        var inTransitionPort= iStorage.GetInTransitionPort(transitionPackage);
        var diff= toStatePortPos-fromStatePortPos;
        if(Mathf.Abs(diff.x) > Mathf.Abs(diff.y)) {
            if(Vector2.Dot(diff, Vector2.right) > 0) {
                inTransitionPort.Edge= iCS_EdgeEnum.Left;
                toStatePort.Edge= iCS_EdgeEnum.Left;
                outTransitionPort.Edge= iCS_EdgeEnum.Right;
                fromStatePort.Edge= iCS_EdgeEnum.Right;
            }
            else {
                inTransitionPort.Edge= iCS_EdgeEnum.Right;
                toStatePort.Edge= iCS_EdgeEnum.Right;
                outTransitionPort.Edge= iCS_EdgeEnum.Left;
                fromStatePort.Edge= iCS_EdgeEnum.Left;
            }
        }
        else {
            if(Vector2.Dot(diff, Vector2.up) > 0) {
                inTransitionPort.Edge= iCS_EdgeEnum.Top;
                toStatePort.Edge= iCS_EdgeEnum.Top;
                outTransitionPort.Edge= iCS_EdgeEnum.Bottom; 
                fromStatePort.Edge= iCS_EdgeEnum.Bottom;
            }
            else {
                inTransitionPort.Edge= iCS_EdgeEnum.Bottom;
                toStatePort.Edge= iCS_EdgeEnum.Bottom;
                outTransitionPort.Edge= iCS_EdgeEnum.Top; 
                fromStatePort.Edge= iCS_EdgeEnum.Top;
            }            
        }
        inTransitionPort.PortPositionRatio= 0.5f;
        outTransitionPort.PortPositionRatio= 0.5f;
        // Layout the graph
        iStorage.ForcedRelayoutOfTree();
        CloseTransaction(iStorage, "Create Transition");            
        return transitionPackage;
    }
    // -------------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateEnablePort(iCS_EditorObject parent) {
        if(parent == null) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
		var port= iStorage.CreateEnablePort(parent.InstanceId);
        var pRect= parent.GlobalRect;
        port.SetInitialPosition(new Vector2(0.5f*(pRect.x+pRect.xMax), pRect.y));        
        parent.LayoutPorts();
        CloseTransaction(iStorage, "Create Enable Port");
        return port;
    }
    // -------------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateTriggerPort(iCS_EditorObject parent) {
        if(parent == null) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
		var port= iStorage.CreateTriggerPort(parent.InstanceId);        
        var pRect= parent.GlobalRect;
        port.SetInitialPosition(new Vector2(0.5f*(pRect.x+pRect.xMax), pRect.yMax));        
        parent.LayoutPorts();
        CloseTransaction(iStorage, "Create Trigger Port");
        return port;
    }
    // -------------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateOutInstancePort(iCS_EditorObject parent) {
        if(parent == null) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
		var port= iStorage.CreateOutInstancePort(parent.InstanceId, parent.RuntimeType);        
        parent.LayoutPorts();
        CloseTransaction(iStorage, "Create 'this' Port");
        return port;
    }
    

    // ======================================================================
    // Instance Object creation.
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject CreateObjectInstance(iCS_EditorObject parent, Vector2 globalPos, Type instanceType) {
        if(instanceType == null) return null;
        if(!IsCreationAllowed()) return null;
        instanceType= iCS_Types.RemoveRefOrPointer(instanceType);
#if DEBUG
        Debug.Log("iCanScript: Create Object Instance => "+instanceType.Name);
#endif
        if(parent == null) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        var name= instanceType.Name;
        iCS_EditorObject instance= null;
        iStorage.AnimateGraph(null,
            _=> {
                instance= iStorage.CreateObjectInstance(parent.InstanceId, name, instanceType);
                instance.SetInitialPosition(globalPos);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Create "+name);            
        return instance;
    }

    // ======================================================================
    // Node Wrapping in package.
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject WrapInPackage(iCS_EditorObject obj) {
        if(obj == null || !obj.CanHavePackageAsParent()) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= obj.IStorage;
        OpenTransaction(iStorage);
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.WrapInPackage(obj);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Wrap : "+obj.Name);
        return package;
    }
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject WrapMultiSelectionInPackage(iCS_IStorage iStorage) {
        if(iStorage == null) return null;
        if(!IsCreationAllowed()) return null;
        var selectedObjects= iStorage.FilterMultiSelectionForWrapInPackage();
        if(selectedObjects == null || selectedObjects.Length == 0) return null;
        OpenTransaction(iStorage);
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.WrapInPackage(selectedObjects);
                if(package != null) {
                    iStorage.ForcedRelayoutOfTree();
                    var r= Math3D.Union(P.map(n => n.GlobalRect, selectedObjects));
                    package.myAnimatedRect.StartValue= BuildRect(Math3D.Middle(r), Vector2.zero);                    
                }
                else {
                    Debug.LogWarning("iCanScript: Unable to create package.  Please report problem.");
                }
            }
        );
        CloseTransaction(iStorage, "Wrap Selection");
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
        if(!IsCreationAllowed()) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        iCS_EditorObject instance= null;
        iStorage.AnimateGraph(null,
            _=> {
                parent.Unfold();
                instance= iStorage.CreatePackage(parent.InstanceId, go.name, iCS_ObjectTypeEnum.Package, go.GetType());
                var thisPort= iStorage.InstanceWizardGetInputThisPort(instance);
                if(thisPort != null) {
                    thisPort.PortValue= go;
                }
                instance.SetInitialPosition(globalPos);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Create "+go.name);
        return instance;
    }
    
    
    // ======================================================================
    // Utilities
	// ----------------------------------------------------------------------
    private static iCS_EditorObject _CreatePackage(iCS_EditorObject parent, Vector2 globalPos, string name, iCS_ObjectTypeEnum objectType= iCS_ObjectTypeEnum.Package, Type runtimeType= null) {
#if DEBUG
        Debug.Log("iCanScript: CreatePackage => "+name);
#endif
        if(parent == null) return null;
        if(!IsCreationAllowed()) return null;
        var iStorage= parent.IStorage;
        OpenTransaction(iStorage);
        iCS_EditorObject package= null;
        iStorage.AnimateGraph(null,
            _=> {
                package= iStorage.CreatePackage(parent.InstanceId, name, objectType, runtimeType);
                package.SetInitialPosition(globalPos);
                iStorage.ForcedRelayoutOfTree();
            }
        );
        CloseTransaction(iStorage, "Create "+name);
        return package;
    }
}
