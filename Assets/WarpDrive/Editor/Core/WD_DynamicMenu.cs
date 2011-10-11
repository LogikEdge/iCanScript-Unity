using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class WD_DynamicMenu {
    internal class MenuContext {
        public string          Command;
        public WD_EditorObject SelectedObject;
        public WD_Storage      Storage;
        public MenuContext(string command, WD_EditorObject selected, WD_Storage storage) {
            Command= command;
            SelectedObject= selected;
            Storage= storage;
        }
    }
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    public bool IsActive= false;
    Vector2     MenuPosition= Vector2.zero;
    
    // ======================================================================
    // Menu Items
	// ----------------------------------------------------------------------
    const string DeleteStr= "Delete";
    const string ModuleStr= "Module";
    const string StateChartStr= "State Chart";
    const string StateStr= "State";
    const string EntryStateStr= "Entry State";
    const string SubStateStr= "SubState";
    const string UpdateModuleStr= "Update/Module";
    const string UpdateStateChartStr= "Update/StateChart";
    const string LateUpdateModuleStr= "LateUpdate/Module";
    const string LateUpdateStateChartStr= "LateUpdate/StateChart";
    const string FixedUpdateModuleStr= "FixedUpdate/Module";
    const string FixedUpdateStateChartStr= "FixedUpdate/StateChart";
    const string OnGUIStr= "OnGUI";
    const string OnDrawGizmosStr= "OnDrawGizmos";
    const string OnEntryStr= "OnEntry";
    const string OnUpdateStr= "OnUpdate";
    const string OnExitStr= "OnExit";
    const string PublishPortStr= "Publish on Module";

    // ======================================================================
    // Menu state management
	// ----------------------------------------------------------------------
    void Reset() {
        MenuPosition= Vector2.zero;
    }
    
	// ----------------------------------------------------------------------
    public void Update(WD_EditorObject selectedObject, WD_Storage storage, Vector2 mouseDownPosition) {
        // Update mouse position if not already done.
        if(MenuPosition == Vector2.zero) MenuPosition= mouseDownPosition;

        // Nothing to show if menu is inactive.
        if(selectedObject == null || MenuPosition != mouseDownPosition) {
            Reset();
            return;
        }

        // Process the menu state.
        switch(selectedObject.ObjectType) {
            case WD_ObjectTypeEnum.Behaviour:  BehaviourMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.StateChart: StateChartMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.State:      StateMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.Module:     ModuleMenu(selectedObject, storage); break;
            case WD_ObjectTypeEnum.Function:   FunctionMenu(selectedObject, storage); break;
            default: if(selectedObject.IsPort) PortMenu(selectedObject, storage); break;
        }
    }

	// ----------------------------------------------------------------------
    void BehaviourMenu(WD_EditorObject selectedObject, WD_Storage storage) {
        string[] menu= new string[]
            { UpdateModuleStr,             
              UpdateStateChartStr,
              LateUpdateModuleStr,        
              LateUpdateStateChartStr,        
              FixedUpdateModuleStr,       
              FixedUpdateStateChartStr,       
              OnGUIStr,              
              OnDrawGizmosStr
            };
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void ModuleMenu(WD_EditorObject selectedObject, WD_Storage storage) {
        string[] moduleMenu= new string[]
        {
            ModuleStr,
            StateChartStr,
        };
        string[] functionMenu= GetFunctionMenu();
        string[] menu= new string[moduleMenu.Length+functionMenu.Length];
        moduleMenu.CopyTo(menu, 0);
        functionMenu.CopyTo(menu, moduleMenu.Length);
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateChartMenu(WD_EditorObject selectedObject, WD_Storage storage) {
        string[] menu= new string[]
        {
            StateStr,
            EntryStateStr
        };
        ShowMenu(menu, selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void StateMenu(WD_EditorObject selectedObject, WD_Storage storage) {
        string[] menu= new string[]
        {
            OnEntryStr,
            OnUpdateStr,
            OnExitStr,
            SubStateStr
        };
        ShowMenu(menu, selectedObject, storage);
    }
    
	// ----------------------------------------------------------------------
    void FunctionMenu(WD_EditorObject selectedObject, WD_Storage storage) {
        ShowMenu(new string[0], selectedObject, storage);
    }
	// ----------------------------------------------------------------------
    void PortMenu(WD_EditorObject selectedObject, WD_Storage storage) {
        WD_EditorObject parent= storage.EditorObjects[selectedObject.ParentId];
        WD_EditorObject grandParent= storage.EditorObjects[parent.ParentId];
        if(grandParent != null && grandParent.IsModule) {
            ShowMenu(new string[]{PublishPortStr}, selectedObject, storage);
        }
    }

    // ======================================================================
    // Menu Utilities
	// ----------------------------------------------------------------------
    void ShowMenu(string[] menu, WD_EditorObject selected, WD_Storage storage) {
        ShowMenu(menu, MenuPosition, selected, storage);
    }
	// ----------------------------------------------------------------------
    void ProcessMenu(object obj) {
        MenuContext context= obj as MenuContext;
        switch(context.Command) {
            case UpdateModuleStr:           CreateModule(context.SelectedObject, context.Storage, "Update"); break;
            case UpdateStateChartStr:       CreateStateChart(context.SelectedObject, context.Storage, "Update"); break;
            case FixedUpdateModuleStr:      CreateModule(context.SelectedObject, context.Storage, "FixedUpdate"); break;
            case FixedUpdateStateChartStr:  CreateStateChart(context.SelectedObject, context.Storage, "FixedUpdate"); break;
            case LateUpdateModuleStr:       CreateModule(context.SelectedObject, context.Storage, "LateUpdate"); break;
            case LateUpdateStateChartStr:   CreateStateChart(context.SelectedObject, context.Storage, "LateUpdate"); break;
            case ModuleStr:                 CreateModule(context.SelectedObject, context.Storage); break;
            case StateChartStr:             CreateStateChart(context.SelectedObject, context.Storage); break;
            case StateStr:                  CreateState (context.SelectedObject, context.Storage);  break;
            case OnEntryStr:                CreateModule(context.SelectedObject, context.Storage, OnEntryStr); break;
            case OnUpdateStr:               CreateModule(context.SelectedObject, context.Storage, OnUpdateStr); break;
            case OnExitStr:                 CreateModule(context.SelectedObject, context.Storage, OnExitStr); break;
            case SubStateStr:               CreateState (context.SelectedObject, context.Storage);  break;
            case DeleteStr:                 DestroySelectedObject(context.SelectedObject, context.Storage); break;
            case PublishPortStr:
                WD_EditorObjectMgr editorObjects= context.Storage.EditorObjects;
                WD_EditorObject selected= context.SelectedObject;
                WD_EditorObject parent= editorObjects[selected.ParentId];
                int grandParent= editorObjects[parent.ParentId].InstanceId;
                if(selected.IsInputPort) {
                    WD_EditorObject port= editorObjects.CreatePort(selected.Name, grandParent, selected.RuntimeType, WD_ObjectTypeEnum.InModulePort);
                    editorObjects.SetSource(selected, port);
                } else {
                    WD_EditorObject port= editorObjects.CreatePort(selected.Name, grandParent, selected.RuntimeType, WD_ObjectTypeEnum.OutModulePort);
                    editorObjects.SetSource(port, selected);
                }
                break;
            default:
                string company= GetCompanyFromMenuItem(context.Command);
                string package= GetPackageFromMenuItem(context.Command);
                string function= GetFunctionFromMenuItem(context.Command);
                if(company != null && package != null && function != null) {
                    WD_BaseDesc desc= WD_DataBase.GetDescriptor(company, package, function);
                    if(desc != null) {
                        CreateFunction(context.SelectedObject, context.Storage, desc);                                           
                    }
                }
                break;
        }
    }
    void ShowMenu(string[] menu, Vector2 pos, WD_EditorObject selected, WD_Storage storage) {
        GenericMenu gMenu= new GenericMenu();
        foreach(var item in menu) {
            gMenu.AddItem(new GUIContent(item), false, ProcessMenu, new MenuContext(item, selected, storage));
        }
        gMenu.AddSeparator("");
        gMenu.AddItem(new GUIContent(DeleteStr), false, ProcessMenu, new MenuContext(DeleteStr, selected, storage));
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
    // Creation Utilities
	// ----------------------------------------------------------------------
    void CreateModule(WD_EditorObject parent, WD_Storage storage, string name= "") {
        storage.EditorObjects.CreateModule(parent.InstanceId, MenuPosition, name);
    }
	// ----------------------------------------------------------------------
    void CreateStateChart(WD_EditorObject parent, WD_Storage storage, string name= "") {
        storage.EditorObjects.CreateStateChart(parent.InstanceId, MenuPosition, name);
    }
	// ----------------------------------------------------------------------
    void CreateState(WD_EditorObject parent, WD_Storage storage, string name= "") {
        storage.EditorObjects.CreateState(parent.InstanceId, MenuPosition, name);
    }
	// ----------------------------------------------------------------------
    void CreateFunction(WD_EditorObject parent, WD_Storage storage, WD_BaseDesc desc) {
        storage.EditorObjects.CreateFunction(parent.InstanceId, MenuPosition, desc);
    }
	// ----------------------------------------------------------------------
    bool DestroySelectedObject(WD_EditorObject selectedObject, WD_Storage storage) {
        bool isDestroyed= false;
        if(EditorUtility.DisplayDialog("Deleting "+selectedObject.ObjectType, "Are you sure you want to remove "+selectedObject.ObjectType+": "+selectedObject.Name, "Delete", "Cancel")) {
            storage.EditorObjects.DestroyInstance(selectedObject.InstanceId);                        
            isDestroyed= true;
        }
        Reset();
        return isDestroyed;
    }
}
