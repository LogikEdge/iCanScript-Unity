using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_DynamicMenu {
    internal class MenuContext {
        public string          Command;
        public iCS_EditorObject SelectedObject;
        public iCS_IStorage      Storage;
        public MenuContext(string command, iCS_EditorObject selected, iCS_IStorage storage) {
            Command= command;
            SelectedObject= selected;
            Storage= storage;
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
    const string DeleteStr= "- Delete";
    const string FoldStr= "Fold";
    const string UnfoldStr= "Unfold";
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
    const string OnEntryStr= "+ "+iCS_EditorStrings.OnEntryNode;
    const string OnUpdateStr= "+ "+iCS_EditorStrings.OnUpdateNode;
    const string OnExitStr= "+ "+iCS_EditorStrings.OnExitNode;
    const string PublishPortStr= "Publish on Module";
    const string EnablePortStr= "+ Enable Port";
    const string UnhideTransitionGuardStr= "Show Transition Guard";
    const string UnhideTransitionActionStr= "Show Transition Action";
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
            case iCS_ObjectTypeEnum.InstanceMethod:   MethodMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.StaticMethod:     MethodMenu(selectedObject, storage); break;
            case iCS_ObjectTypeEnum.Conversion:       MethodMenu(selectedObject, storage); break;
            default: if(selectedObject.IsPort)       PortMenu(selectedObject, storage); break;
        }
    }

	// ----------------------------------------------------------------------
    void BehaviourMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        // Don't show any menu if behaviour not visible.
        if(storage.IsMinimized(selectedObject) || storage.IsFolded(selectedObject)) return;

        int len= iCS_AllowedChildren.BehaviourChildNames.Length;
        string[] menu= new string[len];
        for(int i= 0; i < len; ++i) {
            string name= iCS_AllowedChildren.BehaviourChildNames[i];
            if(iCS_AllowedChildren.CanAddChildNode(name, iCS_ObjectTypeEnum.Module, selectedObject, storage)) {
                menu[i]= String.Concat("+ ", name);
            } else {
                menu[i]= String.Concat("#+ ", name);
            }
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void ModuleMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        string[] tmp= null;
        string[] menu= new string[0];
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            // Base menu items
            menu= new string[3];
            menu[0]= ModuleStr;
            menu[1]= StateChartStr; 
            // Enable port
            bool hasEnablePort= storage.HasEnablePort(selectedObject);
            if(!hasEnablePort) {
                menu[2]= EnablePortStr;
            } else {
                menu[2]= "#"+EnablePortStr;
            }
        }
        // Function menu items
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            string[] functionMenu= GetFunctionMenu();
            tmp= new string[menu.Length+functionMenu.Length+1];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            for(int i= 0; i < functionMenu.Length; ++i) {
                tmp[i+menu.Length+1]= "+ "+functionMenu[i];
            }
            menu= tmp;            
        }
        // Delete menu item
        if(selectedObject.InstanceId != 0) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            tmp[menu.Length+1]= DeleteStr;
            menu= tmp;
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateChartMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        string[] menu= new string[0];
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            menu= new string[1];
            menu[0]= StateStr; 
        }
        // Delete menu item
        string[] tmp= null;
        if(selectedObject.InstanceId != 0) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            tmp[menu.Length+1]= DeleteStr;
            menu= tmp;
        }
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        string[] menu= new string[0];
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            menu= new string[6];
            menu[0]= OnEntryStr;
            menu[1]= OnUpdateStr;
            menu[2]= OnExitStr;
            menu[3]= StateStr;
            menu[4]= SeparatorStr;
            menu[5]= SetAsEntryStr;
            for(int i= 0; i < 3; ++i) {
                string name= menu[i].Substring(2);
                int sep= name.IndexOf('/');
                if(sep > 0) name= name.Substring(0,sep);
                if(AsChildNodeWithName(selectedObject, name, storage)) {
                    menu[i]= String.Concat("#+ ", name);
                }
            }
        }
        // Delete menu item.tmp= new string[menu.Length+2];
        string[] tmp= null;
        tmp= new string[menu.Length+2];
        menu.CopyTo(tmp, 0);
        tmp[menu.Length]= SeparatorStr;
        tmp[menu.Length+1]= DeleteStr;
        menu= tmp;
        ShowMenu(menu, selectedObject, storage);
    }
    
	// ----------------------------------------------------------------------
    void MethodMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        if(storage.EditorObjects[selectedObject.ParentId].IsModule) {
            ShowMenu(new string[]{DeleteStr}, selectedObject, storage);            
        }
    }
	// ----------------------------------------------------------------------
    void PortMenu(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        string[] menu= new string[0];
        // Allow to publish port if the grand-parent is a module.
        iCS_EditorObject parent= storage.EditorObjects[selectedObject.ParentId];
        iCS_EditorObject grandParent= storage.EditorObjects[parent.ParentId];
        if(grandParent != null && grandParent.IsModule) {
            if(!(selectedObject.IsInputPort && storage.IsValid(selectedObject.Source))) {
                menu= new string[]{PublishPortStr};                
            }
        }
        // Allow to delete a port if its parent is a module.
        if(selectedObject.IsStatePort || selectedObject.IsDynamicModulePort || selectedObject.IsEnablePort) {
            int i= menu.Length;
            if(i == 0) {
                menu= new string[1];
                menu[0]= DeleteStr;
            } else {
                string[] tmp= new string[i+2];
                menu.CopyTo(tmp, 0);
                tmp[i]= SeparatorStr;
                tmp[i+1]= DeleteStr;
                menu= tmp;                
            }
        }
        // Display menu if not empty.
        if(menu.Length != 0) {
            ShowMenu(menu, selectedObject, storage);            
        }
    }

    // ======================================================================
    // Menu Utilities
	// ----------------------------------------------------------------------
    void ShowMenu(string[] menu, iCS_EditorObject selected, iCS_IStorage storage) {
        ShowMenu(menu, MenuPosition, selected, storage);
    }
    void ShowMenu(string[] menu, Vector2 pos, iCS_EditorObject selected, iCS_IStorage storage) {
        int sepCnt= 0;
        GenericMenu gMenu= new GenericMenu();
        foreach(var item in menu) {
            if(item == SeparatorStr) {
                if(sepCnt != 0) gMenu.AddSeparator("");
                sepCnt= 0;
            } else {
                if(item[0] == '#') {
                    string tmp= item.Substring(1);
                    gMenu.AddDisabledItem(new GUIContent(tmp));                                    
                } else {
                    gMenu.AddItem(new GUIContent(item), false, ProcessMenu, new MenuContext(item, selected, storage));                                    
                }
                ++sepCnt;
            }
        }
        gMenu.ShowAsContext();
        Reset();
    }
	// ----------------------------------------------------------------------
    // Combines the company/package/function name into menu conforming strings.
    string[] GetFunctionMenu() {
        return iCS_DataBase.BuildMenu();
    }
    iCS_ReflectionDesc GetReflectionDescFromMenuCommand(string menuCommand) {
        menuCommand= iCS_TextUtil.StripBeforeIdent(menuCommand);
        string[] idents= menuCommand.Split(new char[1]{'/'});
        if(idents.Length < 3) return null;
        string company= idents[0];
        string package= idents[1];
        string function= idents[2];
        string signature= null;
        if(idents.Length >= 4) signature= idents[3];
        return iCS_DataBase.GetDescriptor(company, package, function, signature);
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
                    string toolTip= iCS_AllowedChildren.BehaviourChildToolTips[i];
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
                    string toolTip= iCS_AllowedChildren.StateChildToolTips[i];
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
            case ModuleStr:                 ProcessCreateModule(selectedObject, storage); break;
            case StateChartStr:             ProcessCreateStateChart(selectedObject, storage); break;
            case StateStr:                  ProcessCreateState(selectedObject, storage);  break;
            case OnEntryStr:                ProcessCreateOnEntryModule(selectedObject, storage); break;
            case OnUpdateStr:               ProcessCreateOnUpdateModule(selectedObject, storage); break;
            case OnExitStr:                 ProcessCreateOnExitModule(selectedObject, storage); break;
            case FoldStr:                   storage.Fold(selectedObject); break;
            case UnfoldStr:                 storage.Unfold(selectedObject); break;
            case DeleteStr:                 ProcessDestroyObject(selectedObject, storage); break;
            case UnhideTransitionGuardStr:  ProcessUnhideTransitionGuard(selectedObject, storage); break;
            case UnhideTransitionActionStr: ProcessUnhideTransitionAction(selectedObject, storage); break;
            case EnablePortStr: {
                iCS_EditorObject port= storage.CreatePort(iCS_EditorStrings.EnablePort, selectedObject.InstanceId, typeof(bool), iCS_ObjectTypeEnum.EnablePort);
                port.IsNameEditable= false;
                break;
            }
            case PublishPortStr: {
                iCS_EditorObject parent= storage.GetParent(selectedObject);
                iCS_EditorObject grandParent= storage.GetParent(parent);
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
                iCS_ReflectionDesc desc= GetReflectionDescFromMenuCommand(context.Command);
                if(desc != null) {
                    CreateMethod(context.SelectedObject, context.Storage, desc);                                           
                }
                break;                
            }
        }
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateStart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_EditorStrings.StartNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Awake is called when the behaviour is being loaded.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateModuleWithUnchangableName(string behName, iCS_EditorObject parent, iCS_IStorage storage, string toolTip="") {
        iCS_EditorObject module= CreateModule(parent, storage, behName, false);
        module.IsNameEditable= false;
        module.ToolTip= toolTip;
        return module;        
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateUpdateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_EditorStrings.UpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on every frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateUpdateStateChart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject stateChart= CreateStateChart(parent, storage, iCS_EditorStrings.UpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes on every frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateLateUpdateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_EditorStrings.LateUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes after every frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateLateUpdateStateChart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject stateChart= CreateStateChart(parent, storage, iCS_EditorStrings.LateUpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes after every frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateFixedUpdateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_EditorStrings.FixedUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes at a fix frame rate. Independent from the frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateFixedUpdateStateChart(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject stateChart= CreateStateChart(parent, storage, iCS_EditorStrings.FixedUpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes at a fix frame rate. Independent from the frame update.";
        return stateChart;
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
        iCS_EditorObject state= CreateState(parent, storage);
        return state;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateOnEntryModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_EditorStrings.OnEntryNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on entry into this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateOnUpdateModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_EditorStrings.OnUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on every frame this state is active.";
        return module;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject ProcessCreateOnExitModule(iCS_EditorObject parent, iCS_IStorage storage) {
        iCS_EditorObject module= CreateModule(parent, storage, iCS_EditorStrings.OnExitNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on exit from this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    void ProcessDestroyObject(iCS_EditorObject obj, iCS_IStorage storage) {
        DestroyObject(obj, storage);    
    }
	// ----------------------------------------------------------------------
    void ProcessUnhideTransitionGuard(iCS_EditorObject statePort, iCS_IStorage storage) {
        if(statePort.IsInStatePort) statePort= storage[statePort.Source];
        iCS_EditorObject action= null;
        iCS_EditorObject guard= storage.GetTransitionGuardAndAction(statePort, out action);
        storage.Maximize(guard);
    }
	// ----------------------------------------------------------------------
    void ProcessUnhideTransitionAction(iCS_EditorObject statePort, iCS_IStorage storage) {
        if(statePort.IsInStatePort) statePort= storage[statePort.Source];
        iCS_EditorObject action= null;
        storage.GetTransitionGuardAndAction(statePort, out action);
        if(action != null) storage.Maximize(action);
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
    iCS_EditorObject CreateMethod(iCS_EditorObject parent, iCS_IStorage storage, iCS_ReflectionDesc desc) {
        iCS_EditorObject method= storage.CreateMethod(parent.InstanceId, ProcessMenuPosition, desc);
        return method;
    }
	// ----------------------------------------------------------------------
    bool DestroyObject(iCS_EditorObject selectedObject, iCS_IStorage storage) {
        bool isDestroyed= false;
        if(EditorUtility.DisplayDialog("Deleting "+selectedObject.ObjectType, "Are you sure you want to remove "+selectedObject.ObjectType+": "+selectedObject.Name, "Delete", "Cancel")) {
            storage.DestroyInstance(selectedObject.InstanceId);                        
            isDestroyed= true;
        }            
        Reset();
        return isDestroyed;
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
