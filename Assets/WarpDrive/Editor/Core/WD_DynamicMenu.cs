using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class WD_DynamicMenu {
    internal class MenuContext {
        public string          Command;
        public WD_EditorObject SelectedObject;
        public WD_IStorage      Storage;
        public MenuContext(string command, WD_EditorObject selected, WD_IStorage storage) {
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
    const string SubStateStr= "+ SubState";
    const string UpdateModuleStr= "+ Update/Module";
    const string UpdateStateChartStr= "+ Update/StateChart";
    const string LateUpdateModuleStr= "+ LateUpdate/Module";
    const string LateUpdateStateChartStr= "+ LateUpdate/StateChart";
    const string FixedUpdateModuleStr= "+ FixedUpdate/Module";
    const string FixedUpdateStateChartStr= "+ FixedUpdate/StateChart";
    const string SetAsEntryStr="Set as Entry";
    const string OnGUIStr= "+ OnGUI";
    const string OnDrawGizmosStr= "+ OnDrawGizmos";
    const string OnEntryStr= "+ "+WD_EditorStrings.OnEntryNode;
    const string OnUpdateStr= "+ "+WD_EditorStrings.OnUpdateNode;
    const string OnExitStr= "+ "+WD_EditorStrings.OnExitNode;
    const string PublishPortStr= "Publish on Module";
    const string EnablePortStr= "+ Enable Port";
    const string TransitionExitStr= "+ Transition Exit";
    const string TransitionEntryActionStr= "+ Entry Action";
    const string TransitionEntryDataCollectorStr= "+ Data Collector";
    const string UnhideTransitionEntryStr= "Show Transition Entry";
    const string UnhideTransitionExitStr= "Show Transition Exit";
    const string SeparatorStr= "";

    // ======================================================================
    // Menu state management
	// ----------------------------------------------------------------------
    void Reset() {
        MenuPosition= Vector2.zero;
    }
    
	// ----------------------------------------------------------------------
    public void Update(WD_EditorObject selectedObject, WD_IStorage storage, Vector2 mouseDownPosition) {
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
            case WD_ObjectTypeEnum.Behaviour:  BehaviourMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.StateChart: StateChartMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.State:      StateMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.Module:     ModuleMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.Function:   FunctionMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.Conversion: FunctionMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.Class:      ClassMenu(selectedObject, storage); break;
            default: if(selectedObject.IsPort) PortMenu(selectedObject, storage); break;
        }
    }

	// ----------------------------------------------------------------------
    void BehaviourMenu(WD_EditorObject selectedObject, WD_IStorage storage) {
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
    void ModuleMenu(WD_EditorObject selectedObject, WD_IStorage storage) {
        if(storage.IsTransitionEntryModule(selectedObject)) {
            TransitionEntryModuleMenu(selectedObject, storage);
            return;
        }
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
            functionMenu.CopyTo(tmp, menu.Length+1);
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
    void TransitionEntryModuleMenu(WD_EditorObject selectedObject, WD_IStorage storage) {
        string[] tmp= null;
        string[] menu= new string[0];
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            // Transition entry Sub-components.
            WD_EditorObject entryAction= storage.GetActionModuleFromTransitionEntryModule(selectedObject);
            WD_EditorObject dataCollector= storage.GetDataCollectorModuleFromTransitionEntryModule(selectedObject);
            if(entryAction == null || dataCollector == null) {
                tmp= new string[menu.Length+(entryAction==null?1:0)+(dataCollector==null?1:0)];
                menu.CopyTo(tmp, 0);
                int idx= menu.Length;
                if(entryAction == null) {
                    tmp[idx++]= TransitionEntryActionStr;
                }
                if(dataCollector == null) {
                    tmp[idx]= TransitionEntryDataCollectorStr;
                }
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
    void StateChartMenu(WD_EditorObject selectedObject, WD_IStorage storage) {
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
    void StateMenu(WD_EditorObject selectedObject, WD_IStorage storage) {
        string[] menu= new string[0];
        if(!storage.IsMinimized(selectedObject) && !storage.IsFolded(selectedObject)) {
            menu= new string[6];
            menu[0]= OnEntryStr;
            menu[1]= OnUpdateStr;
            menu[2]= OnExitStr;
            menu[3]= SubStateStr;
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
    void FunctionMenu(WD_EditorObject selectedObject, WD_IStorage storage) {
        if(storage.EditorObjects[selectedObject.ParentId].IsModule) {
            ShowMenu(new string[]{DeleteStr}, selectedObject, storage);            
        }
    }
	// ----------------------------------------------------------------------
    void ClassMenu(WD_EditorObject selectedObject, WD_IStorage storage) {
        string[] menu= new string[0];
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
        // Delete menu item.
        if(storage.EditorObjects[selectedObject.ParentId].IsModule) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            tmp[menu.Length+1]= DeleteStr;
            menu= tmp;
        }
        ShowMenu(menu, selectedObject, storage);            
    }
	// ----------------------------------------------------------------------
    void PortMenu(WD_EditorObject selectedObject, WD_IStorage storage) {
        string[] menu= new string[0];
        // Allow to publish port if the grand-parent is a module.
        WD_EditorObject parent= storage.EditorObjects[selectedObject.ParentId];
        WD_EditorObject grandParent= storage.EditorObjects[parent.ParentId];
        if(grandParent != null && grandParent.IsModule) {
            if(!(selectedObject.IsInputPort && storage.IsValid(selectedObject.Source))) {
                menu= new string[]{PublishPortStr};                
            }
        }
        // State Port.
        if(selectedObject.IsStatePort) {
            int i= menu.Length;
            if(selectedObject.IsOutStatePort) {
                WD_EditorObject sourcePort= storage.GetSource(selectedObject);
                WD_EditorObject entryModule= storage.GetParent(sourcePort);
                if(entryModule.IsHidden) {
                    string[] tmp= new string[i+1];
                    tmp[i]= UnhideTransitionEntryStr;
                    menu= tmp;                        
                }
            } else {
                WD_EditorObject connected= storage.FindAConnectedPort(selectedObject);
                if(connected == null) {
                    string[] tmp= new string[i+1];
                    tmp[i]= TransitionExitStr;
                    menu= tmp;
                } else {
                    WD_EditorObject exitModule= storage.GetParent(connected);
                    if(exitModule.IsHidden) {
                        string[] tmp= new string[i+1];
                        tmp[i]= UnhideTransitionExitStr;
                        menu= tmp;                                                
                    }
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
    void ShowMenu(string[] menu, WD_EditorObject selected, WD_IStorage storage) {
        ShowMenu(menu, MenuPosition, selected, storage);
    }
    void ShowMenu(string[] menu, Vector2 pos, WD_EditorObject selected, WD_IStorage storage) {
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
        List<string> result= new List<string>();
        string[] companies= WD_DataBase.GetCompanies();
        foreach(var company in companies) {
            string[] packages= WD_DataBase.GetPackages(company);
            foreach(var package in packages) {
                string[] functions= WD_DataBase.GetFunctions(company, package);
                foreach(var function in functions) {
                    result.Add(company+"/"+package+"/"+function);
                }
            }
        }
        return result.ToArray();
    }
    // Extracts the company name from the menu conforming string.
    string GetCompanyFromMenuItem(string item) {
        int end= item.IndexOf('/');
        if(end < 0) return null;
        return item.Substring(0, end);
    }
    // Extracts the package name from the menu conforming string.
    string GetPackageFromMenuItem(string item) {
        int start= item.IndexOf('/');
        if(start < 0) return null;
        ++start;
        int end= item.IndexOf('/', start);
        if(end < 0) return null;
        return item.Substring(start, end-start);
    }
    // Extracts the function name from the menu conforming string.
    string GetFunctionFromMenuItem(string item) {
        int skip= item.IndexOf('/');
        if(skip < 0) return null;
        int start= item.IndexOf('/', skip+1);
        if(start < 0) return null;
        ++start;
        int end= item.Length;
        return item.Substring(start, end-start);
    }
    
    // ======================================================================
    // Menu processing
	// ----------------------------------------------------------------------
    void ProcessMenu(object obj) {
        MenuContext context= obj as MenuContext;
        WD_EditorObject selectedObject= context.SelectedObject;
        WD_IStorage storage= context.Storage;
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
            case SubStateStr:               ProcessCreateState(selectedObject, storage);  break;
            case TransitionExitStr:         CreateTransitionExit(selectedObject, storage); break;
            case FoldStr:                   storage.Fold(selectedObject); break;
            case UnfoldStr:                 storage.Unfold(selectedObject); break;
            case DeleteStr:                 ProcessDestroyObject(selectedObject, storage); break;
            case UnhideTransitionEntryStr:  ProcessUnhideTransitionEntry(selectedObject, storage); break;
            case UnhideTransitionExitStr:   ProcessUnhideTransitionExit(selectedObject, storage); break;
            case EnablePortStr: {
                WD_EditorObject port= storage.CreatePort(WD_EditorStrings.EnablePort, selectedObject.InstanceId, typeof(bool), WD_ObjectTypeEnum.EnablePort);
                port.IsNameEditable= false;
                break;
            }
            case PublishPortStr: {
                WD_EditorObject parent= storage.GetParent(selectedObject);
                WD_EditorObject grandParent= storage.GetParent(parent);
                int grandParentId= grandParent.InstanceId;
                if(selectedObject.IsInputPort) {
                    WD_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, WD_ObjectTypeEnum.InDynamicModulePort);
                    storage.SetSource(selectedObject, port);
                    port.LocalPosition= new Rect(0, parent.LocalPosition.y+selectedObject.LocalPosition.y, 0, 0);
                } else {
                    WD_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, WD_ObjectTypeEnum.OutDynamicModulePort);
                    storage.SetSource(port, selectedObject);
                    port.LocalPosition= new Rect(grandParent.LocalPosition.width, parent.LocalPosition.y+selectedObject.LocalPosition.y, 0, 0);
                }
                grandParent.IsDirty= true;
                break;                
            }
            case TransitionEntryActionStr: {
                storage.CreateTransitionEntryAction(selectedObject);
                break;
            }
            case TransitionEntryDataCollectorStr: {
                storage.CreateTransitionDataCollector(selectedObject);
                break;
            }
            default: {
                string company= GetCompanyFromMenuItem(context.Command);
                string package= GetPackageFromMenuItem(context.Command);
                string function= GetFunctionFromMenuItem(context.Command);
                if(company != null && package != null && function != null) {
                    WD_ReflectionBaseDesc desc= WD_DataBase.GetDescriptor(company, package, function);
                    if(desc != null) {
                        CreateFunction(context.SelectedObject, context.Storage, desc);                                           
                    }
                }
                break;                
            }
        }
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateUpdateModule(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject module= CreateModule(parent, storage, WD_EditorStrings.UpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on every frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateUpdateStateChart(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject stateChart= CreateStateChart(parent, storage, WD_EditorStrings.UpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes on every frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateLateUpdateModule(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject module= CreateModule(parent, storage, WD_EditorStrings.LateUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes after every frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateLateUpdateStateChart(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject stateChart= CreateStateChart(parent, storage, WD_EditorStrings.LateUpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes after every frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateFixedUpdateModule(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject module= CreateModule(parent, storage, WD_EditorStrings.FixedUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes at a fix frame rate. Independent from the frame update.";
        return module;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateFixedUpdateStateChart(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject stateChart= CreateStateChart(parent, storage, WD_EditorStrings.FixedUpdateNode, false);
        stateChart.IsNameEditable= false;
        stateChart.ToolTip= "Executes at a fix frame rate. Independent from the frame update.";
        return stateChart;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateModule(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject module= CreateModule(parent, storage);
        return module;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateStateChart(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject stateChart= CreateStateChart(parent, storage);
        return stateChart;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateState(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject state= CreateState(parent, storage);
        return state;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateOnEntryModule(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject module= CreateModule(parent, storage, WD_EditorStrings.OnEntryNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on entry into this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateOnUpdateModule(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject module= CreateModule(parent, storage, WD_EditorStrings.OnUpdateNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on every frame this state is active.";
        return module;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject ProcessCreateOnExitModule(WD_EditorObject parent, WD_IStorage storage) {
        WD_EditorObject module= CreateModule(parent, storage, WD_EditorStrings.OnExitNode, false);
        module.IsNameEditable= false;
        module.ToolTip= "Executes on exit from this state.";
        return module;
    }
	// ----------------------------------------------------------------------
    void ProcessDestroyObject(WD_EditorObject obj, WD_IStorage storage) {
        DestroyObject(obj, storage);    
    }
	// ----------------------------------------------------------------------
    void ProcessUnhideTransitionEntry(WD_EditorObject outStatePort, WD_IStorage storage) {
        WD_EditorObject sourcePort= storage.GetSource(outStatePort);
        WD_EditorObject entryModule= storage.GetParent(sourcePort);
        storage.Maximize(entryModule);
    }
	// ----------------------------------------------------------------------
    void ProcessUnhideTransitionExit(WD_EditorObject inStatePort, WD_IStorage storage) {
        WD_EditorObject connected= storage.FindAConnectedPort(inStatePort);
        WD_EditorObject exitModule= storage.GetParent(connected);
        storage.Maximize(exitModule);
    }

    // ======================================================================
    // Creation Utilities
	// ----------------------------------------------------------------------
    WD_EditorObject CreateModule(WD_EditorObject parent, WD_IStorage storage, string name= "", bool nameEditable= true) {
        WD_EditorObject module= storage.CreateModule(parent.InstanceId, ProcessMenuPosition, name);
        module.IsNameEditable= nameEditable;
        return module;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject CreateStateChart(WD_EditorObject parent, WD_IStorage storage, string name= "", bool nameEditable= true) {
        WD_EditorObject stateChart= storage.CreateStateChart(parent.InstanceId, ProcessMenuPosition, name);
        stateChart.IsNameEditable= nameEditable;
        return stateChart;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject CreateState(WD_EditorObject parent, WD_IStorage storage, string name= "") {
        return storage.CreateState(parent.InstanceId, ProcessMenuPosition, name);
    }
	// ----------------------------------------------------------------------
    WD_EditorObject CreateFunction(WD_EditorObject parent, WD_IStorage storage, WD_ReflectionBaseDesc desc) {
        WD_EditorObject function= storage.CreateFunction(parent.InstanceId, ProcessMenuPosition, desc);
        return function;
    }
	// ----------------------------------------------------------------------
    WD_EditorObject CreateTransitionExit(WD_EditorObject inStatePort, WD_IStorage storage) {
        return storage.CreateTransitionExit(inStatePort);
    }
	// ----------------------------------------------------------------------
    bool DestroyObject(WD_EditorObject selectedObject, WD_IStorage storage) {
        bool isDestroyed= false;
        if(storage.IsTransitionExitModule(selectedObject)) {
            WD_EditorObject entryModule= storage.GetTransitionEntryModule(selectedObject);
            WD_EditorObject dataCollector= storage.GetDataCollectorModuleFromTransitionEntryModule(entryModule);
            if(dataCollector != null) {
                if(EditorUtility.DisplayDialog("Deleting Transition Exit & Associated Data Collector",
                                               "Transition Data Collector cannot exist without a transition exit module.  Are you sure you want to remove BOTH the DATA COLLECTOR and the Transition Exit.", "Delete DataCollector & Transition Exit", "Cancel")) {
                    storage.DestroyInstance(dataCollector.InstanceId);
                    storage.DestroyInstance(selectedObject.InstanceId);                        
                    Reset();
                    return true;
                }            
                Reset();
                return false;
            }
        }
        if(EditorUtility.DisplayDialog("Deleting "+selectedObject.ObjectType, "Are you sure you want to remove "+selectedObject.ObjectType+": "+selectedObject.Name, "Delete", "Cancel")) {
            storage.DestroyInstance(selectedObject.InstanceId);                        
            isDestroyed= true;
        }            
        Reset();
        return isDestroyed;
    }
}
