using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class UK_DynamicMenu {
    internal class MenuContext {
        public string          Command;
        public UK_EditorObject SelectedObject;
        public UK_IStorage      Storage;
        public MenuContext(string command, UK_EditorObject selected, UK_IStorage storage) {
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
    const string OnEntryStr= "+ "+UK_EditorStrings.OnEntryNode;
    const string OnUpdateStr= "+ "+UK_EditorStrings.OnUpdateNode;
    const string OnExitStr= "+ "+UK_EditorStrings.OnExitNode;
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
    public void Update(UK_EditorObject selectedObject, UK_IStorage storage, Vector2 mouseDownPosition) {
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
            case UK_ObjectTypeEnum.Behaviour:        BehaviourMenu(selectedObject, storage); break;
            case UK_ObjectTypeEnum.StateChart:       StateChartMenu(selectedObject, storage); break;
            case UK_ObjectTypeEnum.State:            StateMenu(selectedObject, storage); break;
            case UK_ObjectTypeEnum.Module:           ModuleMenu(selectedObject, storage); break;
            case UK_ObjectTypeEnum.TransitionGuard:  ModuleMenu(selectedObject, storage); break;
            case UK_ObjectTypeEnum.TransitionAction: ModuleMenu(selectedObject, storage); break;
            case UK_ObjectTypeEnum.StaticMethod:     StaticMethodMenu(selectedObject, storage); break;
            case UK_ObjectTypeEnum.Conversion:       StaticMethodMenu(selectedObject, storage); break;
            default: if(selectedObject.IsPort)       PortMenu(selectedObject, storage); break;
        }
    }

	// ----------------------------------------------------------------------
    void BehaviourMenu(UK_EditorObject selectedObject, UK_IStorage storage) {
        // Don't show any menu if behaviour not visible.
        if(storage.IsMinimized(selectedObject) || storage.IsFolded(selectedObject)) return;

        string[] menu= new string[]
        {
            UpdateModuleStr,             
            UpdateStateChartStr,
            LateUpdateModuleStr,        
            LateUpdateStateChartStr,        
            FixedUpdateModuleStr,       
            FixedUpdateStateChartStr,       
            OnGUIStr,              
            OnDrawGizmosStr,
        };
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void ModuleMenu(UK_EditorObject selectedObject, UK_IStorage storage) {
        string[] tmp= null;
        string[] menu= new string[0];
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            // Base menu items
            menu= new string[2];
            menu[0]= ModuleStr;
            menu[1]= StateChartStr; 
            // Enable port
            bool hasEnablePort= storage.HasEnablePort(selectedObject);
            if(!hasEnablePort) {
                tmp= new string[menu.Length+1];
                menu.CopyTo(tmp, 0);
                tmp[menu.Length]= EnablePortStr;
                menu= tmp;
            }
        }
        // Fold/Expand menu items
        if(!storage.IsMinimized(selectedObject)) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            if(storage.IsFolded(selectedObject)) {
                tmp[menu.Length+1]= UnfoldStr;
            } else {
                tmp[menu.Length+1]= FoldStr;            
            }
            menu= tmp;            
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
    void StateChartMenu(UK_EditorObject selectedObject, UK_IStorage storage) {
        string[] menu= new string[0];
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            menu= new string[1];
            menu[0]= StateStr; 
        }
        // Fold/Expand menu items
        string[] tmp= null;
        if(!storage.IsMinimized(selectedObject)) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            if(storage.IsFolded(selectedObject)) {
                tmp[menu.Length+1]= UnfoldStr;
            } else {
                tmp[menu.Length+1]= FoldStr;            
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
    void StateMenu(UK_EditorObject selectedObject, UK_IStorage storage) {
        string[] menu= new string[0];
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            menu= new string[6];
            menu[0]= OnEntryStr;
            menu[1]= OnUpdateStr;
            menu[2]= OnExitStr;
            menu[3]= StateStr;
            menu[4]= SeparatorStr;
            menu[5]= SetAsEntryStr;
        }
        // Fold/Expand menu items
        string[] tmp= null;
        if(!storage.IsMinimized(selectedObject)) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            if(storage.IsFolded(selectedObject)) {
                tmp[menu.Length+1]= UnfoldStr;
            } else {
                tmp[menu.Length+1]= FoldStr;            
            }
            menu= tmp;            
        }
        // Delete menu item.tmp= new string[menu.Length+2];
        tmp= new string[menu.Length+2];
        menu.CopyTo(tmp, 0);
        tmp[menu.Length]= SeparatorStr;
        tmp[menu.Length+1]= DeleteStr;
        menu= tmp;
        ShowMenu(menu, selectedObject, storage);
    }
    
	// ----------------------------------------------------------------------
    void StaticMethodMenu(UK_EditorObject selectedObject, UK_IStorage storage) {
        if(storage.EditorObjects[selectedObject.ParentId].IsModule) {
            ShowMenu(new string[]{DeleteStr}, selectedObject, storage);            
        }
    }
	// ----------------------------------------------------------------------
    void PortMenu(UK_EditorObject selectedObject, UK_IStorage storage) {
        string[] menu= new string[0];
        // Allow to publish port if the grand-parent is a module.
        UK_EditorObject parent= storage.EditorObjects[selectedObject.ParentId];
        UK_EditorObject grandParent= storage.EditorObjects[parent.ParentId];
        if(grandParent != null && grandParent.IsModule) {
            if(!(selectedObject.IsInputPort && storage.IsValid(selectedObject.Source))) {
                menu= new string[]{PublishPortStr};                
            }
        }
        // State Port.
        if(selectedObject.IsStatePort) {
            int i= menu.Length;
            if(selectedObject.IsOutStatePort) {
                UK_EditorObject action= null;
                UK_EditorObject guard= storage.GetTransitionGuardAndAction(selectedObject, out action);
                if(guard.IsHidden) {
                    string[] tmp= new string[i+1];
                    tmp[i]= UnhideTransitionGuardStr;
                    menu= tmp;                        
                }
                if(action.IsHidden) {
                    string[] tmp= new string[i+1];
                    tmp[i]= UnhideTransitionActionStr;
                    menu= tmp;                        
                }
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
    void ShowMenu(string[] menu, UK_EditorObject selected, UK_IStorage storage) {
        ShowMenu(menu, MenuPosition, selected, storage);
    }
    void ShowMenu(string[] menu, Vector2 pos, UK_EditorObject selected, UK_IStorage storage) {
        int sepCnt= 0;
        GenericMenu gMenu= new GenericMenu();
        foreach(var item in menu) {
            if(item == SeparatorStr) {
                if(sepCnt != 0) gMenu.AddSeparator("");
                sepCnt= 0;
            } else {
                gMenu.AddItem(new GUIContent(item), false, ProcessMenu, new MenuContext(item, selected, storage));                
                ++sepCnt;
            }
        }
        gMenu.ShowAsContext();
        Reset();
    }
	// ----------------------------------------------------------------------
    // Combines the company/package/function name into menu conforming strings.
    string[] GetFunctionMenu() {
        return UK_DataBase.BuildMenu();
    }
    UK_ReflectionDesc GetReflectionDescFromMenuCommand(string menuCommand) {
        menuCommand= UK_TextUtil.StripBeforeIdent(menuCommand);
        string[] idents= menuCommand.Split(new char[1]{'/'});
        if(idents.Length < 3) return null;
        string company= idents[0];
        string package= idents[1];
        string function= idents[2];
        string signature= null;
        if(idents.Length >= 4) signature= idents[3];
        return UK_DataBase.GetDescriptor(company, package, function, signature);
    }
    
    // ======================================================================
    // Menu processing
	// ----------------------------------------------------------------------
    void ProcessMenu(object obj) {
        MenuContext context= obj as MenuContext;
        UK_EditorObject selectedObject= context.SelectedObject;
        UK_IStorage storage= context.Storage;
        storage.RegisterUndo(context.Command);
        switch(context.Command) {
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
                UK_EditorObject port= storage.CreatePort(UK_EditorStrings.EnablePort, selectedObject.InstanceId, typeof(bool), UK_ObjectTypeEnum.EnablePort);
                port.IsNameEditable= false;
                break;
            }
            case PublishPortStr: {
                UK_EditorObject parent= storage.GetParent(selectedObject);
                UK_EditorObject grandParent= storage.GetParent(parent);
                int grandParentId= grandParent.InstanceId;
                if(selectedObject.IsInputPort) {
                    UK_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, UK_ObjectTypeEnum.InDynamicModulePort);
                    storage.SetSource(selectedObject, port);
                    port.LocalPosition= new Rect(0, parent.LocalPosition.y+selectedObject.LocalPosition.y, 0, 0);
                } else {
                    UK_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, UK_ObjectTypeEnum.OutDynamicModulePort);
                    storage.SetSource(port, selectedObject);
                    port.LocalPosition= new Rect(grandParent.LocalPosition.width, parent.LocalPosition.y+selectedObject.LocalPosition.y, 0, 0);
                }
                grandParent.IsDirty= true;
                break;                
            }
            default: {
                UK_ReflectionDesc desc= GetReflectionDescFromMenuCommand(context.Command);
                if(desc != null) {
                    CreateMethod(context.SelectedObject, context.Storage, desc);                                           
                }
                break;                
            }
        }
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateUpdateModule(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject module= CreateModule(parent, storage, UK_EditorStrings.UpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on every frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateUpdateStateChart(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject stateChart= CreateStateChart(parent, storage, UK_EditorStrings.UpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes on every frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateLateUpdateModule(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject module= CreateModule(parent, storage, UK_EditorStrings.LateUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes after every frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateLateUpdateStateChart(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject stateChart= CreateStateChart(parent, storage, UK_EditorStrings.LateUpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes after every frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateFixedUpdateModule(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject module= CreateModule(parent, storage, UK_EditorStrings.FixedUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes at a fix frame rate. Independent from the frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateFixedUpdateStateChart(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject stateChart= CreateStateChart(parent, storage, UK_EditorStrings.FixedUpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes at a fix frame rate. Independent from the frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateModule(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject module= CreateModule(parent, storage);
        return module;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateStateChart(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject stateChart= CreateStateChart(parent, storage);
        return stateChart;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateState(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject state= CreateState(parent, storage);
        return state;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateOnEntryModule(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject module= CreateModule(parent, storage, UK_EditorStrings.OnEntryNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on entry into this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateOnUpdateModule(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject module= CreateModule(parent, storage, UK_EditorStrings.OnUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on every frame this state is active.";
        return module;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject ProcessCreateOnExitModule(UK_EditorObject parent, UK_IStorage storage) {
        UK_EditorObject module= CreateModule(parent, storage, UK_EditorStrings.OnExitNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on exit from this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    void ProcessDestroyObject(UK_EditorObject obj, UK_IStorage storage) {
        DestroyObject(obj, storage);    
    }
	// ----------------------------------------------------------------------
    void ProcessUnhideTransitionGuard(UK_EditorObject statePort, UK_IStorage storage) {
        if(statePort.IsInStatePort) statePort= storage[statePort.Source];
        UK_EditorObject action= null;
        UK_EditorObject guard= storage.GetTransitionGuardAndAction(statePort, out action);
        storage.Maximize(guard);
    }
	// ----------------------------------------------------------------------
    void ProcessUnhideTransitionAction(UK_EditorObject statePort, UK_IStorage storage) {
        if(statePort.IsInStatePort) statePort= storage[statePort.Source];
        UK_EditorObject action= null;
        storage.GetTransitionGuardAndAction(statePort, out action);
        if(action != null) storage.Maximize(action);
    }

    // ======================================================================
    // Creation Utilities
	// ----------------------------------------------------------------------
    UK_EditorObject CreateModule(UK_EditorObject parent, UK_IStorage storage, string name= "", bool nameEditable= true) {
        UK_EditorObject module= storage.CreateModule(parent.InstanceId, ProcessMenuPosition, name);
        module.IsNameEditable= nameEditable;
        return module;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject CreateStateChart(UK_EditorObject parent, UK_IStorage storage, string name= "", bool nameEditable= true) {
        UK_EditorObject stateChart= storage.CreateStateChart(parent.InstanceId, ProcessMenuPosition, name);
        stateChart.IsNameEditable= nameEditable;
        return stateChart;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject CreateState(UK_EditorObject parent, UK_IStorage storage, string name= "") {
        return storage.CreateState(parent.InstanceId, ProcessMenuPosition, name);
    }
	// ----------------------------------------------------------------------
    UK_EditorObject CreateMethod(UK_EditorObject parent, UK_IStorage storage, UK_ReflectionDesc desc) {
        UK_EditorObject method= storage.CreateMethod(parent.InstanceId, ProcessMenuPosition, desc);
        return method;
    }
	// ----------------------------------------------------------------------
    bool DestroyObject(UK_EditorObject selectedObject, UK_IStorage storage) {
        bool isDestroyed= false;
        if(EditorUtility.DisplayDialog("Deleting "+selectedObject.ObjectType, "Are you sure you want to remove "+selectedObject.ObjectType+": "+selectedObject.Name, "Delete", "Cancel")) {
            storage.DestroyInstance(selectedObject.InstanceId);                        
            isDestroyed= true;
        }            
        Reset();
        return isDestroyed;
    }
}
