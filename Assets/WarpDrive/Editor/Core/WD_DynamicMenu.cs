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
    public bool IsActive= false;
    Vector2     MenuPosition= Vector2.zero;
    
    // ======================================================================
    // Menu Items
	// ----------------------------------------------------------------------
    const string DeleteStr= "Delete";
    const string FoldStr= "Fold";
    const string UnfoldStr= "Unfold";
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
        string[] menu= new string[]
        {
            ModuleStr,
            StateChartStr,
        };
        // Fold/Expand menu items
        string[] tmp= null;
        if(!selectedObject.IsMinimized) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            if(selectedObject.IsFolded) {
                tmp[menu.Length+1]= UnfoldStr;
            } else {
                tmp[menu.Length+1]= FoldStr;            
            }
            menu= tmp;            
        }
        // Function menu items
        string[] functionMenu= GetFunctionMenu();
        tmp= new string[menu.Length+functionMenu.Length+1];
        menu.CopyTo(tmp, 0);
        tmp[menu.Length]= SeparatorStr;
        functionMenu.CopyTo(tmp, menu.Length+1);
        menu= tmp;
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
        string[] menu= new string[]
        {
            StateStr,
            EntryStateStr,
        };
        // Fold/Expand menu items
        string[] tmp= null;
        if(!selectedObject.IsMinimized) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            if(selectedObject.IsFolded) {
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
        string[] menu= new string[]
        {
            OnEntryStr,
            OnUpdateStr,
            OnExitStr,
            SubStateStr,
        };
        // Fold/Expand menu items
        string[] tmp= null;
        if(!selectedObject.IsMinimized) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            if(selectedObject.IsFolded) {
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
        if(!selectedObject.IsMinimized) {
            tmp= new string[menu.Length+2];
            menu.CopyTo(tmp, 0);
            tmp[menu.Length]= SeparatorStr;
            if(selectedObject.IsFolded) {
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
            menu= new string[]{PublishPortStr};
        }
        // Allow to delete a port if its parent is a module.
        if(selectedObject.IsModulePort && parent.IsModule) {
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
	// ----------------------------------------------------------------------
    void ProcessMenu(object obj) {
        MenuContext context= obj as MenuContext;
        WD_EditorObject selectedObject= context.SelectedObject;
        WD_IStorage storage= context.Storage;
        switch(context.Command) {
            case UpdateModuleStr:           CreateModule(selectedObject, storage, "Update"); break;
            case UpdateStateChartStr:       CreateStateChart(selectedObject, storage, "Update"); break;
            case FixedUpdateModuleStr:      CreateModule(selectedObject, storage, "FixedUpdate"); break;
            case FixedUpdateStateChartStr:  CreateStateChart(selectedObject, storage, "FixedUpdate"); break;
            case LateUpdateModuleStr:       CreateModule(selectedObject, storage, "LateUpdate"); break;
            case LateUpdateStateChartStr:   CreateStateChart(selectedObject, storage, "LateUpdate"); break;
            case ModuleStr:                 CreateModule(selectedObject, storage); break;
            case StateChartStr:             CreateStateChart(selectedObject, storage); break;
            case StateStr:                  CreateState (selectedObject, storage);  break;
            case OnEntryStr:                CreateModule(selectedObject, storage, OnEntryStr); break;
            case OnUpdateStr:               CreateModule(selectedObject, storage, OnUpdateStr); break;
            case OnExitStr:                 CreateModule(selectedObject, storage, OnExitStr); break;
            case SubStateStr:               CreateState (selectedObject, storage);  break;
            case FoldStr:                   storage.Fold(selectedObject); break;
            case UnfoldStr:                 storage.Unfold(selectedObject); break;
            case DeleteStr:                 DestroySelectedObject(selectedObject, storage); break;
            case PublishPortStr:
                WD_EditorObject parent= storage.GetParent(selectedObject);
                WD_EditorObject grandParent= storage.GetParent(parent);
                int grandParentId= grandParent.InstanceId;
                if(selectedObject.IsInputPort) {
                    WD_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, WD_ObjectTypeEnum.InModulePort);
                    storage.SetSource(selectedObject, port);
                    port.LocalPosition= new Rect(0, parent.LocalPosition.y+selectedObject.LocalPosition.y, 0, 0);
                } else {
                    WD_EditorObject port= storage.CreatePort(selectedObject.Name, grandParentId, selectedObject.RuntimeType, WD_ObjectTypeEnum.OutModulePort);
                    storage.SetSource(port, selectedObject);
                    port.LocalPosition= new Rect(grandParent.LocalPosition.width, parent.LocalPosition.y+selectedObject.LocalPosition.y, 0, 0);
                }
                grandParent.IsDirty= true;
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
    // Creation Utilities
	// ----------------------------------------------------------------------
    void CreateModule(WD_EditorObject parent, WD_IStorage storage, string name= "") {
        storage.CreateModule(parent.InstanceId, MenuPosition, name);
    }
	// ----------------------------------------------------------------------
    void CreateStateChart(WD_EditorObject parent, WD_IStorage storage, string name= "") {
        storage.CreateStateChart(parent.InstanceId, MenuPosition, name);
    }
	// ----------------------------------------------------------------------
    void CreateState(WD_EditorObject parent, WD_IStorage storage, string name= "") {
        storage.CreateState(parent.InstanceId, MenuPosition, name);
    }
	// ----------------------------------------------------------------------
    void CreateFunction(WD_EditorObject parent, WD_IStorage storage, WD_BaseDesc desc) {
        storage.CreateFunction(parent.InstanceId, MenuPosition, desc);
    }
	// ----------------------------------------------------------------------
    bool DestroySelectedObject(WD_EditorObject selectedObject, WD_IStorage storage) {
        bool isDestroyed= false;
        if(EditorUtility.DisplayDialog("Deleting "+selectedObject.ObjectType, "Are you sure you want to remove "+selectedObject.ObjectType+": "+selectedObject.Name, "Delete", "Cancel")) {
            storage.DestroyInstance(selectedObject.InstanceId);                        
            isDestroyed= true;
        }
        Reset();
        return isDestroyed;
    }
}
