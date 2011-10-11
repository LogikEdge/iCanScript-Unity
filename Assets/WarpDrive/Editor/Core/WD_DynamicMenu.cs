using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class WD_DynamicMenu {
    enum MenuStateEnum { Idle, Behaviour, Module, StateChart, State, Function, Port }
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
    MenuStateEnum   CurrentState= MenuStateEnum.Idle;
    Vector2         MenuPosition= Vector2.zero;
    
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
    const string PublishStr= "Publish to Parent";
    const string MoreStr= "More ...";

    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    public bool IsActive { get { return CurrentState != MenuStateEnum.Idle; }}

	// ----------------------------------------------------------------------
    void Reset() {
        CurrentState= MenuStateEnum.Idle;
        MenuPosition= Vector2.zero;
    }
    
	// ----------------------------------------------------------------------
    // Activate the dynamic menu if it is not already active.
    public void Activate(WD_EditorObject selectedObject) {
        if(!IsActive && selectedObject != null) {
            switch(selectedObject.ObjectType) {
                case WD_ObjectTypeEnum.Behaviour:  CurrentState= MenuStateEnum.Behaviour; break;
                case WD_ObjectTypeEnum.StateChart: CurrentState= MenuStateEnum.StateChart; break;
                case WD_ObjectTypeEnum.State:      CurrentState= MenuStateEnum.State; break;
                case WD_ObjectTypeEnum.Module:     CurrentState= MenuStateEnum.Module; break;
                case WD_ObjectTypeEnum.Function:   CurrentState= MenuStateEnum.Function; break;
                default: if(selectedObject.IsPort) CurrentState= MenuStateEnum.Port; break;
            }
        }
    }
    
	// ----------------------------------------------------------------------
    public void Deactivate() {
        Reset();
    }
    
	// ----------------------------------------------------------------------
    public void Update(WD_EditorObject selectedObject, WD_Storage storage, Vector2 mouseDownPosition) {
        // Update mouse position if not already done.
        if(MenuPosition == Vector2.zero) MenuPosition= mouseDownPosition;

        // Nothing to show if menu is inactive.
        if(selectedObject == null || MenuPosition != mouseDownPosition || !IsActive) {
            Reset();
            return;
        }

        // Process the menu state.
        switch(CurrentState) {
            case MenuStateEnum.Idle:
                break;
            case MenuStateEnum.Behaviour:
                BehaviourMenu(selectedObject, storage);
                break;
            case MenuStateEnum.Module:
                ModuleMenu(selectedObject, storage);
                break;
            case MenuStateEnum.StateChart:
                StateChartMenu(selectedObject, storage);
                break;
            case MenuStateEnum.State:
                StateMenu(selectedObject, storage);
                break;
            case MenuStateEnum.Function:
                FunctionMenu(selectedObject, storage);
                break;
            case MenuStateEnum.Port:
                PortMenu(selectedObject, storage);
                break;
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
    void BehaviourAdditionalMenu() {
        
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
        string[] menu= new string[]
        {
            PublishStr,
        };
        ShowMenu(menu, selectedObject, storage);
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
            default: Reset(); break;
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

    // ======================================================================
    // Creation Utilities
	// ----------------------------------------------------------------------
    void CreateModule(WD_EditorObject parent, WD_Storage storage, string name= "") {
        storage.EditorObjects.CreateModule(parent.InstanceId, MenuPosition, name);
        Reset();        
    }
	// ----------------------------------------------------------------------
    void CreateStateChart(WD_EditorObject parent, WD_Storage storage, string name= "") {
        storage.EditorObjects.CreateStateChart(parent.InstanceId, MenuPosition, name);
        Reset();        
    }
	// ----------------------------------------------------------------------
    void CreateState(WD_EditorObject parent, WD_Storage storage, string name= "") {
        storage.EditorObjects.CreateState(parent.InstanceId, MenuPosition, name);
        Reset();        
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
