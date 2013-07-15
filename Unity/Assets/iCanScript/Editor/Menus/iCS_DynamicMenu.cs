using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_DynamicMenu {
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    Vector2 				GraphPosition  = Vector2.zero;
	List<iCS_MenuContext>	GUICommandQueue= new List<iCS_MenuContext>();
    
    // ======================================================================
    // Menu Items
	// ----------------------------------------------------------------------
    const string ShowHierarchyStr= "Show in hierarchy";
    const string DeleteStr= "- Delete";
    const string ModuleStr= "+ Module";
    const string StateChartStr= "+ State Chart";
    const string StateStr= "+ State";
    const string EntryStateStr= "+ Entry State";
    const string SetAsEntryStr="Set as Entry";
    const string OnEntryStr= "+ "+iCS_Strings.OnEntry;
    const string OnUpdateStr= "+ "+iCS_Strings.OnUpdate;
    const string OnExitStr= "+ "+iCS_Strings.OnExit;
    const string PublishPortStr= "Publish on Module";
    const string EnablePortStr= "+ Enable Port";
    const string SeparatorStr= "";

    // ======================================================================
    // Menu state management
	// ----------------------------------------------------------------------
    void Reset() {
        GraphPosition= Vector2.zero;
    }
    
	// ----------------------------------------------------------------------
	public void OnGUI() {
		GUICommandQueue.ForEach(c=> ProcessMenu(c));
		GUICommandQueue.Clear();
	}
	// ----------------------------------------------------------------------
    public void Update(iCS_EditorObject selectedObject, iCS_IStorage storage, Vector2 graphPosition) {
        // Update mouse position if not already done.
        if(GraphPosition == Vector2.zero) GraphPosition= graphPosition;

        // Nothing to show if menu is inactive.
        if(selectedObject == null || GraphPosition != graphPosition) {
            Reset();
            return;
        }
        
        // Process the menu state.
        switch(selectedObject.ObjectType) {
            case iCS_ObjectTypeEnum.Behaviour:        BehaviourMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.StateChart:       StateChartMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.State:            StateMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.Package:          AggregateMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceMessage:  AggregateMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassMessage:     AggregateMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TransitionGuard:  AggregateMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TransitionAction: AggregateMenu(selectedObject, storage); break;
			case iCS_ObjectTypeEnum.TransitionModule: TransitionModuleMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.Constructor:      FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceFunction: FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassFunction:    FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TypeCast:         FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceField:    FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassField:       FunctionMenu(selectedObject, storage); break;
            default: if(selectedObject.IsPort)        PortMenu(selectedObject, storage); break;
        }
    }

	// ----------------------------------------------------------------------
    void BehaviourMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        // Don't show any menu if behaviour not visible.
        if(selectedObject.IsIconizedOnDisplay || selectedObject.IsFoldedOnDisplay) return;

		var messages= iCS_LibraryDatabase.GetMessages(typeof(MonoBehaviour));
        int len= messages.Length;
		iCS_MenuContext[] menu= new iCS_MenuContext[len];
        for(int i= 0; i < len; ++i) {
			var message= messages[i];
            string name= message.DisplayName;
            if(iCS_AllowedChildren.CanAddChildNode(name, message.ObjectType, selectedObject, storage)) {
                menu[i]= new iCS_MenuContext(String.Concat("+ ", name), message);
            } else {
                menu[i]= new iCS_MenuContext(String.Concat("#+ ", name), message);
            }
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void AggregateMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            // Base menu items
            menu= new iCS_MenuContext[2];
            menu[0]= new iCS_MenuContext(ModuleStr);
            menu[1]= new iCS_MenuContext(StateChartStr); 
        }
        // Show in hierarchy
        AddShowInHierarchyMenuItem(ref menu);
        // Delete menu item
        if(selectedObject.InstanceId != 0 && selectedObject.ObjectType != iCS_ObjectTypeEnum.TransitionGuard && selectedObject.ObjectType != iCS_ObjectTypeEnum.TransitionAction) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void TransitionModuleMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[1];
        menu[0]= new iCS_MenuContext(ShowHierarchyStr);
        AddDeleteMenuItem(ref menu);
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateChartMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            menu= new iCS_MenuContext[1];
            menu[0]= new iCS_MenuContext(StateStr); 
        }
        AddShowInHierarchyMenuItem(ref menu);
        if(selectedObject.InstanceId != 0) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu;
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            int len= iCS_AllowedChildren.StateChildNames.Length;
            menu= new iCS_MenuContext[len+4];
            menu[0]= new iCS_MenuContext(StateStr);
            menu[1]= new iCS_MenuContext(SeparatorStr);
            for(int i= 0; i < len; ++i) {
                string name= iCS_AllowedChildren.StateChildNames[i];
                if(iCS_AllowedChildren.CanAddChildNode(name, iCS_ObjectTypeEnum.Package, selectedObject, storage)) {
                    menu[i+2]= new iCS_MenuContext(String.Concat("+ ", name));
                } else {
                    menu[i+2]= new iCS_MenuContext(String.Concat("#+ ", name));
                }
            }
            menu[len+2]= new iCS_MenuContext(SeparatorStr);
            if(selectedObject.IsEntryState) {
                menu[len+3]= new iCS_MenuContext(String.Concat("#", SetAsEntryStr));
            } else {
                menu[len+3]= new iCS_MenuContext(SetAsEntryStr);
            }
        } else {
            menu= new iCS_MenuContext[0];
        }
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
        ShowMenu(menu, selectedObject, storage);
    }
    
	// ----------------------------------------------------------------------
    void FunctionMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
		iCS_MenuContext[] menu= new iCS_MenuContext[1];
		menu[0]= new iCS_MenuContext(ShowHierarchyStr);
        if(storage.EditorObjects[selectedObject.ParentId].IsKindOfPackage) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, selectedObject, storage);            
    }
	// ----------------------------------------------------------------------
    void PortMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
		/*
			TODO : Add use as library filter option.
		*/
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        // Allow to publish port if the grand-parent is a module.
        iCS_EditorObject parent= storage.EditorObjects[selectedObject.ParentId];
        iCS_EditorObject grandParent= storage.EditorObjects[parent.ParentId];
        if(grandParent != null && grandParent.IsKindOfPackage) {
            if(!(selectedObject.IsInputPort && selectedObject.IsSourceValid)) {
                menu= new iCS_MenuContext[1];
				menu[0]= new iCS_MenuContext(PublishPortStr);                
            }
        }
        // Get compatible functions.
        if(selectedObject.IsDataPort) {
            List<iCS_MethodBaseInfo> functionMenu= null;
            if(selectedObject.IsInputPort) {
                functionMenu= iCS_LibraryDatabase.BuildMenu(null, selectedObject.RuntimeType);
            } else {
                functionMenu= iCS_LibraryDatabase.BuildMenu(selectedObject.RuntimeType, null);
            }
            if(functionMenu.Count != 0) {
                int len= menu.Length;
                iCS_MenuContext[] tmp= null;
                if(len == 0) {
                    tmp= new iCS_MenuContext[functionMenu.Count];
                } else {
                    tmp= new iCS_MenuContext[len+1+functionMenu.Count];
                    menu.CopyTo(tmp, 0);
                    tmp[len]= new iCS_MenuContext(SeparatorStr);
                    ++len;                    
                }
                menu= tmp;                
                for(int i= 0; i < functionMenu.Count; ++i) {
                    menu[len+i]= new iCS_MenuContext(functionMenu[i].ToString(), functionMenu[i]);
                }
            }
        }
        if(menu.Length == 0) {
            menu= new iCS_MenuContext[1];
            menu[0]= new iCS_MenuContext(ShowHierarchyStr);
        } else {
            AddShowInHierarchyMenuItem(ref menu);            
        }
        // Allow to delete a port if its parent is a module.
        if(selectedObject.IsStatePort || selectedObject.IsDynamicPort || selectedObject.IsEnablePort) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, selectedObject, storage);            
    }

    // ======================================================================
    // Menu Utilities
	// ----------------------------------------------------------------------
    void ShowMenu(iCS_MenuContext[] menu, iCS_EditorObject selected, iCS_IStorage storage) {
        int sepCnt= 0;
        GenericMenu gMenu= new GenericMenu();
        foreach(var item in menu) {
            if(item.Command == SeparatorStr) {
                if(sepCnt != 0) gMenu.AddSeparator("");
                sepCnt= 0;
            } else {
                if(item.Command[0] == '#') {
                    string tmp= item.Command.Substring(1);
                    gMenu.AddDisabledItem(new GUIContent(tmp));                                    
                } else {
					item.SelectedObject= selected;
					item.Storage= storage;
					item.GraphPosition= GraphPosition;
                    gMenu.AddItem(new GUIContent(item.Command), false,
                                  c=> GUICommandQueue.Add((iCS_MenuContext)c), item);                                    
                }
                ++sepCnt;
            }
        }
        gMenu.ShowAsContext();
        Reset();
    }
	// ----------------------------------------------------------------------
    void AddDeleteMenuItem(ref iCS_MenuContext[] existingMenu) {
        int idx= ResizeMenu(ref existingMenu, existingMenu.Length+2);
        existingMenu[idx]= new iCS_MenuContext(SeparatorStr);
        existingMenu[idx+1]= new iCS_MenuContext(DeleteStr);
    }
	// ----------------------------------------------------------------------
    void AddShowInHierarchyMenuItem(ref iCS_MenuContext[] existingMenu) {
        int idx= ResizeMenu(ref existingMenu, existingMenu.Length+2);
        existingMenu[idx]= new iCS_MenuContext(SeparatorStr);
        existingMenu[idx+1]= new iCS_MenuContext(ShowHierarchyStr);        
    }
	// ----------------------------------------------------------------------
    int ResizeMenu(ref iCS_MenuContext[] existingMenu, int newSize) {
        int idx= existingMenu.Length;
        if(idx > newSize) idx= newSize;
        iCS_MenuContext[] newMenu= new iCS_MenuContext[newSize];
        existingMenu.CopyTo(newMenu, 0);
        existingMenu= newMenu;
        return idx;
    }
	// ----------------------------------------------------------------------
    iCS_MemberInfo GetReflectionDescFromMenuCommand(iCS_MenuContext menuContext) {
        string menuCommand= iCS_TextUtil.StripBeforeIdent(menuContext.Command);
        return iCS_LibraryDatabase.GetDescriptor(menuCommand);
    }
	// ----------------------------------------------------------------------
    Type GetClassTypeFromMenuCommand(iCS_MenuContext menuContext) {
        string menuCommand= iCS_TextUtil.StripBeforeIdent(menuContext.Command);
        return iCS_LibraryDatabase.GetClassType(menuCommand);
    }
    
    // ======================================================================
    // Menu processing
	// ----------------------------------------------------------------------
    void ProcessMenu(object obj) {
        iCS_MenuContext context= obj as iCS_MenuContext;
        iCS_EditorObject selectedObject= context.SelectedObject;
		Vector2 pos= context.GraphPosition;
        iCS_IStorage storage= context.Storage;
        storage.RegisterUndo(context.Command);
        // Process predefined State children.
        if(selectedObject.IsState) {
            for(int i= 0; i < iCS_AllowedChildren.StateChildNames.Length; ++i) {
                string childName= iCS_AllowedChildren.StateChildNames[i];
                string name= context.Command.Substring(2);
                if(name[0] == ' ') name= name.Substring(1);
                if(name == childName) {
                    string toolTip= iCS_AllowedChildren.StateChildTooltips[i];
                    ProcessCreatePackageWithUnchangableName(childName, context, toolTip);
                    return;
                }
            }            
        }
        // Process all other types of requests.
        switch(context.Command) {
            case ModuleStr:                 ProcessCreatePackage(context); break;
            case StateChartStr:             ProcessCreateStateChart(context); break;
            case StateStr:                  ProcessCreateState(context);  break;
            case SetAsEntryStr:             ProcessSetStateEntry(context); break;
            case OnEntryStr:                ProcessCreateOnEntryModule(context); break;
            case OnUpdateStr:               ProcessCreateOnUpdateModule(context); break;
            case OnExitStr:                 ProcessCreateOnExitModule(context); break;
            case ShowHierarchyStr:          ProcessShowInHierarchy(context); break;
            case DeleteStr:                 ProcessDestroyObject(context); break;
            case EnablePortStr: {
                iCS_EditorObject port= storage.CreatePort(iCS_Strings.EnablePort, selectedObject.InstanceId, typeof(bool), iCS_ObjectTypeEnum.EnablePort);
                port.IsNameEditable= false;
                break;
            }
            case PublishPortStr: {
                iCS_EditorObject parent= selectedObject.Parent;
                iCS_EditorObject grandParent= parent.Parent;
                int grandParentId= grandParent.InstanceId;
				var grandParentRect= grandParent.LayoutRect;
				var portPosition= selectedObject.LayoutPosition;
                if(selectedObject.IsInputPort) {
                    iCS_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, iCS_ObjectTypeEnum.InDynamicPort);
                    storage.SetSource(selectedObject, port);
                    port.SetAnchorAndLayoutPosition(new Vector2(grandParentRect.x, portPosition.y));
                } else {
                    iCS_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, iCS_ObjectTypeEnum.OutDynamicPort);
                    storage.SetSource(port, selectedObject);
                    port.SetAnchorAndLayoutPosition(new Vector2(grandParentRect.xMax, portPosition.y));
                }
                break;                
            }
            default: {
				iCS_MethodBaseInfo desc= context.Descriptor;
				if(desc == null) {
					Debug.LogWarning(iCS_Config.ProductName+": Can find reflection descriptor to create node !!!");
					break;
				}
                if(desc.IsMessage) {
                    storage.CreateMessage(selectedObject.InstanceId, pos, desc);
                } else {
                    CreateMethod(selectedObject, storage, pos, desc);                                                               
                }
                break;                
            }
        }
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreatePackageWithUnchangableName(string behName, iCS_MenuContext context, string toolTip="") {
        iCS_EditorObject module= CreatePackage(context, behName, false);
        module.IsNameEditable= false;
        module.Tooltip= toolTip;
        return module;        
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreatePackage(iCS_MenuContext context) {
        iCS_EditorObject module= CreatePackage(context);
        return module;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreateStateChart(iCS_MenuContext context) {
        iCS_EditorObject stateChart= CreateStateChart(context);
        return stateChart;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreateState(iCS_MenuContext context) {
		var parent       = context.SelectedObject;
		var storage      = context.Storage;
		var graphPosition= context.GraphPosition;
        return storage.CreateState(parent.InstanceId, graphPosition);
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessSetStateEntry(iCS_MenuContext context) {
		var state        = context.SelectedObject;
		var storage      = context.Storage;
        storage.ForEachChild(state.Parent,
            child=>{
                if(child.IsEntryState) {
                    child.IsEntryState= false;
                }
            }
        );
        state.IsEntryState= true;
        return state;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreateOnEntryModule(iCS_MenuContext context) {
        iCS_EditorObject module= CreatePackage(context, iCS_Strings.OnEntry, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes on entry into this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreateOnUpdateModule(iCS_MenuContext context) {
        iCS_EditorObject module= CreatePackage(context, iCS_Strings.OnUpdate, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes on every frame this state is active.";
        return module;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreateOnExitModule(iCS_MenuContext context) {
        iCS_EditorObject module= CreatePackage(context, iCS_Strings.OnExit, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes on exit from this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    static void ProcessShowInHierarchy(iCS_MenuContext context) {
		var obj    = context.SelectedObject;
        var editor= iCS_EditorMgr.FindHierarchyEditor();
        if(editor != null) editor.ShowElement(obj);
    }
	// ----------------------------------------------------------------------
    static void ProcessDestroyObject(iCS_MenuContext context) {
        DestroyObject(context);    
    }

    // ======================================================================
    // Creation Utilities
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreatePackage(iCS_MenuContext context, string name= "", bool nameEditable= true) {
		var parent       = context.SelectedObject;
		var storage      = context.Storage;
		var graphPosition= context.GraphPosition;
        iCS_EditorObject module= storage.CreatePackage(parent.InstanceId, graphPosition, name);
        module.IsNameEditable= nameEditable;
        return module;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateClassModule(iCS_MenuContext context, Type classType) {
		var parent       = context.SelectedObject;
		var storage      = context.Storage;
		var graphPosition= context.GraphPosition;
        iCS_EditorObject module= storage.CreatePackage(parent.InstanceId, graphPosition, null, iCS_ObjectTypeEnum.Package, classType);
        return module;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateStateChart(iCS_MenuContext context, string name= "", bool nameEditable= true) {
		var parent       = context.SelectedObject;
		var storage      = context.Storage;
		var graphPosition= context.GraphPosition;
        iCS_EditorObject stateChart= storage.CreateStateChart(parent.InstanceId, graphPosition, name);
        stateChart.IsNameEditable= nameEditable;
        return stateChart;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateState(iCS_MenuContext context, string name= "") {
		var parent       = context.SelectedObject;
		var storage      = context.Storage;
		var graphPosition= context.GraphPosition;
        return storage.CreateState(parent.InstanceId, graphPosition, name);
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateMethod(iCS_EditorObject parent, iCS_IStorage storage, Vector2 graphPosition, iCS_MethodBaseInfo desc) {
        if(parent.IsPort) {
            iCS_EditorObject port= parent;
            parent= port.Parent;
            iCS_EditorObject grandParent= parent.Parent;
            if(!grandParent.IsKindOfPackage) return null;
			switch(port.Edge) {
				case iCS_EdgeEnum.Top: {
					graphPosition.y-= 100;
					break;
				}
				case iCS_EdgeEnum.Bottom: {
					graphPosition.y+= 100;
					break;
				}
				case iCS_EdgeEnum.Left: {
					graphPosition.x-= 100;
                    graphPosition.y-= 20;
					break;
				}
				case iCS_EdgeEnum.Right:
				default: {
					graphPosition.x+= 100;
					graphPosition.y-= 20;
					break;
				}
			}
            iCS_EditorObject method= storage.CreateFunction(grandParent.InstanceId, graphPosition, desc);
            if(port.IsInputPort) {
				iCS_EditorObject[] outputPorts= Prelude.filter(x=> iCS_Types.IsA(port.RuntimeType, x.RuntimeType), storage.GetChildOutputDataPorts(method)); 
				if(outputPorts.Length >= 1) {
					storage.SetSource(port, outputPorts[0]);
				}
            } else {
				iCS_EditorObject[] inputPorts= Prelude.filter(x=> iCS_Types.IsA(x.RuntimeType, port.RuntimeType), storage.GetChildInputDataPorts(method));
				if(inputPorts.Length >= 1) {
					storage.SetSource(inputPorts[0], port);
				}
            }
            return method;
        }
        return storage.CreateFunction(parent.InstanceId, graphPosition, desc);            
    }
	// ----------------------------------------------------------------------
    static void DestroyObject(iCS_MenuContext context) {
		var selectedObject= context.SelectedObject;
		var iStorage      = context.Storage;
        iCS_EditorUtility.SafeDestroyObject(selectedObject, iStorage);
    }
	// ----------------------------------------------------------------------
    static bool AsChildNodeWithName(iCS_EditorObject parent, string name, iCS_IStorage storage) {
        return storage.UntilMatchingChild(parent,
            child=> {
                if(child.IsNode) {
                    return child.Name == name;
                }
                return false;
            }
        );
    }
}
