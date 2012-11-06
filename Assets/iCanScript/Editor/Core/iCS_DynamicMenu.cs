using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_DynamicMenu {
    internal class MenuContext {
        public string               Command;
        public iCS_EditorObject     SelectedObject;
        public iCS_IStorage         Storage;
        public iCS_ReflectionInfo   Descriptor;
        public MenuContext(string command, iCS_ReflectionInfo descriptor= null) : this(command, descriptor, null, null) {}
        public MenuContext(string command, iCS_ReflectionInfo descriptor, iCS_EditorObject selected, iCS_IStorage storage) {
            Command= command;
            SelectedObject= selected;
            Storage= storage;
            Descriptor= descriptor;
        }
    }
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    Vector2 MenuPosition= Vector2.zero;
    Vector2 ProcessMenuPosition= Vector2.zero;
    
    // ======================================================================
    // Menu Items
	// ----------------------------------------------------------------------
    const string ShowHierarchyStr= "Show in hierarchy";
    const string DeleteStr= "- Delete";
    const string StartStr= "+ Start";
    const string ModuleStr= "+ Module";
    const string StateChartStr= "+ State Chart";
    const string StateStr= "+ State";
    const string EntryStateStr= "+ Entry State";
    const string UpdateModuleStr= "+ Update/Module";
    const string UpdateStateChartStr= "+ Update/StateChart";
    const string LateUpdateModuleStr= "+ LateUpdate/Module";
    const string LateUpdateStateChartStr= "+ LateUpdate/StateChart";
    const string FixedUpdateModuleStr= "+ FixedUpdate/Module";
    const string FixedUpdateStateChartStr= "+ FixedUpdate/StateChart";
    const string SetAsEntryStr="Set as Entry";
    const string OnGUIStr= "+ OnGUI";
    const string OnDrawGizmosStr= "+ OnDrawGizmos";
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
        MenuPosition= Vector2.zero;
    }
    
	// ----------------------------------------------------------------------
    public void Update(iCS_EditorObject selectedObject, iCS_IStorage storage, Vector2 mouseDownPosition) {
        // Update mouse position if not already done.
        if(MenuPosition == Vector2.zero) MenuPosition= mouseDownPosition;

        // Nothing to show if menu is inactive.
        if(selectedObject == null || MenuPosition != mouseDownPosition) {
            Reset();
            return;
        }
        ProcessMenuPosition= MenuPosition;
        
        // Process the menu state.
        switch(selectedObject.ObjectType) {
            case iCS_ObjectTypeEnum.Behaviour:        BehaviourMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.StateChart:       StateChartMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.State:            StateMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.Module:           ModuleMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TransitionGuard:  ModuleMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TransitionAction: ModuleMenu(selectedObject, storage); break;
			case iCS_ObjectTypeEnum.TransitionModule: TransitionModuleMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.Constructor:      MethodMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceMethod:   MethodMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.StaticMethod:     MethodMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.TypeCast:         MethodMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.InstanceField:    MethodMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.StaticField:      MethodMenu(selectedObject, storage); break;
            default: if(selectedObject.IsPort)        PortMenu(selectedObject, storage); break;
        }
    }

	// ----------------------------------------------------------------------
    void BehaviourMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        // Don't show any menu if behaviour not visible.
        if(storage.IsIconized(selectedObject) || storage.IsFolded(selectedObject)) return;

        int len= iCS_AllowedChildren.BehaviourChildNames.Length;
		MenuContext[] menu= new MenuContext[len];
        for(int i= 0; i < len; ++i) {
            string name= iCS_AllowedChildren.BehaviourChildNames[i];
            if(iCS_AllowedChildren.CanAddChildNode(name, iCS_ObjectTypeEnum.Module, selectedObject, storage)) {
                menu[i]= new MenuContext(String.Concat("+ ", name));
            } else {
                menu[i]= new MenuContext(String.Concat("#+ ", name));
            }
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void ModuleMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        MenuContext[] menu= new MenuContext[0];
        if(!storage.IsIconized(selectedObject) && !storage.IsFolded(selectedObject)) {
            // Base menu items
            menu= new MenuContext[2];
            menu[0]= new MenuContext(ModuleStr);
            menu[1]= new MenuContext(StateChartStr); 
        }
//        // Function menu items
//        if(!storage.IsIconized(selectedObject) && !storage.IsFolded(selectedObject)) {
//            List<iCS_ReflectionInfo> functionMenu= iCS_DataBase.BuildExpertMenu();
//            tmp= new MenuContext[menu.Length+functionMenu.Count+1];
//            menu.CopyTo(tmp, 0);
//            tmp[menu.Length]= new MenuContext(SeparatorStr);
//            for(int i= 0; i < functionMenu.Count; ++i) {
//                tmp[i+menu.Length+1]= new MenuContext("+ "+functionMenu[i].ToString(), functionMenu[i]);
//            }
//            menu= tmp;            
//        }            
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
        MenuContext[] menu= new MenuContext[1];
        menu[0]= new MenuContext(ShowHierarchyStr);
        AddDeleteMenuItem(ref menu);
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateChartMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        MenuContext[] menu= new MenuContext[0];
        if(!storage.IsIconized(selectedObject) && !storage.IsFolded(selectedObject)) {
            menu= new MenuContext[1];
            menu[0]= new MenuContext(StateStr); 
        }
        AddShowInHierarchyMenuItem(ref menu);
        if(selectedObject.InstanceId != 0) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        MenuContext[] menu;
        if(!storage.IsIconized(selectedObject) && !storage.IsFolded(selectedObject)) {
            int len= iCS_AllowedChildren.StateChildNames.Length;
            menu= new MenuContext[len+4];
            menu[0]= new MenuContext(StateStr);
            menu[1]= new MenuContext(SeparatorStr);
            for(int i= 0; i < len; ++i) {
                string name= iCS_AllowedChildren.StateChildNames[i];
                if(iCS_AllowedChildren.CanAddChildNode(name, iCS_ObjectTypeEnum.Module, selectedObject, storage)) {
                    menu[i+2]= new MenuContext(String.Concat("+ ", name));
                } else {
                    menu[i+2]= new MenuContext(String.Concat("#+ ", name));
                }
            }
            menu[len+2]= new MenuContext(SeparatorStr);
            if(selectedObject.IsRawEntryState) {
                menu[len+3]= new MenuContext(String.Concat("#", SetAsEntryStr));
            } else {
                menu[len+3]= new MenuContext(SetAsEntryStr);
            }
        } else {
            menu= new MenuContext[0];
        }
        AddShowInHierarchyMenuItem(ref menu);
        AddDeleteMenuItem(ref menu);
        ShowMenu(menu, selectedObject, storage);
    }
    
	// ----------------------------------------------------------------------
    void MethodMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
		MenuContext[] menu= new MenuContext[1];
		menu[0]= new MenuContext(ShowHierarchyStr);
        if(storage.EditorObjects[selectedObject.ParentId].IsModule) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, selectedObject, storage);            
    }
	// ----------------------------------------------------------------------
    void PortMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
		/*
			TODO : Add use as library filter option.
		*/
        MenuContext[] menu= new MenuContext[0];
        // Allow to publish port if the grand-parent is a module.
        iCS_EditorObject parent= storage.EditorObjects[selectedObject.ParentId];
        iCS_EditorObject grandParent= storage.EditorObjects[parent.ParentId];
        if(grandParent != null && grandParent.IsModule) {
            if(!(selectedObject.IsInputPort && selectedObject.IsSourceValid)) {
                menu= new MenuContext[1];
				menu[0]= new MenuContext(PublishPortStr);                
            }
        }
        // Get compatible functions.
        if(selectedObject.IsDataPort) {
            List<iCS_ReflectionInfo> functionMenu= null;
            if(selectedObject.IsInputPort) {
                functionMenu= iCS_DataBase.BuildMenu(null, selectedObject.RuntimeType);
            } else {
                functionMenu= iCS_DataBase.BuildMenu(selectedObject.RuntimeType, null);
            }
            if(functionMenu.Count != 0) {
                int len= menu.Length;
                MenuContext[] tmp= null;
                if(len == 0) {
                    tmp= new MenuContext[functionMenu.Count];
                } else {
                    tmp= new MenuContext[len+1+functionMenu.Count];
                    menu.CopyTo(tmp, 0);
                    tmp[len]= new MenuContext(SeparatorStr);
                    ++len;                    
                }
                menu= tmp;                
                for(int i= 0; i < functionMenu.Count; ++i) {
                    menu[len+i]= new MenuContext(functionMenu[i].ToString(), functionMenu[i]);
                }
            }
        }
        if(menu.Length == 0) {
            menu= new MenuContext[1];
            menu[0]= new MenuContext(ShowHierarchyStr);
        } else {
            AddShowInHierarchyMenuItem(ref menu);            
        }
        // Allow to delete a port if its parent is a module.
        if(selectedObject.IsStatePort || selectedObject.IsDynamicModulePort || selectedObject.IsEnablePort) {
            AddDeleteMenuItem(ref menu);
        }
        ShowMenu(menu, selectedObject, storage);            
    }

    // ======================================================================
    // Menu Utilities
	// ----------------------------------------------------------------------
    void ShowMenu(MenuContext[] menu, iCS_EditorObject selected, iCS_IStorage storage) {
        ShowMenu(menu, MenuPosition, selected, storage);
    }
    void ShowMenu(MenuContext[] menu, Vector2 pos, iCS_EditorObject selected, iCS_IStorage storage) {
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
                    gMenu.AddItem(new GUIContent(item.Command), false, ProcessMenu, item);                                    
                }
                ++sepCnt;
            }
        }
        gMenu.ShowAsContext();
        Reset();
    }
	// ----------------------------------------------------------------------
    void AddDeleteMenuItem(ref MenuContext[] existingMenu) {
        int idx= ResizeMenu(ref existingMenu, existingMenu.Length+2);
        existingMenu[idx]= new MenuContext(SeparatorStr);
        existingMenu[idx+1]= new MenuContext(DeleteStr);
    }
	// ----------------------------------------------------------------------
    void AddShowInHierarchyMenuItem(ref MenuContext[] existingMenu) {
        int idx= ResizeMenu(ref existingMenu, existingMenu.Length+2);
        existingMenu[idx]= new MenuContext(SeparatorStr);
        existingMenu[idx+1]= new MenuContext(ShowHierarchyStr);        
    }
	// ----------------------------------------------------------------------
    int ResizeMenu(ref MenuContext[] existingMenu, int newSize) {
        int idx= existingMenu.Length;
        if(idx > newSize) idx= newSize;
        MenuContext[] newMenu= new MenuContext[newSize];
        existingMenu.CopyTo(newMenu, 0);
        existingMenu= newMenu;
        return idx;
    }
	// ----------------------------------------------------------------------
    iCS_ReflectionInfo GetReflectionDescFromMenuCommand(MenuContext menuContext) {
        string menuCommand= iCS_TextUtil.StripBeforeIdent(menuContext.Command);
        return iCS_DataBase.GetDescriptor(menuCommand);
    }
	// ----------------------------------------------------------------------
    Type GetClassTypeFromMenuCommand(MenuContext menuContext) {
        string menuCommand= iCS_TextUtil.StripBeforeIdent(menuContext.Command);
        return iCS_DataBase.GetClassType(menuCommand);
    }
    
    // ======================================================================
    // Menu processing
	// ----------------------------------------------------------------------
    void ProcessMenu(object obj) {
        MenuContext context= obj as MenuContext;
        iCS_EditorObject selectedObject= context.SelectedObject;
        iCS_IStorage storage= context.Storage;
        storage.RegisterUndo(context.Command);
        // Process predefined modules for Behaviour & State.
        if(selectedObject.IsBehaviour) {
            for(int i= 0; i < iCS_AllowedChildren.BehaviourChildNames.Length; ++i) {
                string childName= iCS_AllowedChildren.BehaviourChildNames[i];
                string name= context.Command.Substring(2);
                if(name[0] == ' ') name= name.Substring(1);
                if(name == childName) {
                    string toolTip= iCS_AllowedChildren.BehaviourChildTooltips[i];
                    ProcessCreateModuleWithUnchangableName(childName, selectedObject, storage, toolTip);
                    return;
                }
            }
        }
        if(selectedObject.IsState) {
            for(int i= 0; i < iCS_AllowedChildren.StateChildNames.Length; ++i) {
                string childName= iCS_AllowedChildren.StateChildNames[i];
                string name= context.Command.Substring(2);
                if(name[0] == ' ') name= name.Substring(1);
                if(name == childName) {
                    string toolTip= iCS_AllowedChildren.StateChildTooltips[i];
                    ProcessCreateModuleWithUnchangableName(childName, selectedObject, storage, toolTip);
                    return;
                }
            }            
        }
        // Process all other types of requests.
        switch(context.Command) {
            case StartStr:                  ProcessCreateStart(selectedObject, storage); break;
            case UpdateModuleStr:           ProcessCreateUpdateModule(selectedObject, storage); break;
            case UpdateStateChartStr:       ProcessCreateUpdateStateChart(selectedObject, storage); break;
            case FixedUpdateModuleStr:      ProcessCreateFixedUpdateModule(selectedObject, storage); break;
            case FixedUpdateStateChartStr:  ProcessCreateFixedUpdateStateChart(selectedObject, storage); break;
            case LateUpdateModuleStr:       ProcessCreateLateUpdateModule(selectedObject, storage); break;
            case LateUpdateStateChartStr:   ProcessCreateLateUpdateStateChart(selectedObject, storage); break;
            case OnGUIStr:                  ProcessCreateOnGui(selectedObject, storage); break;
            case OnDrawGizmosStr:           ProcessCreateOnDrawGizmos(selectedObject, storage); break;
            case ModuleStr:                 ProcessCreateModule(selectedObject, storage); break;
            case StateChartStr:             ProcessCreateStateChart(selectedObject, storage); break;
            case StateStr:                  ProcessCreateState(selectedObject, storage);  break;
            case SetAsEntryStr:             ProcessSetStateEntry(selectedObject, storage); break;
            case OnEntryStr:                ProcessCreateOnEntryModule(selectedObject, storage); break;
            case OnUpdateStr:               ProcessCreateOnUpdateModule(selectedObject, storage); break;
            case OnExitStr:                 ProcessCreateOnExitModule(selectedObject, storage); break;
            case ShowHierarchyStr:          ProcessShowInHierarchy(selectedObject, storage); break;
            case DeleteStr:                 ProcessDestroyObject(selectedObject, storage); break;
            case EnablePortStr: {
                iCS_EditorObject port= storage.CreatePort(iCS_Strings.EnablePort, selectedObject.InstanceId, typeof(bool), iCS_ObjectTypeEnum.EnablePort);
                port.IsNameEditable= false;
                break;
            }
            case PublishPortStr: {
                iCS_EditorObject parent= selectedObject.Parent;
                iCS_EditorObject grandParent= parent.Parent;
                int grandParentId= grandParent.InstanceId;
                if(selectedObject.IsInputPort) {
                    iCS_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                    storage.SetSource(selectedObject, port);
                    port.LocalPosition= new Rect(0, parent.LocalPosition.y+selectedObject.LocalPosition.y, 0, 0);
                } else {
                    iCS_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                    storage.SetSource(port, selectedObject);
                    port.LocalPosition= new Rect(grandParent.LocalPosition.width, parent.LocalPosition.y+selectedObject.LocalPosition.y, 0, 0);
                }
                grandParent.IsDirty= true;
                break;                
            }
            default: {
				iCS_ReflectionInfo desc= context.Descriptor;
				if(desc == null) {
					Debug.LogWarning(iCS_Config.ProductName+": Can find reflection descriptor to create node !!!");
					break;
				}
                CreateMethod(selectedObject, storage, desc);                                           
                break;                
            }
        }
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateStart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_Strings.Start, false);
        module.IsNameEditable= false;
        module.Tooltip= "Awake is called when the behaviour is being loaded.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateModuleWithUnchangableName(string behName, iCS_EditorObject parent, iCS_IStorage storage, string toolTip="") {
        iCS_EditorObject module= CreateModule(parent, storage, behName, false);
        module.IsNameEditable= false;
        module.Tooltip= toolTip;
        return module;        
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateUpdateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_Strings.Update, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes on every frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateUpdateStateChart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject stateChart= CreateStateChart(parent, storage, iCS_Strings.Update, false);
        stateChart.IsNameEditable= false;
        stateChart.Tooltip= "Executes on every frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateLateUpdateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_Strings.LateUpdate, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes after every frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateLateUpdateStateChart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject stateChart= CreateStateChart(parent, storage, iCS_Strings.LateUpdate, false);
        stateChart.IsNameEditable= false;
        stateChart.Tooltip= "Executes after every frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateFixedUpdateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_Strings.FixedUpdate, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes at a fix frame rate. Independent from the frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateFixedUpdateStateChart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject stateChart= CreateStateChart(parent, storage, iCS_Strings.FixedUpdate, false);
        stateChart.IsNameEditable= false;
        stateChart.Tooltip= "Executes at a fix frame rate. Independent from the frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateOnGui(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_GuiUtilities.UnsupportedFeature();
        return null;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateOnDrawGizmos(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_GuiUtilities.UnsupportedFeature();
        return null;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage);
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateStateChart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject stateChart= CreateStateChart(parent, storage);
        return stateChart;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateState(iCS_EditorObject parent, iCS_IStorage storage) {
        return storage.CreateState(parent.InstanceId, ProcessMenuPosition);
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessSetStateEntry(iCS_EditorObject state, iCS_IStorage storage) {
        iCS_EditorObject parent= state.Parent;
        storage.ForEachChild(parent,
            child=>{
                if(child.IsEntryState) {
                    child.IsRawEntryState= false;
                    storage.SetDirty(child);
                }
            }
        );
        state.IsRawEntryState= true;
        storage.SetDirty(state);
        return state;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateOnEntryModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_Strings.OnEntry, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes on entry into this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateOnUpdateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_Strings.OnUpdate, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes on every frame this state is active.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateOnExitModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_Strings.OnExit, false);
        module.IsNameEditable= false;
        module.Tooltip= "Executes on exit from this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    void ProcessShowInHierarchy(iCS_EditorObject obj, iCS_IStorage iStorage) {
        var editor= iCS_EditorMgr.FindHierarchyEditor();
        if(editor != null) editor.ShowElement(obj);
    }
	// ----------------------------------------------------------------------
    void ProcessDestroyObject(iCS_EditorObject obj, iCS_IStorage storage) {
        DestroyObject(obj, storage);    
    }

    // ======================================================================
    // Creation Utilities
	// ----------------------------------------------------------------------
    iCS_EditorObject CreateModule(iCS_EditorObject parent, iCS_IStorage storage, string name= "", bool nameEditable= true) {
        iCS_EditorObject module= storage.CreateModule(parent.InstanceId, ProcessMenuPosition, name);
        module.IsNameEditable= nameEditable;
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject CreateClassModule(iCS_EditorObject parent, iCS_IStorage storage, Type classType) {
        iCS_EditorObject module= storage.CreateModule(parent.InstanceId, ProcessMenuPosition, null, iCS_ObjectTypeEnum.Module, classType);
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject CreateStateChart(iCS_EditorObject parent, iCS_IStorage storage, string name= "", bool nameEditable= true) {
        iCS_EditorObject stateChart= storage.CreateStateChart(parent.InstanceId, ProcessMenuPosition, name);
        stateChart.IsNameEditable= nameEditable;
        return stateChart;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject CreateState(iCS_EditorObject parent, iCS_IStorage storage, string name= "") {
        return storage.CreateState(parent.InstanceId, ProcessMenuPosition, name);
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject CreateMethod(iCS_EditorObject parent, iCS_IStorage storage, iCS_ReflectionInfo desc) {
        if(parent.IsPort) {
            iCS_EditorObject port= parent;
            parent= port.Parent;
            iCS_EditorObject grandParent= parent.Parent;
            if(!grandParent.IsModule) return null;
			Vector2 pos= ProcessMenuPosition;
			switch(port.Edge) {
				case iCS_EdgeEnum.Top: {
					pos.y-= 100;
					break;
				}
				case iCS_EdgeEnum.Bottom: {
					pos.y+= 100;
					break;
				}
				case iCS_EdgeEnum.Left: {
					pos.x-= 100;
                    pos.y-= 20;
					break;
				}
				case iCS_EdgeEnum.Right:
				default: {
					pos.x+= 100;
					pos.y-= 20;
					break;
				}
			}
            iCS_EditorObject method= storage.CreateMethod(grandParent.InstanceId, pos, desc);
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
        return storage.CreateMethod(parent.InstanceId, ProcessMenuPosition, desc);            
    }
	// ----------------------------------------------------------------------
    void DestroyObject(iCS_EditorObject selectedObject, iCS_IStorage iStorage) {
        iCS_EditorUtility.SafeDestroyObject(selectedObject, iStorage);
        Reset();
    }
	// ----------------------------------------------------------------------
    bool AsChildNodeWithName(iCS_EditorObject parent, string name, iCS_IStorage storage) {
        return storage.ForEachChild(parent,
            child=> {
                if(child.IsNode) {
                    return child.Name == name;
                }
                return false;
            }
        );
    }
}
