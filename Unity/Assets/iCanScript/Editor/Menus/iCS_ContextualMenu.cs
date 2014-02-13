using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ContextualMenu {
    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
	public enum MenuType { SelectedObject, ReleaseAfterDrag };
	
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    Vector2 				GraphPosition  = Vector2.zero;
	List<iCS_MenuContext>	GUICommandQueue= new List<iCS_MenuContext>();
    
    // ======================================================================
    // Menu Items
	// ----------------------------------------------------------------------
    const string ShowHierarchyStr              = "Show in hierarchy";
    const string DeleteStr                     = "- Delete";
    const string PackageStr                    = "+ Package";
    const string StateChartStr                 = "+ State Chart";
    const string StateStr                      = "+ State";
    const string EntryStateStr                 = "+ Entry State";
    const string SetAsEntryStr                 = "Set as Entry";
    const string OnEntryStr                    = "+ "+iCS_Strings.OnEntry;
    const string OnUpdateStr                   = "+ "+iCS_Strings.OnUpdate;
    const string OnExitStr                     = "+ "+iCS_Strings.OnExit;
	const string ObjectInstanceStr             = "+ Object Instance";
    const string EnablePortStr                 = "+ Enable Port";
    const string TriggerPortStr                = "+ Trigger Port";
    const string OutputThisPortStr             = "+ This Port (output)";
	const string WrapInPackageStr              = "+ Wrap in Package";
    const string MultiSelectionWrapInPackageStr= "+ Wrap Multi-Selection in Package";
    const string MultiSelectionDeleteStr       = "- Delete Multi-Selection";
    const string DeleteKeepChildrenStr         = "- Delete Keep Children";
    const string SeparatorStr                  = "";

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
	// Note: reverseInOut is only applicable for Port :: ReleaseAfterDrag.
    public void Update(MenuType menuType, iCS_EditorObject selectedObject, iCS_IStorage storage, Vector2 graphPosition, bool reverseInOut= false) {
        // Update mouse position if not already done.
        if(GraphPosition == Vector2.zero) GraphPosition= graphPosition;

        // Multi-Selection has its own set of menus.
        if(storage.IsMultiSelectionActive) {
            MultiSelectionMenu(storage);
            return;
        }
        
        // Nothing to show if menu is inactive.
        if(selectedObject == null || GraphPosition != graphPosition) {
            Reset();
            return;
        }
        
        // Process the menu state.
        switch(selectedObject.ObjectType) {
            case iCS_ObjectTypeEnum.Behaviour:         BehaviourMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.StateChart:        StateChartMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.State:             StateMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceMessage:   PackageMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassMessage:      PackageMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TransitionPackage: PackageMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.Constructor:       FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceFunction:  FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassFunction:     FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TypeCast:          FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceField:     FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.ClassField:        FunctionMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.Package:           if(selectedObject.IsInstanceNode) {
                                                             InstanceMenu(selectedObject, storage);
                                                       } else {
                                                             PackageMenu(selectedObject, storage);
                                                       }
                                                       break;
            default: if(selectedObject.IsPort)         PortMenu(menuType, selectedObject, storage, reverseInOut); break;
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
    void PackageMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            // Base menu items
            menu= new iCS_MenuContext[5];
            menu[0]= new iCS_MenuContext(PackageStr);
            menu[1]= new iCS_MenuContext(StateChartStr);
            menu[2]= new iCS_MenuContext(SeparatorStr);
            menu[3]= new iCS_MenuContext(EnablePortStr);
            if(storage.HasTriggerPort(selectedObject)) {
                menu[4]= new iCS_MenuContext("#"+TriggerPortStr);
            } else {
                menu[4]= new iCS_MenuContext(TriggerPortStr);                
            }
        }
		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
        if(selectedObject.ObjectType == iCS_ObjectTypeEnum.Package) {
            if(selectedObject.HasChildNode()) {
                int idx= GrowMenuBy(ref menu, 1);
                menu[idx]= new iCS_MenuContext(DeleteKeepChildrenStr);                
            }
        }
		ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void InstanceMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            // Determine if we should support output 'this' port.
            Type classType= selectedObject.RuntimeType;
            bool shouldSupportThis= !iCS_Types.IsStaticClass(classType);
            // Base menu items
            menu= new iCS_MenuContext[shouldSupportThis ? 3 : 2];
            menu[0]= new iCS_MenuContext(EnablePortStr);
            if(storage.HasTriggerPort(selectedObject)) {
                menu[1]= new iCS_MenuContext("#"+TriggerPortStr);
            } else {
                menu[1]= new iCS_MenuContext(TriggerPortStr);                
            }
            if(shouldSupportThis) {
                if(storage.HasOutInstancePort(selectedObject)) {
                    menu[2]= new iCS_MenuContext("#"+OutputThisPortStr);
                } else {
                    menu[2]= new iCS_MenuContext(OutputThisPortStr);                
                }                
            }
        }
		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
        AddShowInHierarchyMenuItem(ref menu);
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
		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
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
        iCS_MenuContext[] menu;
        if(!selectedObject.IsIconizedOnDisplay && !selectedObject.IsFoldedOnDisplay) {
            // Determine if we should support output 'this' port.
            Type classType= selectedObject.RuntimeType;
            bool shouldSupportThis= !iCS_Types.IsStaticClass(classType);
            // Base menu items
            menu= new iCS_MenuContext[shouldSupportThis ? 3 : 2];
            menu[0]= new iCS_MenuContext(EnablePortStr);
            if(storage.HasTriggerPort(selectedObject)) {
                menu[1]= new iCS_MenuContext("#"+TriggerPortStr);
            } else {
                menu[1]= new iCS_MenuContext(TriggerPortStr);                
            }
            if(shouldSupportThis) {
                if(storage.HasOutInstancePort(selectedObject)) {
                    menu[2]= new iCS_MenuContext("#"+OutputThisPortStr);
                } else {
                    menu[2]= new iCS_MenuContext(OutputThisPortStr);                
                }                
            }
        } else {
            menu= new iCS_MenuContext[0];
        }
		AddWrapInPackageIfAppropriate(ref menu, selectedObject);
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void PortMenu(MenuType menuType, iCS_EditorObject port, iCS_IStorage storage, bool reverseInOut) {
		switch(menuType) {
			case MenuType.SelectedObject:
				SelectedPortMenu(port, storage);
				break;
			case MenuType.ReleaseAfterDrag:
				ReleaseAfterDragPortMenu(port, storage, reverseInOut);
				break;
		}
    }
	// ----------------------------------------------------------------------
    void SelectedPortMenu(iCS_EditorObject port, iCS_IStorage storage) {
        iCS_MenuContext[] menu= new iCS_MenuContext[1];
        menu[0]= new iCS_MenuContext(ShowHierarchyStr);
        // Allow to delete a port if its parent is a module.
        if(port.IsStatePort || !port.IsFixDataPort) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, port, storage);            
    }
	// ----------------------------------------------------------------------
    void ReleaseAfterDragPortMenu(iCS_EditorObject port, iCS_IStorage storage, bool reverseInOut) {
		// Different behaviour for "object instance" and standard packages.
        iCS_EditorObject newNodeParent= storage.GetNodeAt(GraphPosition);
		if(newNodeParent == null) return;
        iCS_MenuContext[] menu= new iCS_MenuContext[0];
		if(!newNodeParent.IsInstanceNode) {
			// Add shortcut ti instance node creation.
			menu= new iCS_MenuContext[2];
			menu[0]= new iCS_MenuContext(ObjectInstanceStr);
	        menu[1]= new iCS_MenuContext(SeparatorStr);
		}
        // Get compatible functions.
        if(port.IsDataOrControlPort) {
            List<iCS_MethodBaseInfo> functionMenu= null;
			Type inputType= null;
			Type outputType= null;
			if(port.IsInputPort) {
                outputType= port.RuntimeType;
            } else {
                inputType= port.RuntimeType;
            }
			if(reverseInOut) {
				var tmp= inputType;
				inputType= outputType;
				outputType= tmp;
			}
			if(newNodeParent.IsInstanceNode) {
	            functionMenu= iCS_LibraryDatabase.BuildMenuForMembersOfType(newNodeParent.RuntimeType, inputType, outputType);								
			}
			else {
	            functionMenu= iCS_LibraryDatabase.BuildMenu(inputType, outputType);				
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
        ShowMenu(menu, port, storage);            
    }
	// ----------------------------------------------------------------------
    void MultiSelectionMenu(iCS_IStorage iStorage) {
        var multiSelectedObjects= iStorage.GetMultiSelectedObjects();
        if(multiSelectedObjects == null || multiSelectedObjects.Length == 0) return;
        iCS_MenuContext[] menu= new iCS_MenuContext[3];
        menu[0]= new iCS_MenuContext(MultiSelectionWrapInPackageStr);
        menu[1]= new iCS_MenuContext(SeparatorStr);
        menu[2]= new iCS_MenuContext(MultiSelectionDeleteStr);
        ShowMenu(menu, null, iStorage);
    }
	// ----------------------------------------------------------------------
	void AddWrapInPackageIfAppropriate(ref iCS_MenuContext[] menu, iCS_EditorObject obj) {
		// Don't add if object cannot support a parent package.
		if(!obj.CanHavePackageAsParent()) return;
		if(menu == null || menu.Length == 0) {
			menu= new iCS_MenuContext[1];
			menu[0]= new iCS_MenuContext(WrapInPackageStr);
			return;
		}
		int i= GrowMenuBy(ref menu, 2);
		menu[i]  = new iCS_MenuContext(WrapInPackageStr);
		menu[i+1]= new iCS_MenuContext(SeparatorStr);
	}
	// ----------------------------------------------------------------------
	int GrowMenuBy(ref iCS_MenuContext[] menu, int additionalSize) {
		int sze= menu == null ? 0 : menu.Length;
		if(additionalSize == 0) return sze;
		var newMenu= new iCS_MenuContext[sze+additionalSize];
		menu.CopyTo(newMenu, 0);
		menu= newMenu;
		return sze;
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
        // Reject request if child node is not allowed.
		string tooltip= null;
        if(selectedObject != null && selectedObject.IsState) {
			bool isAllowed= false;
            string name= context.Command.Substring(2);
            if(name[0] == ' ') name= name.Substring(1);
            for(int i= 0; i < iCS_AllowedChildren.StateChildNames.Length; ++i) {
                string childName= iCS_AllowedChildren.StateChildNames[i];
                if(name == childName) {
                    tooltip= iCS_AllowedChildren.StateChildTooltips[i];
					isAllowed= true;
					break;
                }
            }
			if(isAllowed == false) return;            
        }
        // Process all other types of requests.
        switch(context.Command) {
            case PackageStr:        ProcessCreatePackage(context); break;
            case StateChartStr:     ProcessCreateStateChart(context); break;
            case StateStr:          ProcessCreateState(context);  break;
            case SetAsEntryStr:     ProcessSetStateEntry(context); break;
            case OnEntryStr:        ProcessCreateOnEntryPackage(context, tooltip); break;
            case OnUpdateStr:       ProcessCreateOnUpdatePackage(context, tooltip); break;
            case OnExitStr:         ProcessCreateOnExitPackage(context, tooltip); break;
			case ObjectInstanceStr: ProcessCreateObjectInstance(context); break;
            case ShowHierarchyStr:  ProcessShowInHierarchy(context); break;
            case DeleteStr:         ProcessDestroyObject(context); break;
            case EnablePortStr:     ProcessCreateEnablePort(context); break;
            case TriggerPortStr:    ProcessCreateTriggerPort(context); break;
            case OutputThisPortStr: ProcessCreateOutInstancePort(context); break;
			case WrapInPackageStr:  ProcessWrapInPackage(context); break;
            case MultiSelectionWrapInPackageStr: ProcessMultiSelectionWrapInPackage(context); break;
            case MultiSelectionDeleteStr:        ProcessMultiSelectionDelete(context); break;
            case DeleteKeepChildrenStr:          ProcessDeleteKeepChildren(context); break;
            default: {
				iCS_MethodBaseInfo desc= context.Descriptor;
				if(desc == null) {
					Debug.LogWarning(iCS_Config.ProductName+": Can find reflection descriptor to create node !!!");
					break;
				}
                if(selectedObject == null) break;
                if(desc.IsMessage) {
                    iCS_UserCommands.CreateMessageHandler(selectedObject, pos, desc as iCS_MessageInfo);
                } else {
					if(selectedObject.IsPort) {
	                    CreateAttachedMethod(selectedObject, storage, pos, desc);
					}
					else {
	                    CreateMethod(selectedObject, storage, pos, desc);
					}
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
		var graphPosition= context.GraphPosition;
        return iCS_UserCommands.CreateState(parent, graphPosition, null);
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
    static iCS_EditorObject ProcessCreateOnEntryPackage(iCS_MenuContext context, string tooltip) {
		var parent       = context.SelectedObject;
		var graphPosition= context.GraphPosition;
        var package= iCS_UserCommands.CreateOnEntryPackage(parent, graphPosition);
        package.Tooltip= tooltip;
        return package;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreateOnUpdatePackage(iCS_MenuContext context, string tooltip) {
		var parent       = context.SelectedObject;
		var graphPosition= context.GraphPosition;
        var package= iCS_UserCommands.CreateOnUpdatePackage(parent, graphPosition);
        package.Tooltip= tooltip;
        return package;

    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject ProcessCreateOnExitPackage(iCS_MenuContext context, string tooltip) {
		var parent       = context.SelectedObject;
		var graphPosition= context.GraphPosition;
        var package= iCS_UserCommands.CreateOnExitPackage(parent, graphPosition);
        package.Tooltip= tooltip;
        return package;
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
    // -------------------------------------------------------------------------
    static void ProcessCreateEnablePort(iCS_MenuContext context) {
		var parent = context.SelectedObject;
		iCS_UserCommands.CreateEnablePort(parent);        
    }
    // -------------------------------------------------------------------------
    static void ProcessCreateTriggerPort(iCS_MenuContext context) {
		var parent = context.SelectedObject;
		iCS_UserCommands.CreateTriggerPort(parent);        
    }
    // -------------------------------------------------------------------------
    static void ProcessCreateOutInstancePort(iCS_MenuContext context) {
		var parent = context.SelectedObject;
		iCS_UserCommands.CreateOutInstancePort(parent);        
    }
    // -------------------------------------------------------------------------
    static void ProcessCreateObjectInstance(iCS_MenuContext context) {
		var port = context.SelectedObject;
		var storage= context.Storage;
		var pos= context.GraphPosition;
		var parent= storage.GetNodeAt(pos);
		if(parent == null) return;
		iCS_UserCommands.CreateObjectInstance(parent, pos, port.RuntimeType, port);        
    }
    // -------------------------------------------------------------------------
	static void ProcessWrapInPackage(iCS_MenuContext context) {
		var obj= context.SelectedObject;
		var storage= context.Storage;
		var package= iCS_UserCommands.WrapInPackage(obj);
		if(package != null) {
			storage.SelectedObject= package;
		}
	}
    // -------------------------------------------------------------------------
	static void ProcessMultiSelectionWrapInPackage(iCS_MenuContext context) {
        iCS_UserCommands.WrapMultiSelectionInPackage(context.Storage);
    }
    // -------------------------------------------------------------------------
	static void ProcessMultiSelectionDelete(iCS_MenuContext context) {
        iCS_UserCommands.DeleteMultiSelectedObjects(context.Storage);
    }
    // -------------------------------------------------------------------------
    static void ProcessDeleteKeepChildren(iCS_MenuContext context) {
        iCS_UserCommands.DeleteKeepChildren(context.SelectedObject);
    }	

    // ======================================================================
    // Creation Utilities
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreatePackage(iCS_MenuContext context, string name= "", bool isNameEditable= true) {
		var parent   = context.SelectedObject;
		var globalPos= context.GraphPosition;
        var package= iCS_UserCommands.CreatePackage(parent, globalPos, name);
        package.IsNameEditable= isNameEditable;
        return package;
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateObjectInstance(iCS_MenuContext context, Type classType) {
		var parent       = context.SelectedObject;
		var graphPosition= context.GraphPosition;
        return iCS_UserCommands.CreateObjectInstance(parent, graphPosition, classType, null);
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateStateChart(iCS_MenuContext context, string name= "", bool nameEditable= true) {
		var parent   = context.SelectedObject;
		var globalPos= context.GraphPosition;
        return iCS_UserCommands.CreateStateChart(parent, globalPos, name);
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateState(iCS_MenuContext context, string name= "") {
		var parent   = context.SelectedObject;
		var globalPos= context.GraphPosition;
        return iCS_UserCommands.CreateState(parent, globalPos, name);
    }
	// ----------------------------------------------------------------------
    static iCS_EditorObject CreateMethod(iCS_EditorObject parent, iCS_IStorage storage, Vector2 graphPosition, iCS_MethodBaseInfo desc) {
        return iCS_UserCommands.CreateFunction(parent, graphPosition, desc);            
	}
	// ----------------------------------------------------------------------
	/*
		TODO : Should move this function to IStorage.
	*/
    static iCS_EditorObject CreateAttachedMethod(iCS_EditorObject port, iCS_IStorage storage, Vector2 graphPosition, iCS_MethodBaseInfo desc) {
        iCS_EditorObject newNodeParent= storage.GetNodeAt(graphPosition);
		if(newNodeParent == null) return null;
        if(!newNodeParent.IsKindOfPackage || newNodeParent.IsBehaviour) return null;
		iCS_EditorObject method= null;
		if(newNodeParent.IsInstanceNode) {
			method= iCS_UserCommands.CreateInstanceElement(newNodeParent, desc);
		}
		else {
			bool createMethod= true;
			if(desc.IsInstanceFunction || desc.IsInstanceField) {
				if(desc.ClassType != port.RuntimeType) {
					int sel= EditorUtility.DisplayDialogComplex("Missing the Instance Node",
																"The function you selected requires an instance.\nPlease select one of the following:\n1) automatically create the Instance Builder and Instance Wizard;\n2) automatically create the Instance Wizard (I will provide the instance);\n3) just create the function. (I will provide the instance).",
																"Function",
																"Build Instance",
																"Instance Wizard");
					switch(sel) {
					case 0:
						break;
					case 1: {
						method= iCS_UserCommands.CreateInstanceBuilderAndObjectAndElement(newNodeParent, graphPosition, desc.ClassType, desc);
						createMethod= false;
						break;
					}
					case 2: {
						method= iCS_UserCommands.CreateInstanceObjectAndElement(newNodeParent, graphPosition, desc.ClassType, desc);
						createMethod= false;
						break;
					}}
				}
			}
			if(createMethod) {
		        method= CreateMethod(newNodeParent, storage, graphPosition, desc);							
			}
		}

		// Inverse effective data flow if new node is inside port parent.
		bool isInputPort= port.IsInputPort;
		var portParent= port.ParentNode;
		if(portParent.IsParentOf(method)) isInputPort= !isInputPort;
        if(isInputPort) {
			iCS_EditorObject[] outputPorts= Prelude.filter(x=> iCS_Types.IsA(port.RuntimeType, x.RuntimeType), storage.GetChildOutputDataPorts(method)); 
			// Connect if only one possibility.
			if(outputPorts.Length == 1) {
				storage.SetNewDataConnection(port, outputPorts[0]);
			}
			else {
				var bestPort= GetClosestMatch(port, outputPorts);
				if(bestPort != null) {
					storage.SetNewDataConnection(port, bestPort);						
				}
			}
        } else {
			iCS_EditorObject[] inputPorts= Prelude.filter(x=> iCS_Types.IsA(x.RuntimeType, port.RuntimeType), storage.GetChildInputDataPorts(method));
			// Connect if only one posiibility
			if(inputPorts.Length == 1) {
				storage.SetNewDataConnection(inputPorts[0], port);
			}
			// Multiple choices exist so try the one with the closest name.
			else {
				var bestPort= GetClosestMatch(port, inputPorts);
				if(bestPort != null) {
					storage.SetNewDataConnection(bestPort, port);											
				}
			}
        }
        return method;
    }
	// ----------------------------------------------------------------------
	static iCS_EditorObject GetClosestMatch(iCS_EditorObject refPort, iCS_EditorObject[] otherPorts) {
		if(otherPorts.Length == 0) return null;
		float bestScore= 0f;
		iCS_EditorObject bestPort= null;
		string refName= refPort.Name;
		if(string.IsNullOrEmpty(refPort.RawName)) refName= refPort.ParentNode.Name;
		foreach(var p in otherPorts) {
			string pName= p.Name;
			if(string.IsNullOrEmpty(p.RawName)) pName= p.ParentNode.Name;
			var score= iCS_StringUtil.FuzzyCompare(refName, pName);
			if(score > bestScore) {
				bestScore= score;
				bestPort= p;
			}
		}
		return bestPort;		
	}
	// ----------------------------------------------------------------------
    static void DestroyObject(iCS_MenuContext context) {
		var selectedObject= context.SelectedObject;
        iCS_UserCommands.DeleteObject(selectedObject);
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
