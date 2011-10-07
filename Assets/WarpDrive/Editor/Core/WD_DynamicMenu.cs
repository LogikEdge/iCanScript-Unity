using UnityEngine;
using System.Collections;

public class WD_DynamicMenu {
    enum MenuStateEnum { Idle, Behaviour, Module, StateChart, State, Function, Port, Company, Package }
    
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    MenuStateEnum   CurrentState= MenuStateEnum.Idle;
    string          SelectedCompany= null;
    string          SelectedPackage= null;
    string          SelectedFunction= null;
    Vector2         MenuPosition= Vector2.zero;
    int             Selection   = -1;
    
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    public bool IsActive { get { return CurrentState != MenuStateEnum.Idle; }}

	// ----------------------------------------------------------------------
    void Reset() {
        CurrentState= MenuStateEnum.Idle;
        SelectedCompany= null;
        SelectedPackage= null;
        SelectedFunction= null;
        MenuPosition= Vector2.zero;
        Selection= -1;
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
    public void Update(WD_EditorObject selectedObject, WD_Storage storage, Vector2 mouseDownPosition) {
        // Update mouse position if not already done.
        if(MenuPosition == Vector2.zero) MenuPosition= mouseDownPosition;

        // Nothing to show if menu is inactive.
        if(MenuPosition != mouseDownPosition || !IsActive) {
            Reset();
            return;
        }

        // Process the menu state.
        switch(CurrentState) {
            case MenuStateEnum.Idle:
                break;
            case MenuStateEnum.Behaviour:
                BehaviourMenu();
                break;
            case MenuStateEnum.Module:
                ModuleMenu();
                break;
            case MenuStateEnum.StateChart:
                StateChartMenu();
                break;
            case MenuStateEnum.State:
                StateMenu();
                break;
            case MenuStateEnum.Function:
                FunctionMenu(selectedObject, storage);
                break;
            case MenuStateEnum.Port:
                PortMenu();
                break;
            case MenuStateEnum.Company:
                CompanyMenu();
                break;
            case MenuStateEnum.Package:
                PackageMenu();
                break;
        }
    }

	// ----------------------------------------------------------------------
    void BehaviourMenu() {
        string[] menu= new string[]
            { "Add Update ...",             // 0
              "Add Late Update ...",        // 1
              "Add Fixed Update ...",       // 2
              "OnGUI ...",                  // 3
              "OnDrawGizmos ..."            // 4
            };
        if(ShowMenu(menu) != -1) {
            switch(Selection) {
                case 0: break;
                case 1: break;
                case 2: break;
                case 3: break;
                case 4: break;
                default: Reset(); break;
            }
        }
    }
	// ----------------------------------------------------------------------
    void ModuleMenu() {
        string[] menu= new string[]
        {
            "Add Module",
            "Add StateChart",
            "Delete"
        };
        if(ShowMenu(menu) != -1) {
            Reset();
        }
    }
	// ----------------------------------------------------------------------
    void StateChartMenu() {
        string[] menu= new string[]
        {
            "Add State",
            "Add Entry State",
            "Delete"
        };
        if(ShowMenu(menu) != -1) {
            Reset();
        }
    }
	// ----------------------------------------------------------------------
    void StateMenu() {
        string[] menu= new string[]
        {
            "Add OnEntry Module",
            "Add OnUpdate Module",
            "Add OnExit Module",
            "Add SubState",
            "Delete"
        };
        if(ShowMenu(menu) != -1) {
            Reset();
        }
    }
	// ----------------------------------------------------------------------
    void FunctionMenu() {
        string[] menu= new string[]
        {
            "Delete"
        };
        if(ShowMenu(menu) != -1) {
            Reset();
        }        
    }
	// ----------------------------------------------------------------------
    void PortMenu() {
        string[] menu= new string[]
        {
            "Publish to Module",
            "Delete"
        };
        if(ShowMenu(menu) != -1) {
            Reset();
        }                
    }
	// ----------------------------------------------------------------------
    void CompanyMenu() {
        string[] companies= WD_DataBase.GetCompanies();
        if(ShowMenu(companies) != -1) {
            SelectedCompany= companies[Selection];
            CurrentState= MenuStateEnum.Package;
            Selection= -1;
        }        
    }
	// ----------------------------------------------------------------------
    void PackageMenu() {
        string[] packages= WD_DataBase.GetPackages(SelectedCompany);
        if(ShowMenu(packages) != -1) {
            SelectedPackage= packages[Selection];
            CurrentState= MenuStateEnum.Function;
            Selection= -1;
        }                
    }
	// ----------------------------------------------------------------------
    void FunctionMenu(WD_EditorObject selectedObject, WD_Storage storage) {
        // Gather all functions.
        string[] functions  = WD_DataBase.GetFunctions(SelectedCompany, SelectedPackage);
        if(ShowMenu(functions) != -1) {
            SelectedFunction= functions[Selection];
            CurrentState= MenuStateEnum.Idle;
            Selection= -1;
            Debug.Log("Selected: "+SelectedCompany+":"+SelectedPackage+":"+SelectedFunction);
            WD_BaseDesc desc= WD_DataBase.GetDescriptor(SelectedCompany, SelectedPackage, SelectedFunction);
            if(desc == null) {
                Debug.LogError("Unable to find: "+SelectedCompany+":"+SelectedPackage+":"+SelectedFunction+" in Database !!!");
            };
            desc.CreateInstance(storage.EditorObjects, (selectedObject != null ? selectedObject.InstanceId : -1), MenuPosition);
        }                

    }

    // ======================================================================
    // Utilities
	// ----------------------------------------------------------------------
    Vector2 GetMaxSize(string[] menuItems) {
        Vector2 maxSize= Vector2.zero;
        foreach(var item in menuItems) {
            Vector2 size= GUI.skin.button.CalcSize(new GUIContent(item));
            if(size.x > maxSize.x) maxSize.x= size.x;
            if(size.y > maxSize.y) maxSize.y= size.y;
        }        
        return maxSize;
    }
	// ----------------------------------------------------------------------
    int ShowMenu(string[] menu, int width= -1) {
        Vector2 itemSize= GetMaxSize(menu);
        Selection= GUI.SelectionGrid(new Rect(MenuPosition.x,MenuPosition.y,itemSize.x,itemSize.y*menu.Length), Selection, menu, 1);        
        return Selection;
    }
}
